﻿using DiscordRPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SBRW.Launcher.Core.Classes.Cache;
using SBRW.Launcher.Core.Classes.Required.Anti_Cheat;
using System.Text.RegularExpressions;
using System.Xml;
using DiscordButton = DiscordRPC.Button;
using SBRW.Launcher.Core.Extras.Proxy_.Nancy_;
using SBRW.Launcher.Core.Classes.Extension.Logging_;
using SBRW.Launcher.Core.Discord.Discord_.Reference_.List_;

namespace SBRW.Launcher.Core.Discord.Discord_.RPC_
{
    public class Presence_Game
    {
        public static RichPresence _presence = new RichPresence();

        /* Some checks */
        private static readonly string serverName = Proxy_Server.Instance.GetServerName();
        private static bool canUpdateProfileField = false;
        private static bool eventTerminatedManually = false;
        private static int EventID;
        private static string carslotsXML = String.Empty;
        private static bool inSafeHouse = false;
        public static bool T1000() => eventTerminatedManually && (Session_Timer.Remaining <= 0);

        /* Some data related, can be touched. */
        public static string PersonaId = String.Empty;
        public static string PersonaLevel = String.Empty;
        public static string PersonaAvatarId = String.Empty;
        public static string LoggedPersonaId = String.Empty;
        public static string LauncherRPC = "SBRW Launcher: v" + Presence_Settings.Launcher_Version;
        public static int PersonaTreasure = 0;
        public static int TotalTreasure = 15;
        public static int THDay = 0;
        public static List<string> PersonaIds = new List<string>();
        public static Dictionary<string, object> QueryParams = new Dictionary<string, object>();
        public static string GETContent = string.Empty;

