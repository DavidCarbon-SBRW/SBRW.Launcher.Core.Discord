using DiscordRPC;
using System;
using System.Collections.Generic;
using System.Linq;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Required.Anti_Cheat;
using System.Text.RegularExpressions;
using System.Xml;
using DiscordButton = DiscordRPC.Button;
using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Discord.Reference_.List_;
using SBRW.Launcher.Core.Extension.String_;
using System.Threading.Tasks;

namespace SBRW.Launcher.Core.Discord.RPC_
{
    /// <summary>
    /// Discord RPC Set from Server Side
    /// </summary>
    public class Presence_Game
    {
        private static RichPresence Server_Presence { get; set; } = new RichPresence();
        /* Some checks */
        private static bool CanUpdateProfileField { get; set; }
        private static int EventID { get; set; }
        private static string CarslotsXML { get; set; } = string.Empty;
        private static bool InSafeHouse { get; set; }
        /* Some data related, can be touched. */
        private static string PersonaId { get; set; } = string.Empty;
        private static string PersonaLevel { get; set; } = string.Empty;
        private static string PersonaAvatarId { get; set; } = string.Empty;
        private static string LoggedPersonaId { get; set; } = string.Empty;
        private static string LauncherRPC { get; set; } = "SBRW Launcher: v" + Presence_Settings.Launcher_Version;
        private static int PersonaTreasure { get; set; } = 0;
        private static int TotalTreasure { get; set; } = 15;
        private static int THDay { get; set; } = 0;
        private static List<string> PersonaIds { get; set; } = new List<string>();
        private static Dictionary<string, object> QueryParams { get; set; } = new Dictionary<string, object>();
        private static string GETContent { get; set; } = string.Empty;
        /// <summary>
        /// Game Status State
        /// </summary>
        /// <param name="Uri">Address Path</param>
        /// <param name="Server_Reply">XML string File</param>
        /// <param name="GET">Sub-Path in Address Path</param>
        public static void State(string Uri, string Server_Reply, dynamic GET)
        {
            try
            {
                if (QueryParams.Count > 0)
                {
                    QueryParams.Clear();
                }

                foreach (dynamic param in GET)
                {
                    dynamic value = GET[param];
                    QueryParams[param] = value;
                }

                GETContent = string.Join(";", QueryParams.Select(x => x.Key + "=" + x.Value).ToArray());
            }
            catch (Exception Error)
            {
                Log_Detail.OpenLog("DISCORD GAME PRESENCE [GET]", null, Error, null, true);
            }

            try
            {
                XmlDocument SBRW_XML = new XmlDocument();
                string[] splitted_Uri = Uri.Split('/');

                string _serverPanelLink = Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home;
                string _serverWebsiteLink = Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home;
                string _serverDiscordLink = Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Discord;
                if (!string.IsNullOrWhiteSpace(_serverWebsiteLink) || !string.IsNullOrWhiteSpace(_serverDiscordLink) || !string.IsNullOrWhiteSpace(_serverPanelLink))
                {
                    Presence_Launcher.ButtonsList.Clear();

                    if (!string.IsNullOrWhiteSpace(_serverPanelLink))
                    {
                        /* Let's format it now, if possible */
                        if (Launcher_Value.Game_Persona_ID == string.Empty || Launcher_Value.Game_Persona_Name == string.Empty)
                        {
                            Presence_Launcher.ButtonsList.Add(new DiscordButton()
                            {
                                Label = "View Panel",
                                Url = _serverPanelLink.Split(new string[] { "{sep}" }, StringSplitOptions.None)[0]
                            });
                        }
                        else
                        {
                            _serverPanelLink = _serverPanelLink.Replace("{personaid}", Launcher_Value.Game_Persona_ID);
                            _serverPanelLink = _serverPanelLink.Replace("{personaname}", Launcher_Value.Game_Persona_Name);
                            _serverPanelLink = _serverPanelLink.Replace("{sep}", string.Empty);

                            Presence_Launcher.ButtonsList.Add(new DiscordButton()
                            {
                                Label = "Check " + Launcher_Value.Game_Persona_Name + " on Panel",
                                Url = _serverPanelLink
                            });
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(_serverWebsiteLink) && _serverWebsiteLink != _serverDiscordLink)
                    {
                        Presence_Launcher.ButtonsList.Add(new DiscordButton()
                        {
                            Label = "Website",
                            Url = _serverWebsiteLink
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(_serverDiscordLink))
                    {
                        Presence_Launcher.ButtonsList.Add(new DiscordButton()
                        {
                            Label = "Discord",
                            Url = _serverDiscordLink
                        });
                    }
                }

                if (Uri == "/User/SecureLoginPersona")
                {
                    LoggedPersonaId = GETContent.Split(';').Last().Split('=').Last();
                    CanUpdateProfileField = true;
                }

                if (Uri == "/User/SecureLogoutPersona")
                {
                    PersonaId = string.Empty;
                    Launcher_Value.Game_Persona_Name_Live = string.Empty;
                    PersonaLevel = string.Empty;
                    PersonaAvatarId = string.Empty;
                    Launcher_Value.Game_Car_Name = string.Empty;
                    LauncherRPC = string.Empty;
                    PersonaTreasure = 0;
                }

                /* FIRST PERSONA EVER LOCALIZED IN CODE */
                if (Uri == "/User/GetPermanentSession")
                {
                    SBRW_XML.LoadXml(Server_Reply);

                    Launcher_Value.Game_Persona_Name_Live = SBRW_XML.SelectSingleNode("UserInfo/personas/ProfileData/Name").InnerText.Replace("¤", "[S]");
                    PersonaLevel = SBRW_XML.SelectSingleNode("UserInfo/personas/ProfileData/Level").InnerText;
                    PersonaAvatarId = "avatar_" + SBRW_XML.SelectSingleNode("UserInfo/personas/ProfileData/IconIndex").InnerText;
                    PersonaId = SBRW_XML.SelectSingleNode("UserInfo/personas/ProfileData/PersonaId").InnerText;

                    /* Let's get rest of PERSONAIDs */
                    XmlNode UserInfo = SBRW_XML.SelectSingleNode("UserInfo");
                    XmlNodeList personas = UserInfo.SelectNodes("personas/ProfileData");
                    foreach (XmlNode node in personas)
                    {
                        PersonaIds.Add(node.SelectSingleNode("PersonaId").InnerText);
                    }
                }

                /* CREATE/DELETE PERSONA Handler  */
                if (Uri == "/DriverPersona/CreatePersona")
                {
                    SBRW_XML.LoadXml(Server_Reply);
                    PersonaIds.Add(SBRW_XML.SelectSingleNode("ProfileData/PersonaId").InnerText);
                }

                /* DRIVING CARNAME */
                if (Uri == "/DriverPersona/GetPersonaInfo" && CanUpdateProfileField)
                {
                    if (LoggedPersonaId == GETContent.Split(';').Last().Split('=').Last())
                    {
                        SBRW_XML.LoadXml(Server_Reply);
                        Launcher_Value.Game_Persona_Name_Live = SBRW_XML.SelectSingleNode("ProfileData/Name").InnerText.Replace("¤", "[S]");
                        PersonaLevel = SBRW_XML.SelectSingleNode("ProfileData/Level").InnerText;
                        PersonaAvatarId = "avatar_" + SBRW_XML.SelectSingleNode("ProfileData/IconIndex").InnerText;
                        PersonaId = SBRW_XML.SelectSingleNode("ProfileData/PersonaId").InnerText;

                        Launcher_Value.Game_Persona_ID = SBRW_XML.SelectSingleNode("ProfileData/PersonaId").InnerText;
                        Launcher_Value.Game_Persona_Name = SBRW_XML.SelectSingleNode("ProfileData/Name").InnerText.Replace("¤", "[S]");
                    }
                }

                if (Uri == "/events/gettreasurehunteventsession")
                {
                    /* Treasure Hunt Streak/Gems From Server */
                    PersonaTreasure = 0;
                    TotalTreasure = 15;
                    THDay = 0;

                    SBRW_XML.LoadXml(Server_Reply);
                    var xPersonaTreasure = Convert.ToInt32(SBRW_XML.SelectSingleNode("TreasureHuntEventSession/CoinsCollected").InnerText);
                    for (var i = 0; i < 15; i++)
                    {
                        if ((xPersonaTreasure & (1 << (15 - i))) != 0) PersonaTreasure++;
                    }

                    TotalTreasure = Convert.ToInt32(SBRW_XML.SelectSingleNode("TreasureHuntEventSession/NumCoins").InnerText);
                    THDay = Convert.ToInt32(SBRW_XML.SelectSingleNode("TreasureHuntEventSession/Streak").InnerText);
                }

                if (Uri == "/events/notifycoincollected")
                {
                    Launcher_Value.Game_In_Event = false;

                    /* Actively Collection Treasure Hunt Gems */
                    PersonaTreasure++;

                    if (PersonaTreasure != TotalTreasure)
                    {
                        Server_Presence.Details = "Collecting Gems (" + PersonaTreasure + " of " + TotalTreasure + ")";
                    }
                    else if (PersonaTreasure == TotalTreasure)
                    {
                        Server_Presence.Details = "Finished Collecting Gems (" + PersonaTreasure + " of " + TotalTreasure + ")";
                    }

                    Server_Presence.State = LauncherRPC;
                    Server_Presence.Assets = new Assets
                    {
                        LargeImageText = Launcher_Value.Game_Persona_Name_Live + " - Level: " + PersonaLevel,
                        LargeImageKey = PersonaAvatarId,
                        SmallImageText = "Treasure Hunt - Day: " + THDay,
                        SmallImageKey = "gamemode_treasure"
                    };
                    Server_Presence.Buttons = Presence_Launcher.ButtonsList.ToArray();

                    if (Presence_Launcher.Running()) Presence_Launcher.Client.SetPresence(Server_Presence);
                }

                /* IN SAFEHOUSE/FREEROAM */
                if (Uri == "/DriverPersona/UpdatePersonaPresence")
                {
                    string UpdatePersonaPresenceParam = GETContent.Split(';').Last().Split('=').Last();
                    Server_Presence.Assets = new Assets();
                    if (UpdatePersonaPresenceParam == "1")
                    {
                        Server_Presence.Details = "Driving " + Launcher_Value.Game_Car_Name;
                        Server_Presence.Assets.SmallImageText = "In-Freeroam";
                        Server_Presence.Assets.SmallImageKey = "gamemode_freeroam";
                        Server_Presence.State = LauncherRPC;
                        Launcher_Value.Game_In_Event = false;
                        InSafeHouse = false;
                    }
                    else
                    {
                        Server_Presence.Details = "In Safehouse";
                        Server_Presence.Assets.SmallImageText = "In-Safehouse";
                        Server_Presence.Assets.SmallImageKey = "gamemode_safehouse";
                        Server_Presence.State = Launcher_Value.Game_Server_Name;
                        Launcher_Value.Game_In_Event = true;;
                        InSafeHouse = true;
                    }

                    Server_Presence.Assets.LargeImageText = Launcher_Value.Game_Persona_Name_Live + " - Level: " + PersonaLevel;
                    Server_Presence.Assets.LargeImageKey = PersonaAvatarId;
                    Server_Presence.Buttons = Presence_Launcher.ButtonsList.ToArray();

                    if (Presence_Launcher.Running()) Presence_Launcher.Client.SetPresence(Server_Presence);
                }

                if (Uri == "/matchmaking/leavelobby" || Uri == "/matchmaking/declineinvite")
                {
                    /* Display Current Car in Freeroam */
                    Server_Presence.Details = "Driving " + Launcher_Value.Game_Car_Name;
                    Server_Presence.State = LauncherRPC;
                    Server_Presence.Assets = new Assets
                    {
                        LargeImageText = Launcher_Value.Game_Persona_Name_Live + " - Level: " + PersonaLevel,
                        LargeImageKey = PersonaAvatarId,
                        SmallImageText = "In-Freeroam",
                        SmallImageKey = "gamemode_freeroam"
                    };
                    Server_Presence.Buttons = Presence_Launcher.ButtonsList.ToArray();

                    if (Uri == "/matchmaking/leavelobby")
                    {
                        AC_Core.Stop(false);
                    }

                    Launcher_Value.Game_In_Event = false;

                    if (Presence_Launcher.Running()) Presence_Launcher.Client.SetPresence(Server_Presence);
                }
                /* IN LOBBY */
                else if (Uri == "/matchmaking/acceptinvite")
                {
                    /* Accept (Group/Search) Event Invite */
                    Launcher_Value.Game_In_Event = true;

                    SBRW_XML.LoadXml(Server_Reply);
                    XmlNode eventIdNode = SBRW_XML.SelectSingleNode("LobbyInfo/EventId");

                    if (eventIdNode != null)
                    {
                        EventID = Convert.ToInt32(eventIdNode.InnerText);

                        Server_Presence.Details = "In Lobby: " + Events.Get_Name(EventID);
                        Server_Presence.State = Launcher_Value.Game_Server_Name;
                        Server_Presence.Assets = new Assets
                        {
                            LargeImageText = Launcher_Value.Game_Persona_Name_Live + " - Level: " + PersonaLevel,
                            LargeImageKey = PersonaAvatarId,
                            SmallImageText = LauncherRPC,
                            SmallImageKey = Events.Get_Type(Convert.ToInt32(EventID))
                        };
                        Server_Presence.Buttons = Presence_Launcher.ButtonsList.ToArray();

                        if (Presence_Launcher.Running()) Presence_Launcher.Client.SetPresence(Server_Presence);
                    }
                }
                else if (Uri == "/matchmaking/joinqueueracenow")
                {
                    Launcher_Value.Game_In_Event = false;

                    /* Searching for Events */
                    Server_Presence.Details = "Searching for Event";
                    Server_Presence.State = LauncherRPC;
                    Server_Presence.Assets = new Assets
                    {
                        LargeImageText = Launcher_Value.Game_Persona_Name_Live + " - Level: " + PersonaLevel,
                        LargeImageKey = PersonaAvatarId,
                        SmallImageText = "In-Freeroam",
                        SmallImageKey = "gamemode_freeroam"
                    };
                    Server_Presence.Buttons = Presence_Launcher.ButtonsList.ToArray();

                    if (Presence_Launcher.Running()) Presence_Launcher.Client.SetPresence(Server_Presence);
                }

                /* IN EVENT */
                if (Regex.Match(Uri, "/matchmaking/launchevent").Success)
                {
                    /* Singleplayer Event (Launch) */
                    Launcher_Value.Game_In_Event = true;

                    EventID = Convert.ToInt32(splitted_Uri[3]);

                    Server_Presence.Details = "Loading Event: " + Events.Get_Name(EventID);
                    Server_Presence.State = Launcher_Value.Game_Server_Name;
                    Server_Presence.Assets = new Assets
                    {
                        LargeImageText = Launcher_Value.Game_Persona_Name_Live + " - Level: " + PersonaLevel,
                        LargeImageKey = PersonaAvatarId,
                        SmallImageText = LauncherRPC,
                        SmallImageKey = Events.Get_Type(EventID)
                    };
                    Server_Presence.Buttons = Presence_Launcher.ButtonsList.ToArray();

                    if (Presence_Launcher.Running()) Presence_Launcher.Client.SetPresence(Server_Presence);
                }
                else if (Uri == "/event/launched" && Launcher_Value.Game_In_Event)
                {
                    /* Once the Race Starts */
                    Server_Presence.Details = "In Event: " + Events.Get_Name(EventID);
                    Server_Presence.State = Launcher_Value.Game_Server_Name;
                    Server_Presence.Assets = new Assets
                    {
                        LargeImageText = Launcher_Value.Game_Persona_Name_Live + " - Level: " + PersonaLevel,
                        LargeImageKey = PersonaAvatarId,
                        SmallImageText = LauncherRPC,
                        SmallImageKey = Events.Get_Type(EventID)
                    };
                    Server_Presence.Buttons = Presence_Launcher.ButtonsList.ToArray();

                    AC_Core.Start(Launcher_Value.Launcher_Select_Server_JSON.Server_Enable_Crew_Tags, true, 0, EventID);

                    if (Presence_Launcher.Running()) Presence_Launcher.Client.SetPresence(Server_Presence);
                }
                else if (Uri == "/event/arbitration")
                {
                    Launcher_Value.Game_In_Event = true;

                    /* Once the Race Finishes */
                    Server_Presence.Details = "Finished Event: " + Events.Get_Name(EventID);
                    Server_Presence.State = Launcher_Value.Game_Server_Name;
                    Server_Presence.Assets = new Assets
                    {
                        LargeImageText = Launcher_Value.Game_Persona_Name_Live + " - Level: " + PersonaLevel,
                        LargeImageKey = PersonaAvatarId,
                        SmallImageText = LauncherRPC,
                        SmallImageKey = Events.Get_Type(EventID)
                    };
                    Server_Presence.Buttons = Presence_Launcher.ButtonsList.ToArray();

                    AC_Core.Stop(true);
                    if (Presence_Launcher.Running()) Presence_Launcher.Client.SetPresence(Server_Presence);
                }

                /* Extending Safehouse */
                if (Uri.Contains("catalog") && InSafeHouse)
                {
                    if (GETContent.Contains("categoryName=NFSW_NA_EP_VINYLS_Category")) Server_Presence.Details = "In Safehouse - Applying Vinyls";
                    if (GETContent.Contains("clientProductType=PAINTS_BODY")) Server_Presence.Details = "In Safehouse - Applying Colors";
                    if (GETContent.Contains("clientProductType=PERFORMANCEPART")) Server_Presence.Details = "In Safehouse - Applying Performance Parts";
                    if (GETContent.Contains("clientProductType=VISUALPART")) Server_Presence.Details = "In Safehouse - Applying Visual Parts";
                    if (GETContent.Contains("clientProductType=SKILLMODPART")) Server_Presence.Details = "In Safehouse - Applying Skillmods";
                    if (GETContent.Contains("clientProductType=PRESETCAR")) Server_Presence.Details = "In Safehouse - Purchasing Car";
                    if (GETContent.Contains("categoryName=BoosterPacks")) Server_Presence.Details = "In Safehouse - Opening Cardpacks";

                    Server_Presence.Assets = new Assets
                    {
                        SmallImageText = "In-Safehouse",
                        SmallImageKey = "gamemode_safehouse"
                    };
                    Server_Presence.State = Launcher_Value.Game_Server_Name;
                    Server_Presence.Assets.LargeImageText = Launcher_Value.Game_Persona_Name_Live + " - Level: " + PersonaLevel;
                    Server_Presence.Assets.LargeImageKey = PersonaAvatarId;
                    Server_Presence.Buttons = Presence_Launcher.ButtonsList.ToArray();

                    if (Presence_Launcher.Running()) Presence_Launcher.Client.SetPresence(Server_Presence);
                }

                /* CARS RELATED */
                foreach (var single_personaId in PersonaIds)
                {
                    if (Regex.Match(Uri, "/personas/" + single_personaId + "/carslots", RegexOptions.IgnoreCase).Success)
                    {
                        CarslotsXML = Server_Reply;

                        SBRW_XML.LoadXml(CarslotsXML);

                        int DefaultID = Convert.ToInt32(SBRW_XML.SelectSingleNode("CarSlotInfoTrans/DefaultOwnedCarIndex").InnerText);
                        int current = 0;

                        XmlNode CarsOwnedByPersona = SBRW_XML.SelectSingleNode("CarSlotInfoTrans/CarsOwnedByPersona");
                        XmlNodeList OwnedCarTrans = CarsOwnedByPersona.SelectNodes("OwnedCarTrans");

                        foreach (XmlNode node in OwnedCarTrans)
                        {
                            if (DefaultID == current)
                            {
                                Launcher_Value.Game_Car_Name = Cars.Get_Name(Strings.Encode(node.SelectSingleNode("CustomCar/Name").InnerText));
                            }
                            current++;
                        }
                    }
                    if (Regex.Match(Uri, "/personas/" + single_personaId + "/defaultcar", RegexOptions.IgnoreCase).Success)
                    {
                        if (splitted_Uri.Last() != "defaultcar")
                        {
                            string receivedId = splitted_Uri.Last();

                            SBRW_XML.LoadXml(CarslotsXML);
                            XmlNode CarsOwnedByPersona = SBRW_XML.SelectSingleNode("CarSlotInfoTrans/CarsOwnedByPersona");
                            XmlNodeList OwnedCarTrans = CarsOwnedByPersona.SelectNodes("OwnedCarTrans");

                            foreach (XmlNode node in OwnedCarTrans)
                            {
                                if (receivedId == node.SelectSingleNode("Id").InnerText)
                                {
                                    Launcher_Value.Game_Car_Name = Cars.Get_Name(Strings.Encode(node.SelectSingleNode("CustomCar/Name").InnerText));
                                }
                            }
                        }
                    }
                }

                GETContent = string.Empty;
            }
            catch (Exception Error)
            {
                Log_Detail.OpenLog("DISCORD GAME PRESENCE", null, Error, null, true);
            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// Game Status State
        /// </summary>
        /// <param name="Uri">Address Path</param>
        /// <param name="Server_Reply">XML string File</param>
        /// <param name="GET">Sub-Path in Address Path</param>
        public static async void State_Async(string Uri, string Server_Reply, dynamic GET)
        {
            try
            {
                await Task.Run(() => State(Uri, Server_Reply, GET)).ConfigureAwait(false);
            }
            catch (Exception Error)
            {
                Log_Detail.OpenLog("DISCORD GAME PRESENCE [Library]", string.Empty, Error, string.Empty, true);
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
