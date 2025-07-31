using DiscordRPC;
using System;
using System.Collections.Generic;
using System.Linq;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Required.Anti_Cheat;
using System.Text.RegularExpressions;
using DiscordButton = DiscordRPC.Button;
using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Discord.Reference_.List_;
using SBRW.Launcher.Core.Extension.String_;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;

namespace SBRW.Launcher.Core.Discord.RPC_
{
    /// <summary>
    /// Represents the Discord Rich Presence state for the game.
    /// </summary>
    public static class Presence_Game
    {
        private static RichPresence Server_Presence { get; set; } = new RichPresence();
        private static bool CanUpdateProfileField { get; set; }
        private static int EventID { get; set; }
        private static string CarslotsXML { get; set; } = string.Empty;
        private static bool InSafeHouse { get; set; }
        private static string PersonaId { get; set; } = string.Empty;
        private static string PersonaLevel { get; set; } = string.Empty;
        private static string PersonaAvatarId { get; set; } = string.Empty;
        private static string LoggedPersonaId { get; set; } = string.Empty;
        private static string LauncherRPC { get; set; } = "SBRW Launcher: v" + Presence_Settings.Launcher_Version;
        private static int PersonaTreasure { get; set; }
        private static int TotalTreasure { get; set; } = 15;
        private static int THDay { get; set; }
        private static List<string> PersonaIds { get; set; } = new List<string>();
        private static bool _treasureHuntTimerStarted { get; set; }
        private static System.Timers.Timer _treasureHuntTimer;
        /// <summary>
        /// Time in Milliseconds (1 Min.)
        /// </summary>
        private static double TreasureHuntTimerInterval = 60000;
        /* Constants for URIs to improve readability and maintainability - DavidCarbon */
        /// <summary>
        /// Represents in-game SafeHouse catalog Uri string
        /// </summary>
        private const string CatalogUri = "catalog";
        /// <summary>
        /// Represents different states for the User
        /// </summary>
        private static class UserStates
        {
            public const string SecureLoginPersona = "/User/SecureLoginPersona";
            public const string SecureLogoutPersona = "/User/SecureLogoutPersona";
            public const string GetPermanentSession = "/User/GetPermanentSession";
        }
        /// <summary>
        /// Represents different states DriverPersona
        /// </summary>
        private static class DriverPersonaStates
        {
            public const string CreatePersona = "/DriverPersona/CreatePersona";
            public const string GetPersonaInfo = "/DriverPersona/GetPersonaInfo";
            public const string UpdatePersonaPresence = "/DriverPersona/UpdatePersonaPresence";
        }
        /// <summary>
        /// Represents different states for events (In-Game or Freeroam)
        /// </summary>
        private static class EventStates
        {
            public const string Launched = "/event/launched";
            public const string Arbitration = "/event/arbitration";
            public const string GetTreasureHuntEventSession = "/events/gettreasurehunteventsession";
            public const string NotifyCoinCollected = "/events/notifycoincollected";
        }
        /// <summary>
        /// Represents different states for Matchmaking (Before or Triggering an In-Game Event)
        /// </summary>
        private static class MatchmakingStates
        {
            public const string LeaveLobby = "/matchmaking/leavelobby";
            public const string DeclineInvite = "/matchmaking/declineinvite";
            public const string LeaveQueue = "/matchmaking/leavequeue";
            public const string AcceptInvite = "/matchmaking/acceptinvite";
            public const string JoinQueueRaceNow = "/matchmaking/joinqueueracenow";
            public const string LaunchEvent = "/matchmaking/launchevent";
        }
        /// <summary>
        /// Represents different states within the safehouse
        /// </summary>
        private static class SafehouseStates
        {
            public const string Vinyls = "categoryName=NFSW_NA_EP_VINYLS_Category";
            public const string PerformanceParts = "clientProductType=PERFORMANCEPART";
            public const string VisualParts = "clientProductType=VISUALPART";
            public const string Skillmods = "clientProductType=SKILLMODPART";
            public const string CarDealership = "clientProductType=PRESETCAR";
            public static class Paints
            {
                public const string Section = "categoryName=NFSW_NA_EP_PAINTS_";
                //Check the following sections for servers that doesn't have the new parameters 
                public const string Body = "clientProductType=PAINTS_BODY";
                public const string Wheel = "clientProductType=PAINTS_WHEEL";
            }
            public static class BoosterPacks
            {
                public const string v2 = "categoryName=STORE_BOOSTERPACKS";
                //Check the following sections for servers that doesn't have the new parameters 
                public const string v1 = "categoryName=BoosterPacks";
            }
        }
        /// <summary>
        /// Game Status State
        /// </summary>
        /// <param name="uri">String - Address Path</param>
        /// <param name="serverReply">String - XML string File</param>
        /// <param name="getParams">Dynamic - Sub-Path in Address Path</param>
        public static Task State(string uri, string serverReply, dynamic getParams)
        {
            try
            {
                // Process GET parameters
                string queryParams = ProcessGetParams(getParams);

                UpdateDiscordButtons();

                // Use a switch expression or pattern matching for cleaner URI handling
                switch (uri)
                {
                    case UserStates.SecureLoginPersona:
                        LoggedPersonaId = queryParams.Split(';').Last().Split('=').Last();
                        CanUpdateProfileField = true;
                        break;
                    case UserStates.SecureLogoutPersona:
                        ResetPersonaData();
                        break;
                    case UserStates.GetPermanentSession:
                        ParsePermanentSession(serverReply);
                        break;
                    case DriverPersonaStates.CreatePersona:
                        ParseCreatePersona(serverReply);
                        break;
                    case DriverPersonaStates.GetPersonaInfo:
                        if (CanUpdateProfileField && LoggedPersonaId == queryParams.Split(';').Last().Split('=').Last())
                        {
                            ParsePersonaInfo(serverReply);
                        }
                        break;
                    case EventStates.GetTreasureHuntEventSession:
                        ParseTreasureHuntSession(serverReply);
                        break;
                    case EventStates.NotifyCoinCollected:
                        HandleCoinCollection();
                        break;
                    case DriverPersonaStates.UpdatePersonaPresence:
                        HandlePersonaPresenceUpdate(queryParams.Split(';').Last().Split('=').Last());
                        break;
                    case MatchmakingStates.LeaveLobby:
                    case MatchmakingStates.DeclineInvite:
                    case MatchmakingStates.LeaveQueue:
                        HandleLeaveMatchmaking(uri);
                        break;
                    case MatchmakingStates.AcceptInvite:
                        HandleAcceptInvite(serverReply);
                        break;
                    case MatchmakingStates.JoinQueueRaceNow:
                        HandleJoinQueueRaceNow();
                        break;
                }

                if (Regex.Match(uri, MatchmakingStates.LaunchEvent).Success)
                {
                    HandleLaunchEvent(uri.Split('/'));
                }
                if (uri == EventStates.Launched && Launcher_Value.Game_In_Event)
                {
                    HandleEventLaunched();
                }
                if (uri == EventStates.Arbitration)
                {
                    HandleEventArbitration();
                }
                if (uri.Contains(CatalogUri) && InSafeHouse)
                {
                    HandleSafehouseCatalog(queryParams);
                }

                // Handle car-related updates
                foreach (var singlePersonaId in PersonaIds)
                {
                    if (Regex.Match(uri, $"/personas/{singlePersonaId}/carslots", RegexOptions.IgnoreCase).Success)
                    {
                        CarslotsXML = serverReply; // Store the XML for later use if needed
                        UpdateCarNameFromCarSlots(serverReply);
                        break; // No need to check other personas once found
                    }
                    if (Regex.Match(uri, $"/personas/{singlePersonaId}/defaultcar", RegexOptions.IgnoreCase).Success)
                    {
                        var receivedId = uri.Split('/').Last();
                        if (receivedId != "defaultcar")
                        {
                            UpdateDefaultCarName(receivedId);
                            break; // No need to check other personas once found
                        }
                    }
                }
            }
            catch (Exception error)
            {
                Log_Detail.Full("DISCORD GAME PRESENCE", error);
            }

            return Task.CompletedTask;
        }
        /// <summary>
        /// Game Status State as a Task
        /// </summary>
        /// <param name="objectData"><inheritdoc cref="State"/></param>
        /// <returns>Completed Task Regardless if an Error was Encountered or Not</returns>
        public static Task State_Task(object objectData)
        {
            try
            {
                if (objectData is object[] liveData && liveData.Length == 3)
                {
                    State(liveData[0] as string, liveData[1] as string, liveData[2] as dynamic);
                }
                else
                {
                    Log_Detail.Full("DISCORD GAME PRESENCE [Task]", new ArgumentException("Invalid objectData format for State_Task."));
                }
            }
            catch (Exception error)
            {
                Log_Detail.Full("DISCORD GAME PRESENCE [Task]", error);
            }

            return Task.CompletedTask;
        }
        /// <summary>
        /// Game Status State
        /// </summary>
        /// <param name="uri">Address Path</param>
        /// <param name="serverReply">XML string File</param>
        /// <param name="get">Sub-Path in Address Path</param>
        public static async Task State_Async(string uri, string serverReply, dynamic get)
        {
            try
            {
                await Task.Run(() => State(uri, serverReply, get)).ConfigureAwait(false);
            }
            catch (Exception error)
            {
                Log_Detail.Full("DISCORD GAME PRESENCE [Async]", error);
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// Processes dynamic GET parameters into a dictionary.
        /// </summary>
        /// <param name="getParams">Dynamic GET parameters.</param>
        /// <returns>A dictionary of query parameters.</returns>
        private static string ProcessGetParams(dynamic getParams)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            foreach (dynamic param in getParams)
            {
                parameters[param] = getParams[param];
            }

            return string.Join(";", parameters.Select(x => x.Key + "=" + x.Value).ToArray());
        }
        /// <summary>
        /// Updates the Discord buttons based on server links.
        /// </summary>
        private static void UpdateDiscordButtons()
        {
            Presence_Launcher.ButtonsList.Clear();

            string serverPanelLink = Launcher_Value.Launcher_Select_Server_JSON.Server_Panel ?? string.Empty;
            string serverWebsiteLink = Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home ?? string.Empty;
            string serverDiscordLink = Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Discord ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(serverPanelLink) && !string.IsNullOrEmpty(Launcher_Value.Game_Persona_Name))
            {
                serverPanelLink = serverPanelLink.Replace("{personaname}", Launcher_Value.Game_Persona_Name);
                Presence_Launcher.ButtonsList.Add(new DiscordButton()
                {
                    Label = $"Check {Launcher_Value.Game_Persona_Name} on Panel",
                    Url = serverPanelLink
                });
            }
            else if (!string.IsNullOrWhiteSpace(serverWebsiteLink) && serverWebsiteLink != serverDiscordLink)
            {
                Presence_Launcher.ButtonsList.Add(new DiscordButton()
                {
                    Label = "Website",
                    Url = serverWebsiteLink
                });
            }

            if (!string.IsNullOrWhiteSpace(serverDiscordLink))
            {
                Presence_Launcher.ButtonsList.Add(new DiscordButton()
                {
                    Label = "Discord",
                    Url = serverDiscordLink
                });
            }
        }
        /// <summary>
        /// Resets persona-related data.
        /// </summary>
        private static void ResetPersonaData()
        {
            PersonaId = string.Empty;
            Launcher_Value.Game_Persona_Name_Live = string.Empty;
            PersonaLevel = string.Empty;
            PersonaAvatarId = string.Empty;
            Launcher_Value.Game_Car_Name = string.Empty;
            PersonaTreasure = 0;
            PersonaIds.Clear(); // Clear the list of persona IDs
            CanUpdateProfileField = false; // Reset this flag as well
        }
        /// <summary>
        /// Parses the XML reply for GetPermanentSession and updates persona data.
        /// </summary>
        /// <param name="serverReply">The XML string from the server.</param>
        private static void ParsePermanentSession(string serverReply)
        {
            try
            {
                var xml = XDocument.Parse(serverReply);
                var userInfo = xml.Element("UserInfo");

                if (userInfo == null) return;

                var profileData = userInfo.Element("personas")?.Element("ProfileData");
                if (profileData != null)
                {
                    Launcher_Value.Game_Persona_Name_Live = profileData.Element("Name")?.Value.Replace("¤", "[S]");
                    PersonaLevel = profileData.Element("Level")?.Value;
                    PersonaAvatarId = "avatar_" + profileData.Element("IconIndex")?.Value;
                    PersonaId = profileData.Element("PersonaId")?.Value;
                }

                PersonaIds.Clear(); // Clear previous IDs
                foreach (var personaNode in userInfo.Elements("personas").Elements("ProfileData"))
                {
                    PersonaIds.Add(personaNode.Element("PersonaId")?.Value);
                }
            }
            catch (Exception ex)
            {
                Log_Detail.Full("DISCORD GAME PRESENCE [ParsePermanentSession]", ex);
            }
        }
        /// <summary>
        /// Parses the XML reply for CreatePersona and adds the new persona ID.
        /// </summary>
        /// <param name="serverReply">The XML string from the server.</param>
        private static void ParseCreatePersona(string serverReply)
        {
            try
            {
                var xml = XDocument.Parse(serverReply);
                var personaId = xml.Element("ProfileData")?.Element("PersonaId")?.Value;
                if (!string.IsNullOrEmpty(personaId))
                {
                    PersonaIds.Add(personaId);
                }
            }
            catch (Exception ex)
            {
                Log_Detail.Full("DISCORD GAME PRESENCE [ParseCreatePersona]", ex);
            }
        }
        /// <summary>
        /// Parses the XML reply for GetPersonaInfo and updates persona data.
        /// </summary>
        /// <param name="serverReply">The XML string from the server.</param>
        private static void ParsePersonaInfo(string serverReply)
        {
            try
            {
                var xml = XDocument.Parse(serverReply);
                var profileData = xml.Element("ProfileData");
                if (profileData != null)
                {
                    Launcher_Value.Game_Persona_Name_Live = profileData.Element("Name")?.Value.Replace("¤", "[S]");
                    PersonaLevel = profileData.Element("Level")?.Value;
                    PersonaAvatarId = "avatar_" + profileData.Element("IconIndex")?.Value;
                    PersonaId = profileData.Element("PersonaId")?.Value;

                    Launcher_Value.Game_Persona_ID = PersonaId;
                    Launcher_Value.Game_Persona_Name = Launcher_Value.Game_Persona_Name_Live;
                }
            }
            catch (Exception ex)
            {
                Log_Detail.Full("DISCORD GAME PRESENCE [ParsePersonaInfo]", ex);
            }
        }
        /// <summary>
        /// Parses the XML reply for Treasure Hunt event session.
        /// </summary>
        /// <param name="serverReply">The XML string from the server.</param>
        private static void ParseTreasureHuntSession(string serverReply)
        {
            try
            {
                var xml = XDocument.Parse(serverReply);
                var session = xml.Element("TreasureHuntEventSession");
                if (session != null)
                {
                    PersonaTreasure = 0; // Reset for calculation

                    int xPersonaTreasure = Convert.ToInt32(session.Element("CoinsCollected")?.Value);
                    for (int i = 0; i < 15; i++)
                    {
                        if ((xPersonaTreasure & (1 << (15 - i))) != 0)
                        {
                            PersonaTreasure++;
                        }
                    }

                    TotalTreasure = Convert.ToInt32(session.Element("NumCoins")?.Value);
                    THDay = Convert.ToInt32(session.Element("Streak")?.Value);
                }
            }
            catch (Exception ex)
            {
                Log_Detail.Full("DISCORD GAME PRESENCE [ParseTreasureHuntSession]", ex);
            }
        }
        /// <summary>
        /// Handles updates when a coin is collected in Treasure Hunt.
        /// </summary>
        private static void HandleCoinCollection()
        {
            Launcher_Value.Game_In_Event = false;
            PersonaTreasure++;

            Server_Presence.Details = PersonaTreasure < TotalTreasure
                ? $"Collecting Gems ({PersonaTreasure} of {TotalTreasure})"
                : PersonaTreasure == TotalTreasure
                    ? $"Finished Collecting Gems ({PersonaTreasure} of {TotalTreasure})"
                    : "Finished Collecting Gems";

            UpdateAndSetPresence(
                state: LauncherRPC,
                largeImageText: $"{Launcher_Value.Game_Persona_Name_Live} - Level: {PersonaLevel}",
                largeImageKey: PersonaAvatarId,
                smallImageText: $"Treasure Hunt - Day: {THDay}",
                smallImageKey: "gamemode_treasure"
            );

            Treasure_Hunt_Start();
        }
        /// <summary>
        /// Handles updates for persona presence (safehouse/freeroam).
        /// </summary>
        /// <param name="presenceParam">The presence parameter (e.g., "1" for freeroam).</param>
        private static void HandlePersonaPresenceUpdate(string presenceParam)
        {
            Server_Presence.Assets = new Assets(); // Re-initialize assets for clarity

            string details;
            string smallImageText;
            string smallImageKey;
            string state;

            if (presenceParam == "1")
            {
                details = $"Driving {Launcher_Value.Game_Car_Name}";
                smallImageText = "In-Freeroam";
                smallImageKey = "gamemode_freeroam";
                state = LauncherRPC;
                Launcher_Value.Game_In_Event = false;
                InSafeHouse = false;
            }
            else
            {
                details = "In Safehouse";
                smallImageText = "In-Safehouse";
                smallImageKey = "gamemode_safehouse";
                state = Launcher_Value.Game_Server_Name;
                Launcher_Value.Game_In_Event = true;
                InSafeHouse = true;
            }

            Server_Presence.Details = details;

            UpdateAndSetPresence(
                state: state,
                largeImageText: $"{Launcher_Value.Game_Persona_Name_Live} - Level: {PersonaLevel}",
                largeImageKey: PersonaAvatarId,
                smallImageText: smallImageText,
                smallImageKey: smallImageKey
            );
        }
        /// <summary>
        /// Handles updates when leaving matchmaking or declining an invite.
        /// </summary>
        /// <param name="uri">The current URI.</param>
        private static void HandleLeaveMatchmaking(string uri)
        {
            Server_Presence.Details = $"Driving {Launcher_Value.Game_Car_Name}";
            Server_Presence.State = LauncherRPC;

            UpdateAndSetPresence(
                state: LauncherRPC,
                largeImageText: $"{Launcher_Value.Game_Persona_Name_Live} - Level: {PersonaLevel}",
                largeImageKey: PersonaAvatarId,
                smallImageText: "In-Freeroam",
                smallImageKey: "gamemode_freeroam"
            );

            if (uri == MatchmakingStates.LeaveLobby)
            {
                AC_Core.Stop(false);
            }

            Launcher_Value.Game_In_Event = false;
        }
        /// <summary>
        /// Handles updates when accepting a matchmaking invite.
        /// </summary>
        /// <param name="serverReply">The XML string from the server.</param>
        private static void HandleAcceptInvite(string serverReply)
        {
            Launcher_Value.Game_In_Event = true;
            try
            {
                var xml = XDocument.Parse(serverReply);
                var eventIdNode = xml.Element("LobbyInfo")?.Element("EventId");

                if (eventIdNode != null && int.TryParse(eventIdNode.Value, out int parsedEventId))
                {
                    EventID = parsedEventId;

                    Server_Presence.Details = $"In Lobby: {EventID.Get_Name_Event()}";
                    Server_Presence.State = Launcher_Value.Game_Server_Name;

                    UpdateAndSetPresence(
                        state: Launcher_Value.Game_Server_Name,
                        largeImageText: $"{Launcher_Value.Game_Persona_Name_Live} - Level: {PersonaLevel}",
                        largeImageKey: PersonaAvatarId,
                        smallImageText: LauncherRPC,
                        smallImageKey: EventID.Get_Type_Event()
                    );
                }
            }
            catch (Exception ex)
            {
                Log_Detail.Full("DISCORD GAME PRESENCE [HandleAcceptInvite]", ex);
            }
        }
        /// <summary>
        /// Handles updates when joining a queue for a race.
        /// </summary>
        private static void HandleJoinQueueRaceNow()
        {
            Launcher_Value.Game_In_Event = false;

            Server_Presence.Details = "Searching for Event";
            Server_Presence.State = LauncherRPC;

            UpdateAndSetPresence(
                state: LauncherRPC,
                largeImageText: $"{Launcher_Value.Game_Persona_Name_Live} - Level: {PersonaLevel}",
                largeImageKey: PersonaAvatarId,
                smallImageText: "In-Freeroam",
                smallImageKey: "gamemode_freeroam"
            );
        }
        /// <summary>
        /// Handles updates when launching an event.
        /// </summary>
        /// <param name="splittedUri">The URI split into segments.</param>
        private static void HandleLaunchEvent(string[] splittedUri)
        {
            Launcher_Value.Game_In_Event = true;
            EventID = Convert.ToInt32(splittedUri[3]); // Assumes event ID is always at index 3

            Server_Presence.Details = $"Loading Event: {EventID.Get_Name_Event()}";
            Server_Presence.State = Launcher_Value.Game_Server_Name;

            UpdateAndSetPresence(
                state: Launcher_Value.Game_Server_Name,
                largeImageText: $"{Launcher_Value.Game_Persona_Name_Live} - Level: {PersonaLevel}",
                largeImageKey: PersonaAvatarId,
                smallImageText: LauncherRPC,
                smallImageKey: EventID.Get_Type_Event()
            );
        }
        /// <summary>
        /// Handles updates when an event has launched.
        /// </summary>
        private static void HandleEventLaunched()
        {
            Server_Presence.Details = $"In Event: {EventID.Get_Name_Event()}";
            Server_Presence.State = Launcher_Value.Game_Server_Name;

            UpdateAndSetPresence(
                state: Launcher_Value.Game_Server_Name,
                largeImageText: $"{Launcher_Value.Game_Persona_Name_Live} - Level: {PersonaLevel}",
                largeImageKey: PersonaAvatarId,
                smallImageText: LauncherRPC,
                smallImageKey: EventID.Get_Type_Event()
            );

            AC_Core.Start(Launcher_Value.Launcher_Select_Server_JSON.Server_Enable_Crew_Tags, true, 0, EventID);
        }
        /// <summary>
        /// Handles updates when an event has finished.
        /// </summary>
        private static void HandleEventArbitration()
        {
            Server_Presence.Details = $"Finished Event: {EventID.Get_Name_Event()}";
            Server_Presence.State = Launcher_Value.Game_Server_Name;

            UpdateAndSetPresence(
                state: Launcher_Value.Game_Server_Name,
                largeImageText: $"{Launcher_Value.Game_Persona_Name_Live} - Level: {PersonaLevel}",
                largeImageKey: PersonaAvatarId,
                smallImageText: LauncherRPC,
                smallImageKey: EventID.Get_Type_Event()
            );
            
            AC_Core.Stop(true);
            Launcher_Value.Game_In_Event = true;
        }
        /// <summary>
        /// Handles updates for different states within the safehouse catalog.
        /// </summary>
        /// <param name="getParamContent">The GET parameter content.</param>
        private static void HandleSafehouseCatalog(string getParamContent)
        {
            if (getParamContent.Contains(SafehouseStates.Vinyls))
            {
                Server_Presence.Details = "In Safehouse - Applying Vinyls";
            }
            else if (getParamContent.Contains(SafehouseStates.Paints.Section) ||
                getParamContent.Contains(SafehouseStates.Paints.Body) ||
                getParamContent.Contains(SafehouseStates.Paints.Wheel))
            {
                Server_Presence.Details = "In Safehouse - Applying Paint Colors";
            }
            else if (getParamContent.Contains(SafehouseStates.PerformanceParts))
            {
                Server_Presence.Details = "In Safehouse - Applying Performance Parts";
            }
            else if (getParamContent.Contains(SafehouseStates.VisualParts))
            {
                Server_Presence.Details = "In Safehouse - Applying Visual Parts";
            }
            else if (getParamContent.Contains(SafehouseStates.Skillmods))
            {
                Server_Presence.Details = "In Safehouse - Applying Skillmods";
            }
            else if (getParamContent.Contains(SafehouseStates.CarDealership))
            {
                Server_Presence.Details = "In Safehouse - Car Dealership";
            }
            else if (getParamContent.Contains(SafehouseStates.BoosterPacks.v2) ||
                getParamContent.Contains(SafehouseStates.BoosterPacks.v1))
            {
                Server_Presence.Details = "In Safehouse - Opening Cardpacks";
            }
            else
            {
                Server_Presence.Details = "In Safehouse - Idle";
            }

            UpdateAndSetPresence(
                state: Launcher_Value.Game_Server_Name,
                largeImageText: $"{Launcher_Value.Game_Persona_Name_Live} - Level: {PersonaLevel}",
                largeImageKey: PersonaAvatarId,
                smallImageText: "In-Safehouse",
                smallImageKey: "gamemode_safehouse"
            );
        }
        /// <summary>
        /// Updates the car name from the provided car slots XML.
        /// </summary>
        /// <param name="serverReply">The car slots XML string.</param>
        private static void UpdateCarNameFromCarSlots(string serverReply)
        {
            try
            {
                var xml = XDocument.Parse(serverReply);
                var carSlotInfo = xml.Element("CarSlotInfoTrans");
                if (carSlotInfo != null)
                {
                    int defaultId = Convert.ToInt32(carSlotInfo.Element("DefaultOwnedCarIndex")?.Value);
                    int current = 0;

                    foreach (var ownedCar in carSlotInfo.Element("CarsOwnedByPersona")?.Elements("OwnedCarTrans") ?? Enumerable.Empty<XElement>())
                    {
                        if (defaultId == current)
                        {
                            Launcher_Value.Game_Car_Name = ownedCar.Element("CustomCar")?.Element("Name")?.Value.Encode_UTF8().Get_Name_Car();
                            break;
                        }
                        current++;
                    }
                }
            }
            catch (Exception ex)
            {
                Log_Detail.Full("DISCORD GAME PRESENCE [UpdateCarNameFromCarSlots]", ex);
            }
        }
        /// <summary>
        /// Updates the default car name based on a received car ID.
        /// </summary>
        /// <param name="receivedId">The ID of the default car.</param>
        private static void UpdateDefaultCarName(string receivedId)
        {
            if (string.IsNullOrEmpty(CarslotsXML))
            {
                Log_Detail.Full("DISCORD GAME PRESENCE [UpdateDefaultCarName]", new InvalidOperationException("CarslotsXML is empty. Cannot update default car name."));
                return;
            }

            try
            {
                var xml = XDocument.Parse(CarslotsXML);
                var carSlotInfo = xml.Element("CarSlotInfoTrans");
                if (carSlotInfo != null)
                {
                    foreach (var ownedCar in carSlotInfo.Element("CarsOwnedByPersona")?.Elements("OwnedCarTrans") ?? Enumerable.Empty<XElement>())
                    {
                        if (receivedId == ownedCar.Element("Id")?.Value)
                        {
                            Launcher_Value.Game_Car_Name = ownedCar.Element("CustomCar")?.Element("Name")?.Value.Encode_UTF8().Get_Name_Car();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log_Detail.Full("DISCORD GAME PRESENCE [UpdateDefaultCarName]", ex);
            }
        }
        /// <summary>
        /// Centralized method to update and set Discord Presence.
        /// </summary>
        /// <param name="state">The state text.</param>
        /// <param name="largeImageText">Text for the large image.</param>
        /// <param name="largeImageKey">Key for the large image.</param>
        /// <param name="smallImageText">Text for the small image (optional).</param>
        /// <param name="smallImageKey">Key for the small image (optional).</param>
        private static void UpdateAndSetPresence(string state, string largeImageText, string largeImageKey, string smallImageText = null, string smallImageKey = null)
        {
            Server_Presence.State = state;
            Server_Presence.Assets = new Assets
            {
                LargeImageText = largeImageText,
                LargeImageKey = largeImageKey,
                SmallImageText = smallImageText,
                SmallImageKey = smallImageKey
            };

            if (Presence_Launcher.ButtonsList.Count > 0)
            {
                Server_Presence.Buttons = Presence_Launcher.ButtonsList.ToArray();
            }

            if (Presence_Launcher.Running())
            {
                Presence_Launcher.Client.SetPresence(Server_Presence);
                Presence_Launcher.User_Details();
            }
        }
        /// <summary>
        /// Treasure Hunt elapsed timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnTreasureHuntTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                // Only update if not in an event.
                if (!Launcher_Value.Game_In_Event)
                {
                    Server_Presence.Details = "Driving " + Launcher_Value.Game_Car_Name;
                    UpdateAndSetPresence(
                        state: LauncherRPC,
                        largeImageText: $"{Launcher_Value.Game_Persona_Name_Live} - Level: {PersonaLevel}",
                        largeImageKey: PersonaAvatarId,
                        smallImageText: "In-Freeroam",
                        smallImageKey: "gamemode_freeroam"
                    );
                }
                else
                {
                    Log.Debug("INCORRECT IN_GAME_EVENT STATUS");
                }
            }
            catch (Exception error)
            {
                Log_Detail.Full("DISCORD GAME PRESENCE [TIMER Elapsed]", error);
            }
            finally
            {
                Treasure_Hunt_Stop();
            }
        }
        /// <summary>
        /// Stops the Treasure Hunt timer.
        /// </summary>
        private static void Treasure_Hunt_Stop()
        {
            try
            {
                if (_treasureHuntTimerStarted)
                {
                    _treasureHuntTimer.Stop();
                    _treasureHuntTimer.Dispose();
                    _treasureHuntTimerStarted = false;
                }
            }
            catch (Exception error)
            {
                Log_Detail.Full("DISCORD GAME PRESENCE [TIMER STOP]", error);
            }
        }
        /// <summary>
        /// Starts or restarts the Treasure Hunt timer.
        /// </summary>
        private static void Treasure_Hunt_Start()
        {
            try
            {
                if (_treasureHuntTimerStarted)
                {
                    Treasure_Hunt_Stop();
                }

                _treasureHuntTimerStarted = true;
                _treasureHuntTimer = new System.Timers.Timer(TreasureHuntTimerInterval);
                _treasureHuntTimer.Elapsed += OnTreasureHuntTimerElapsed;
                _treasureHuntTimer.AutoReset = false;
                _treasureHuntTimer.Enabled = true;
            }
            catch (Exception error)
            {
                Log_Detail.Full("DISCORD GAME PRESENCE [TIMER START]", error);
            }
        }

        #endregion
    }
}