        public static void HandleGameState(string uri, string serverreply, dynamic GET)
        {
            try
            {
                foreach (var param in GET)
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
            finally
            {
                QueryParams.Clear();
            }

            try
            {
                var SBRW_XML = new XmlDocument();
                string[] splitted_uri = uri.Split('/');

                String _serverPanelLink = Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home;
                String _serverWebsiteLink = Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home;
                String _serverDiscordLink = Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Discord;
                if (!String.IsNullOrWhiteSpace(_serverWebsiteLink) || !String.IsNullOrWhiteSpace(_serverDiscordLink) || !String.IsNullOrWhiteSpace(_serverPanelLink))
                {
                    Presence_Launcher.ButtonsList.Clear();

                    if (!String.IsNullOrWhiteSpace(_serverPanelLink))
                    {
                        /* Let's format it now, if possible */
                        if (Launcher_Value.Game_Persona_ID == String.Empty || Launcher_Value.Game_Persona_Name == String.Empty)
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
                            _serverPanelLink = _serverPanelLink.Replace("{sep}", String.Empty);

                            Presence_Launcher.ButtonsList.Add(new DiscordButton()
                            {
                                Label = "Check " + Launcher_Value.Game_Persona_Name + " on Panel",
                                Url = _serverPanelLink
                            });
                        }
                    }
                    else if (!String.IsNullOrWhiteSpace(_serverWebsiteLink) && _serverWebsiteLink != _serverDiscordLink)
                    {
                        Presence_Launcher.ButtonsList.Add(new DiscordButton()
                        {
                            Label = "Website",
                            Url = _serverWebsiteLink
                        });
                    }

                    if (!String.IsNullOrWhiteSpace(_serverDiscordLink))
                    {
                        Presence_Launcher.ButtonsList.Add(new DiscordButton()
                        {
                            Label = "Discord",
                            Url = _serverDiscordLink
                        });
                    }
                }

                if (uri == "/User/SecureLoginPersona")
                {
                    LoggedPersonaId = GETContent.Split(';').Last().Split('=').Last();
                    canUpdateProfileField = true;
                }

                if (uri == "/User/SecureLogoutPersona")
                {
                    PersonaId = String.Empty;
                    Launcher_Value.Game_Persona_Name_Live = String.Empty;
                    PersonaLevel = String.Empty;
                    PersonaAvatarId = String.Empty;
                    Launcher_Value.Game_Car_Name = String.Empty;
                    LauncherRPC = String.Empty;
                    PersonaTreasure = 0;
                }

                /* FIRST PERSONA EVER LOCALIZED IN CODE */
                if (uri == "/User/GetPermanentSession")
                {
                    /* Moved Statuses.cs Code to Gist | Check RemovedClasses.cs for Link */
                    //try { Statuses.getToken(); } catch { }

                    SBRW_XML.LoadXml(serverreply);

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
                if (uri == "/DriverPersona/CreatePersona")
                {
                    SBRW_XML.LoadXml(serverreply);
                    PersonaIds.Add(SBRW_XML.SelectSingleNode("ProfileData/PersonaId").InnerText);
                }

                /* DRIVING CARNAME */
                if (uri == "/DriverPersona/GetPersonaInfo" && canUpdateProfileField)
                {
                    if (LoggedPersonaId == GETContent.Split(';').Last().Split('=').Last())
                    {
                        SBRW_XML.LoadXml(serverreply);
                        Launcher_Value.Game_Persona_Name_Live = SBRW_XML.SelectSingleNode("ProfileData/Name").InnerText.Replace("¤", "[S]");
                        PersonaLevel = SBRW_XML.SelectSingleNode("ProfileData/Level").InnerText;
                        PersonaAvatarId = "avatar_" + SBRW_XML.SelectSingleNode("ProfileData/IconIndex").InnerText;
                        PersonaId = SBRW_XML.SelectSingleNode("ProfileData/PersonaId").InnerText;

                        Launcher_Value.Game_Persona_ID = SBRW_XML.SelectSingleNode("ProfileData/PersonaId").InnerText;
                        Launcher_Value.Game_Persona_Name = SBRW_XML.SelectSingleNode("ProfileData/Name").InnerText.Replace("¤", "[S]");
                    }
                }

                if (uri == "/events/gettreasurehunteventsession")
                {
                    /* Treasure Hunt Streak/Gems From Server */
                    PersonaTreasure = 0;
                    TotalTreasure = 15;
                    THDay = 0;

                    SBRW_XML.LoadXml(serverreply);
                    var xPersonaTreasure = Convert.ToInt32(SBRW_XML.SelectSingleNode("TreasureHuntEventSession/CoinsCollected").InnerText);
                    for (var i = 0; i < 15; i++)
                    {
                        if ((xPersonaTreasure & (1 << (15 - i))) != 0) PersonaTreasure++;
                    }

                    TotalTreasure = Convert.ToInt32(SBRW_XML.SelectSingleNode("TreasureHuntEventSession/NumCoins").InnerText);
                    THDay = Convert.ToInt32(SBRW_XML.SelectSingleNode("TreasureHuntEventSession/Streak").InnerText);
                }

                if (uri == "/events/notifycoincollected")
                {
                    Launcher_Value.Game_In_Event = true;

                    /* Actively Collection Treasure Hunt Gems */
                    PersonaTreasure++;

                    if (PersonaTreasure != TotalTreasure)
                    {
                        _presence.Details = "Collecting Gems (" + PersonaTreasure + " of " + TotalTreasure + ")";
                    }
                    else if (PersonaTreasure == TotalTreasure)
                    {
                        _presence.Details = "Finished Collecting Gems (" + PersonaTreasure + " of " + TotalTreasure + ")";
                    }

                    _presence.State = LauncherRPC;
                    _presence.Assets = new Assets
                    {
                        LargeImageText = Launcher_Value.Game_Persona_Name_Live + " - Level: " + PersonaLevel,
                        LargeImageKey = PersonaAvatarId,
                        SmallImageText = "Treasure Hunt - Day: " + THDay,
                        SmallImageKey = "gamemode_treasure"
                    };
                    _presence.Buttons = Presence_Launcher.ButtonsList.ToArray();

                    if (Presence_Launcher.Running()) Presence_Launcher.Client.SetPresence(_presence);
                }

                /* IN SAFEHOUSE/FREEROAM */
                if (uri == "/DriverPersona/UpdatePersonaPresence")
                {
                    string UpdatePersonaPresenceParam = GETContent.Split(';').Last().Split('=').Last();
                    _presence.Assets = new Assets();
                    if (UpdatePersonaPresenceParam == "1")
                    {
                        _presence.Details = "Driving " + Launcher_Value.Game_Car_Name;
                        _presence.Assets.SmallImageText = "In-Freeroam";
                        _presence.Assets.SmallImageKey = "gamemode_freeroam";
                        _presence.State = LauncherRPC;
                        Launcher_Value.Game_In_Event = true;
                        inSafeHouse = false;
                    }
                    else
                    {
                        _presence.Details = "In Safehouse";
                        _presence.Assets.SmallImageText = "In-Safehouse";
                        _presence.Assets.SmallImageKey = "gamemode_safehouse";
                        _presence.State = serverName;
                        Launcher_Value.Game_In_Event = false;
                        inSafeHouse = true;
                    }

                    _presence.Assets.LargeImageText = Launcher_Value.Game_Persona_Name_Live + " - Level: " + PersonaLevel;
                    _presence.Assets.LargeImageKey = PersonaAvatarId;
                    _presence.Buttons = Presence_Launcher.ButtonsList.ToArray();

                    if (Presence_Launcher.Running()) Presence_Launcher.Client.SetPresence(_presence);
                }

                if (uri == "/matchmaking/leavelobby" || uri == "/matchmaking/declineinvite")
                {
                    /* Display Current Car in Freeroam */
                    _presence.Details = "Driving " + Launcher_Value.Game_Car_Name;
                    _presence.State = LauncherRPC;
                    _presence.Assets = new Assets
                    {
                        LargeImageText = Launcher_Value.Game_Persona_Name_Live + " - Level: " + PersonaLevel,
                        LargeImageKey = PersonaAvatarId,
                        SmallImageText = "In-Freeroam",
                        SmallImageKey = "gamemode_freeroam"
                    };
                    _presence.Buttons = Presence_Launcher.ButtonsList.ToArray();

                    if (uri == "/matchmaking/leavelobby")
                    {
                        AC_Core.Stop(false);
                    }

                    Launcher_Value.Game_In_Event = true;

                    if (Presence_Launcher.Running()) Presence_Launcher.Client.SetPresence(_presence);
                }
                /* IN LOBBY */
                else if (uri == "/matchmaking/acceptinvite")
                {
                    /* Accept (Group/Search) Event Invite */
                    Launcher_Value.Game_In_Event = false;

                    SBRW_XML.LoadXml(serverreply);
                    XmlNode eventIdNode = SBRW_XML.SelectSingleNode("LobbyInfo/EventId");

                    if (eventIdNode != null)
                    {
                        EventID = Convert.ToInt32(eventIdNode.InnerText);

                        _presence.Details = "In Lobby: " + Events.GetEventName(EventID);
                        _presence.State = serverName;
                        _presence.Assets = new Assets
                        {
                            LargeImageText = Launcher_Value.Game_Persona_Name_Live + " - Level: " + PersonaLevel,
                            LargeImageKey = PersonaAvatarId,
                            SmallImageText = LauncherRPC,
                            SmallImageKey = Events.GetEventType(Convert.ToInt32(EventID))
                        };
                        _presence.Buttons = Presence_Launcher.ButtonsList.ToArray();

                        if (Presence_Launcher.Running()) Presence_Launcher.Client.SetPresence(_presence);
                    }
                }
                else if (uri == "/matchmaking/joinqueueracenow")
                {
                    Launcher_Value.Game_In_Event = true;

                    /* Searching for Events */
                    _presence.Details = "Searching for Event";
                    _presence.State = LauncherRPC;
                    _presence.Assets = new Assets
                    {
                        LargeImageText = Launcher_Value.Game_Persona_Name_Live + " - Level: " + PersonaLevel,
                        LargeImageKey = PersonaAvatarId,
                        SmallImageText = "In-Freeroam",
                        SmallImageKey = "gamemode_freeroam"
                    };
                    _presence.Buttons = Presence_Launcher.ButtonsList.ToArray();

                    if (Presence_Launcher.Running()) Presence_Launcher.Client.SetPresence(_presence);
                }

                /* IN EVENT */
                if (Regex.Match(uri, "/matchmaking/launchevent").Success)
                {
                    /* Singleplayer Event (Launch) */
                    Launcher_Value.Game_In_Event = false;

                    EventID = Convert.ToInt32(splitted_uri[3]);

                    _presence.Details = "Loading Event: " + Events.GetEventName(EventID);
                    _presence.State = serverName;
                    _presence.Assets = new Assets
                    {
                        LargeImageText = Launcher_Value.Game_Persona_Name_Live + " - Level: " + PersonaLevel,
                        LargeImageKey = PersonaAvatarId,
                        SmallImageText = LauncherRPC,
                        SmallImageKey = Events.GetEventType(EventID)
                    };
                    _presence.Buttons = Presence_Launcher.ButtonsList.ToArray();

                    if (Presence_Launcher.Running()) Presence_Launcher.Client.SetPresence(_presence);
                }
                else if (uri == "/event/launched" && !eventTerminatedManually)
                {
                    /* Once the Race Starts */
                    _presence.Details = "In Event: " + Events.GetEventName(EventID);
                    _presence.State = serverName;
                    _presence.Assets = new Assets
                    {
                        LargeImageText = Launcher_Value.Game_Persona_Name_Live + " - Level: " + PersonaLevel,
                        LargeImageKey = PersonaAvatarId,
                        SmallImageText = LauncherRPC,
                        SmallImageKey = Events.GetEventType(EventID)
                    };
                    _presence.Buttons = Presence_Launcher.ButtonsList.ToArray();

                    AC_Core.Start(Launcher_Value.Launcher_Select_Server_JSON.Server_Enable_Crew_Tags, false, Launcher_Value.Game_Process.Id, EventID);

                    if (Presence_Launcher.Running()) Presence_Launcher.Client.SetPresence(_presence);
                }
                else if (uri == "/event/arbitration")
                {
                    Launcher_Value.Game_In_Event = false;

                    /* Once the Race Finishes */
                    _presence.Details = "Finished Event: " + Events.GetEventName(EventID);
                    _presence.State = serverName;
                    _presence.Assets = new Assets
                    {
                        LargeImageText = Launcher_Value.Game_Persona_Name_Live + " - Level: " + PersonaLevel,
                        LargeImageKey = PersonaAvatarId,
                        SmallImageText = LauncherRPC,
                        SmallImageKey = Events.GetEventType(EventID)
                    };
                    _presence.Buttons = Presence_Launcher.ButtonsList.ToArray();

                    AC_Core.Stop(true);
                    if (Presence_Launcher.Running()) Presence_Launcher.Client.SetPresence(_presence);
                }

                /* Extending Safehouse */
                if (uri.Contains("catalog") && inSafeHouse)
                {
                    if (GETContent.Contains("categoryName=NFSW_NA_EP_VINYLS_Category")) _presence.Details = "In Safehouse - Applying Vinyls";
                    if (GETContent.Contains("clientProductType=PAINTS_BODY")) _presence.Details = "In Safehouse - Applying Colors";
                    if (GETContent.Contains("clientProductType=PERFORMANCEPART")) _presence.Details = "In Safehouse - Applying Performance Parts";
                    if (GETContent.Contains("clientProductType=VISUALPART")) _presence.Details = "In Safehouse - Applying Visual Parts";
                    if (GETContent.Contains("clientProductType=SKILLMODPART")) _presence.Details = "In Safehouse - Applying Skillmods";
                    if (GETContent.Contains("clientProductType=PRESETCAR")) _presence.Details = "In Safehouse - Purchasing Car";
                    if (GETContent.Contains("categoryName=BoosterPacks")) _presence.Details = "In Safehouse - Opening Cardpacks";

                    _presence.Assets = new Assets
                    {
                        SmallImageText = "In-Safehouse",
                        SmallImageKey = "gamemode_safehouse"
                    };
                    _presence.State = serverName;
                    _presence.Assets.LargeImageText = Launcher_Value.Game_Persona_Name_Live + " - Level: " + PersonaLevel;
                    _presence.Assets.LargeImageKey = PersonaAvatarId;
                    _presence.Buttons = Presence_Launcher.ButtonsList.ToArray();

                    if (Presence_Launcher.Running()) Presence_Launcher.Client.SetPresence(_presence);
                }

                /* CARS RELATED */
                foreach (var single_personaId in PersonaIds)
                {
                    if (Regex.Match(uri, "/personas/" + single_personaId + "/carslots", RegexOptions.IgnoreCase).Success)
                    {
                        carslotsXML = serverreply;

                        SBRW_XML.LoadXml(carslotsXML);

                        int DefaultID = Convert.ToInt32(SBRW_XML.SelectSingleNode("CarSlotInfoTrans/DefaultOwnedCarIndex").InnerText);
                        int current = 0;

                        XmlNode CarsOwnedByPersona = SBRW_XML.SelectSingleNode("CarSlotInfoTrans/CarsOwnedByPersona");
                        XmlNodeList OwnedCarTrans = CarsOwnedByPersona.SelectNodes("OwnedCarTrans");

                        foreach (XmlNode node in OwnedCarTrans)
                        {
                            if (DefaultID == current)
                            {
                                Launcher_Value.Game_Car_Name = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes
                                    (Cars.GetCarName(node.SelectSingleNode("CustomCar/Name").InnerText)));
                            }
                            current++;
                        }
                    }
                    if (Regex.Match(uri, "/personas/" + single_personaId + "/defaultcar", RegexOptions.IgnoreCase).Success)
                    {
                        if (splitted_uri.Last() != "defaultcar")
                        {
                            string receivedId = splitted_uri.Last();

                            SBRW_XML.LoadXml(carslotsXML);
                            XmlNode CarsOwnedByPersona = SBRW_XML.SelectSingleNode("CarSlotInfoTrans/CarsOwnedByPersona");
                            XmlNodeList OwnedCarTrans = CarsOwnedByPersona.SelectNodes("OwnedCarTrans");

                            foreach (XmlNode node in OwnedCarTrans)
                            {
                                if (receivedId == node.SelectSingleNode("Id").InnerText)
                                {
                                    Launcher_Value.Game_Car_Name = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(
                                        Cars.GetCarName(node.SelectSingleNode("CustomCar/Name").InnerText)));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception Error)
            {
                Log_Detail.OpenLog("DISCORD GAME PRESENCE", null, Error, null, true);
            }
            finally
            {
                GETContent = string.Empty;
            }
        }
    }
}
