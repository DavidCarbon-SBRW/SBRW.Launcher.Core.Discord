using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordRPC;
using DiscordRPC.Logging;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Extension.Security_;
using SBRW.Launcher.Core.Required.Certificate;
using DiscordButton = DiscordRPC.Button;

namespace SBRW.Launcher.Core.Discord.RPC_
{
    /// <summary>
    /// Discord RPC Set from Launcher Side
    /// </summary>
    public static class Presence_Launcher
    {
        /// <summary>
        /// Launcher's Discord RPC Client
        /// </summary>
        /// <remarks>Discord RPC Client</remarks>
        public static DiscordRpcClient Client { get; set; }

        /// <summary>
        /// Boolean Value on If RPC is Running
        /// </summary>
        /// <returns>True or False</returns>
        public static bool Running() => Client != null;

        /// <summary>
        /// Launcher's Discord Presence To Show Statuss
        /// </summary>
        /// <remarks>Instance of Discord Presence</remarks>
        public static RichPresence Presence { get; set; } = new RichPresence();

        /// <summary>
        /// Used to Set Discord Buttons on RPC Status
        /// </summary>
        /// <remarks>Instance of Discord Buttons</remarks>
        public static List<DiscordButton> ButtonsList { get; set; } = new List<DiscordButton>();

        /// <summary>
        /// Used to prevent Displaying RPC when there is an Error (Displays a Simple Error Message in RPC)
        /// </summary>
        /// <remarks>Displays Launcher Errors in RPC</remarks>
        public static bool Download { get; set; } = true;

        /// <summary>
        /// Used to programmatically manually invoke Discord RPC
        /// </summary>
        public static bool Invoked { get; set; }

        /// <summary>
        /// Used to Retrive User Details from Discord Client
        /// </summary>
        public static void User_Details()
        {
            try
            {
                if (!Invoked && Client.CurrentUser != null)
                {
                    Invoked = true;
                    Update();

                    if (Launcher_Value.Launcher_Discord_UserID != Client.CurrentUser.ID.ToString())
                    {
                        Launcher_Value.Launcher_Discord_UserID = Client.CurrentUser.ID.ToString();
                    }
                }
            }
            catch (Exception Error)
            {
                Log_Detail.Full("DISCORD [User Details]", Error);
            }
        }

        /// <summary>
        /// Sets the current Status of the Launcher's State<br></br>
        /// </summary>
        /// <remarks>RPC Status<br></br></remarks>
        /// <param name="RPC_State">
        /// Int - Which RPC Status Text to Set
        /// <br></br><br></br>
        /// <remarks>
        /// 0 - "Start Up"<br></br>
        /// 1 - "Unpack Game Files"<br></br>
        /// 2 - "Download Game Files"<br></br>
        /// 3 - "Download Game Files Error"<br></br>
        /// 4 - "Idle Ready"<br></br>
        /// 5 - "Checking ModNet"<br></br>
        /// 6 - "ModNet File Check Passed"<br></br>
        /// 7 - "Download ModNet"<br></br>
        /// 8 - "Download ModNet Error"<br></br>
        /// 9 - "Download Server Mods"<br></br>
        /// 10 - "Download Server Mods Error"<br></br>
        /// <br></br>
        /// 20 - "Security Center"<br></br>
        /// 21 - "Register"<br></br>
        /// 22 - "Settings"<br></br>
        /// 23 - "User XML Editor"<br></br>
        /// 24 - "Verify"<br></br>
        /// 25 - "Verify Scan"<br></br>
        /// 26 - "Verify Bad"<br></br>
        /// 27 - "Verify Good"<br></br>
        /// 28 - "In-Game"<br></br>
        /// </remarks>
        /// </param>
        public static void Status(int RPC_State)
        {
            Status(RPC_State, string.Empty, false);
        }
        /// <summary>
        /// Sets the current Status of the Launcher's State<br></br>
        /// </summary>
        /// <remarks>RPC Status<br></br></remarks>
        /// <param name="RPC_State">
        /// Int - Which RPC Status Text to Set
        /// <br></br><br></br>
        /// <remarks>
        /// 0 - "Start Up"<br></br>
        /// 1 - "Unpack Game Files"<br></br>
        /// 2 - "Download Game Files"<br></br>
        /// 3 - "Download Game Files Error"<br></br>
        /// 4 - "Idle Ready"<br></br>
        /// 5 - "Checking ModNet"<br></br>
        /// 6 - "ModNet File Check Passed"<br></br>
        /// 7 - "Download ModNet"<br></br>
        /// 8 - "Download ModNet Error"<br></br>
        /// 9 - "Download Server Mods"<br></br>
        /// 10 - "Download Server Mods Error"<br></br>
        /// <br></br>
        /// 20 - "Security Center"<br></br>
        /// 21 - "Register"<br></br>
        /// 22 - "Settings"<br></br>
        /// 23 - "User XML Editor"<br></br>
        /// 24 - "Verify"<br></br>
        /// 25 - "Verify Scan"<br></br>
        /// 26 - "Verify Bad"<br></br>
        /// 27 - "Verify Good"<br></br>
        /// 28 - "In-Game"<br></br>
        /// </remarks>
        /// </param>
        /// <param name="RPC_Status">String - Additional RPC Status Details to Display<br></br></param> 
        public static void Status(int RPC_State, string RPC_Status)
        {
            Status(RPC_State, RPC_Status, false);
        }
        /// <summary>
        /// Sets the current Status of the Launcher's State<br></br>
        /// </summary>
        /// <remarks>RPC Status<br></br></remarks>
        /// <param name="RPC_State">
        /// Int - Which RPC Status Text to Set
        /// <br></br><br></br>
        /// <remarks>
        /// 0 - "Start Up"<br></br>
        /// 1 - "Unpack Game Files"<br></br>
        /// 2 - "Download Game Files"<br></br>
        /// 3 - "Download Game Files Error"<br></br>
        /// 4 - "Idle Ready"<br></br>
        /// 5 - "Checking ModNet"<br></br>
        /// 6 - "ModNet File Check Passed"<br></br>
        /// 7 - "Download ModNet"<br></br>
        /// 8 - "Download ModNet Error"<br></br>
        /// 9 - "Download Server Mods"<br></br>
        /// 10 - "Download Server Mods Error"<br></br>
        /// <br></br>
        /// 20 - "Security Center"<br></br>
        /// 21 - "Register"<br></br>
        /// 22 - "Settings"<br></br>
        /// 23 - "User XML Editor"<br></br>
        /// 24 - "Verify"<br></br>
        /// 25 - "Verify Scan"<br></br>
        /// 26 - "Verify Bad"<br></br>
        /// 27 - "Verify Good"<br></br>
        /// 28 - "In-Game"<br></br>
        /// </remarks>
        /// </param>
        /// <param name="RPC_Status">String - Additional RPC Status Details to Display<br></br></param>
        /// <param name="RPC_Beta">Bool - Displays a Different Icon for Beta Launcher Builds<br></br></param> 
        public static void Status(int RPC_State, string RPC_Status, bool RPC_Beta)
        {
            try
            {
                ButtonsList.Clear();
                ButtonsList.Add(new DiscordButton()
                {
                    Label = "Project Site",
                    Url = "https://soapboxrace.world"
                });
                ButtonsList.Add(new DiscordButton()
                {
                    Label = "Launcher Patch Notes",
                    Url = "https://github.com/SoapboxRaceWorld/GameLauncher_NFSW/releases/tag/" + Presence_Settings.Launcher_Version
                });
                Presence.Buttons = ButtonsList.ToArray();

                switch (RPC_State)
                {
                    case 0:
                        Presence.State = RPC_Status;
                        Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                        Presence.Assets = new Assets
                        {
                            LargeImageText = "Launcher",
                            LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw"
                        };
                        break;
                    case 1:
                        Download = true;
                        Presence.State = RPC_Status;
                        Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                        Presence.Assets = new Assets
                        {
                            LargeImageText = "Launcher",
                            LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw",
                            SmallImageText = string.Empty,
                            SmallImageKey = "files_success"
                        };
                        Presence.Buttons = ButtonsList.ToArray();
                        break;
                    case 2:
                        Download = true;
                        Presence.State = RPC_Status;
                        Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                        Presence.Assets = new Assets
                        {
                            LargeImageText = "Launcher",
                            LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw",
                            SmallImageText = string.Empty,
                            SmallImageKey = "files"
                        };
                        Presence.Buttons = ButtonsList.ToArray();
                        break;
                    case 3:
                        Download = true;
                        Presence.State = "Game Download Error";
                        Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                        Presence.Assets = new Assets
                        {
                            LargeImageText = "Launcher",
                            LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw",
                            SmallImageText = string.Empty,
                            SmallImageKey = "files_error"
                        };
                        Presence.Buttons = ButtonsList.ToArray();
                        break;
                    case 4:
                        Presence.State = "Ready To Race";
                        Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                        Presence.Assets = new Assets
                        {
                            LargeImageText = "Launcher",
                            LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw",
                            SmallImageText = string.Empty,
                            SmallImageKey = Certificate_Signature_Validation.Signed() ? "official" : "unofficial"
                        };
                        Presence.Buttons = ButtonsList.ToArray();
                        break;
                    case 5:
                        Presence.State = "Checking ModNet";
                        Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                        Presence.Assets = new Assets
                        {
                            LargeImageText = "Launcher",
                            LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw",
                            SmallImageText = string.Empty,
                            SmallImageKey = "files_alert"
                        };
                        Presence.Buttons = ButtonsList.ToArray();
                        break;
                    case 6:
                        Presence.State = "Has ModNet File: " + RPC_Status;
                        Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                        Presence.Assets = new Assets
                        {
                            LargeImageText = "Launcher",
                            LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw",
                            SmallImageText = string.Empty,
                            SmallImageKey = "files_success"
                        };
                        Presence.Buttons = ButtonsList.ToArray();
                        break;
                    case 7:
                        Presence.State = "Downloading ModNet Files";
                        Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                        Presence.Assets = new Assets
                        {
                            LargeImageText = "Launcher",
                            LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw",
                            SmallImageText = string.Empty,
                            SmallImageKey = "files"
                        };
                        Presence.Buttons = ButtonsList.ToArray();
                        break;
                    case 8:
                        Presence.State = "ModNet Encounterd an Error";
                        Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                        Presence.Assets = new Assets
                        {
                            LargeImageText = "Launcher",
                            LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw",
                            SmallImageText = string.Empty,
                            SmallImageKey = "files_error"
                        };
                        Presence.Buttons = ButtonsList.ToArray();
                        break;
                    case 9:
                        Presence.State = "Downloading Server Mods";
                        Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                        Presence.Assets = new Assets
                        {
                            LargeImageText = "Launcher",
                            LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw",
                            SmallImageText = string.Empty,
                            SmallImageKey = "files"
                        };
                        Presence.Buttons = ButtonsList.ToArray();
                        break;
                    case 10:
                        Presence.State = "Server Mod Download Error";
                        Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                        Presence.Assets = new Assets
                        {
                            LargeImageText = "Launcher",
                            LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw",
                            SmallImageText = string.Empty,
                            SmallImageKey = "files_error"
                        };
                        Presence.Buttons = ButtonsList.ToArray();
                        break;
                    case 20:
                        if (!Download)
                        {
                            Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                            Presence.State = "On Security Center Screen";
                            Presence.Assets = new Assets
                            {
                                LargeImageText = "Launcher",
                                LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw",
                                SmallImageText = Security_Center_Conversion.RPC(Launcher_Value.Launcher_Security_Center_Codes, false),
                                SmallImageKey = Security_Center_Conversion.RPC(Launcher_Value.Launcher_Security_Center_Codes)
                            };
                        }
                        break;
                    case 21:
                        if (!Download)
                        {
                            Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                            Presence.State = "On Registration Screen";
                            Presence.Assets = new Assets
                            {
                                LargeImageText = "Launcher",
                                LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw",
                                SmallImageText = string.Empty,
                                SmallImageKey = "screen_register"
                            };
                        }
                        break;
                    case 22:
                        if (!Download)
                        {
                            Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                            Presence.State = "On Settings Screen";
                            Presence.Assets = new Assets
                            {
                                LargeImageText = "Launcher",
                                LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw",
                                SmallImageText = string.Empty,
                                SmallImageKey = "screen_settings"
                            };
                        }
                        break;
                    case 23:
                        if (!Download)
                        {
                            Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                            Presence.State = "On User XML Editor Screen";
                            Presence.Assets = new Assets
                            {
                                LargeImageText = "Launcher",
                                LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw",
                                SmallImageText = string.Empty,
                                SmallImageKey = "screen_uxe"
                            };
                        }
                        break;
                    case 24:
                        if (!Download)
                        {
                            Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                            Presence.State = "On Verify Game Files Screen";
                            Presence.Assets = new Assets
                            {
                                LargeImageText = "Launcher",
                                LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw",
                                SmallImageText = string.Empty,
                                SmallImageKey = "screen_verify"
                            };
                        }
                        break;
                    case 25:
                        if (!Download)
                        {
                            Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                            Presence.State = "Verifying Game Files";
                            Presence.Assets = new Assets
                            {
                                LargeImageText = "Launcher",
                                LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw",
                                SmallImageText = string.Empty,
                                SmallImageKey = "verify_files_scan"
                            };
                        }
                        break;
                    case 26:
                        if (!Download)
                        {
                            Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                            Presence.State = "Downloaded " + RPC_Status + " Missing Game Files";
                            Presence.Assets = new Assets
                            {
                                LargeImageText = "Launcher",
                                LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw",
                                SmallImageText = string.Empty,
                                SmallImageKey = "verify_files_bad"
                            };
                        }
                        break;
                    case 27:
                        if (!Download)
                        {
                            Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                            Presence.State = "Finished Validating Game Files";
                            Presence.Assets = new Assets
                            {
                                LargeImageText = "Launcher",
                                LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw",
                                SmallImageText = string.Empty,
                                SmallImageKey = "verify_files_good"
                            };
                        }
                        break;
                    case 28:
                        if (!Download)
                        {
                            Presence.State = Launcher_Value.Game_Server_Name;
                            Presence.Details = "In-Game";
                            Presence.Assets = new Assets
                            {
                                LargeImageText = "Need for Speed: World",
                                LargeImageKey = "nfsw",
                                SmallImageText = string.Empty,
                                SmallImageKey = "ingame"
                            };

                            if (!string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home ?? string.Empty) ||
                                !string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Discord ?? string.Empty) ||
                                !string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Panel ?? string.Empty))
                            {
                                ButtonsList.Clear();

                                if (!string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Panel))
                                {
                                    /* Let's format it now, if possible */
                                    ButtonsList.Add(new DiscordButton()
                                    {
                                        Label = "View Panel",
                                        Url = Launcher_Value.Launcher_Select_Server_JSON.Server_Panel.Split(new string[] { "{sep}" }, StringSplitOptions.None)[0]
                                    });
                                }
                                else if (!string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home) &&
                                    Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home != Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Discord)
                                {
                                    ButtonsList.Add(new DiscordButton()
                                    {
                                        Label = "Website",
                                        Url = Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home
                                    });
                                }

                                if (!string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Discord))
                                {
                                    ButtonsList.Add(new DiscordButton()
                                    {
                                        Label = "Discord",
                                        Url = Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Discord
                                    });
                                }
                            }

                            Presence.Buttons = ButtonsList.ToArray();
                        }
                        break;
                    default:
                        Presence.State = "Unknown Status";
                        Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                        Presence.Assets = new Assets
                        {
                            LargeImageText = "Launcher",
                            LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw"
                        };
                        break;
                }

                if (Running() && Launcher_Value.Launcher_Select_Server_Category != "DEV")
                {
                    Client.SetPresence(Presence);
                    User_Details();
                }
            }
            catch (Exception Error)
            {
                Log_Detail.Full("DISCORD LAUNCHER PRESENCE", Error);
            }
        }

        /// <summary>
        /// Sets the current Status of the Launcher's RPC_State as a Task
        /// </summary>
        /// <param name="Object_Data"><inheritdoc cref="Status(int)"/></param>
        /// <returns>Completed Task Regardless if an Error was Encountered or Not</returns>
        public static Task Status_Task(object Object_Data)
        {
            try
            {
                object[] Live_Data = Object_Data as object[];
                if (Live_Data.Length > 2)
                {
                    if (bool.TryParse(Live_Data[2] as string, out bool Object_Bool))
                    {
                        int.TryParse(Live_Data[0].ToString(), out int Converted_Int);
                        Status(Converted_Int, Live_Data[1] as string, Object_Bool);
                    }
                    else
                    {
                        int.TryParse(Live_Data[0].ToString(), out int Converted_Int);
                        Status(Converted_Int, Live_Data[1] as string);
                    }
                }
                else
                {
                    int.TryParse(Live_Data[0].ToString(), out int Converted_Int);
                    Status(Converted_Int, Live_Data[1] as string);
                }
            }
            catch (Exception Error)
            {
                Log_Detail.Full("DISCORD LAUNCHER PRESENCE [Task]", Error);
            }
            
            return Task.CompletedTask;
        }
        /// <summary>
        /// Sets the current Status of the Launcher's RPC_State
        /// </summary>
        /// <remarks>RPC Status</remarks>
        /// <param name="RPC_State">Which RPC Status Text to Set</param>
        /// <param name="RPC_Status">Additional RPC Status Details to Display</param>
        public static async Task Status_Async(int RPC_State, string RPC_Status)
        {
            try
            {
                await Task.Run(() => Status_Task(new object[] { RPC_State, RPC_Status, false })).ConfigureAwait(false);
            }
            catch (Exception Error)
            {
                Log_Detail.Full("DISCORD LAUNCHER PRESENCE [Async]", Error);
            }
        }
        /// <summary>
        /// Sets the current Status of the Launcher's RPC_State
        /// </summary>
        /// <remarks>RPC Status</remarks>
        /// <param name="RPC_State">Which RPC Status Text to Set</param>
        /// <param name="RPC_Status">Additional RPC Status Details to Display</param>
        /// <param name="RPC_Beta">Displays a Different Icon for Beta Launcher Builds</param>
        public static async Task Status_Async(int RPC_State, string RPC_Status, bool RPC_Beta)
        {
            try
            {
                await Task.Run(() => Status_Task(new object[] { RPC_State, RPC_Status, RPC_Beta })).ConfigureAwait(false);
            }
            catch (Exception Error)
            {
                Log_Detail.Full("DISCORD LAUNCHER PRESENCE [Async]", Error);
            }
        }
        /// <summary>
        /// Starts Game Launcher's RPC Status. If a Discord Client is Running on the Machine, it will Display the status on the User's Profile.
        /// </summary>
        /// <remarks>Displays Launcher and Server Status</remarks>
        public static void Start()
        {
            Start(true, string.Empty);
        }
        /// <summary>
        /// Starts Game Launcher's RPC Status. If a Discord Client is Running on the Machine, it will Display the status on the User's Profile.
        /// </summary>
        /// <param name="Boot_Or_Reboot">Calls an Invoke to Discord Client</param>
        /// <remarks>Displays Launcher and Server Status</remarks>
        public static void Start(bool Boot_Or_Reboot)
        {
            Start(Boot_Or_Reboot, string.Empty);
        }
        /// <summary>
        /// Starts Game Launcher's RPC Status. If a Discord Client is Running on the Machine, it will Display the status on the User's Profile.
        /// </summary>
        /// <param name="Boot_Or_Reboot">Calls an Invoke to Discord Client</param>
        /// <param name="RPC_ID">Custom Application ID for RPC. Default is Soapbox Race World's App</param>
        /// <remarks>Displays Launcher and Server Status</remarks>
        public static void Start(bool Boot_Or_Reboot, string RPC_ID)
        {
            try
            {
                if (!Presence_Settings.Disable_RPC_Startup)
                {
                    bool Valid_RPC = long.TryParse(RPC_ID, out long App_ID_Checked);

                    if (Valid_RPC || Boot_Or_Reboot)
                    {
                        Stop("Update");
                    }

                    Log.Core("DISCORD: Initializing Rich Presence Core" + (!Boot_Or_Reboot ? " For Server" : ""));

                    Client = new DiscordRpcClient(Valid_RPC ? App_ID_Checked.ToString() : "576154452348633108");
                    Client.OnReady += (_, Data) =>
                    {
                        Log.Info("DISCORD: Discord ready. Detected user: " + Data.User.Username + ". Discord version: " + Data.Version);
                        Launcher_Value.Launcher_Discord_UserID = Data.User.ID.ToString();
                        Invoked = true;
                    };
                    Client.OnError += (_, Error) =>
                    {
                        Log.Error("DISCORD [Client]: " + Error.Message);
                        Log.ErrorIC("DISCORD [Client]: " + Error.Code);
                        Log.ErrorFR("DISCORD [Client]: " + Error);
                    };
                    Client.SkipIdenticalPresence = true;
                    Client.ShutdownOnly = true;

                    if (Launcher_Value.Launcher_Select_Server_Category != "DEV")
                    {
                        Client.Initialize();
                    }
                    else
                    {
                        Stop("Close");
                    }

                    if (Boot_Or_Reboot)
                    {
                        Update();
                    }
                }
                else
                {
                    Log.Warning("DISCORD: User disabled Rich Presence Core from Launcher Settings");
                }
            }
            catch (Exception Error)
            {
                Log_Detail.Full("DISCORD", Error);
            }
        }

        /// <summary>
        /// Invokes a new Instance of RPC
        /// </summary>
        /// <remarks>Starts a new RPC for Launcher</remarks>
        public static void Update()
        {
            try
            {
                if (Running())
                {
                    Client.Invoke();
                }
            }
            catch (Exception Error)
            {
                Log_Detail.Full("DISCORD [Update]", Error);
            }
        }

        /// <summary>
        /// Invokes a Stop Command to RPC by Clearing Current Status before Disposing RPC
        /// </summary>
        /// <remarks>Clears and Stops RPC</remarks>
        public static void Stop(string RPC_State)
        {
            try
            {
                if (Running())
                {
                    try
                    {
                        if (RPC_State == "Close")
                        {
                            Client.ClearPresence();
                            Log.Core("DISCORD: Client RPC has now been Cleared");
                        }
                    }
                    catch (Exception Error)
                    {
                        Log_Detail.Full("DISCORD [ClearPresence]", Error);
                    }

                    Log.Core("DISCORD: Client RPC Service has been " + RPC_State + "d.");
                    Client.Dispose();
                    Client = null;
                    Invoked = false;
                }
            }
            catch (Exception Error)
            {
                Log_Detail.Full("DISCORD [Stop]", Error);
            }
        }
        /// <summary>
        /// Retives Discord Application ID by first checking the Server JSON, with the Server List being Second, and the Fallback being the Launcher's ID
        /// </summary>
        /// <remarks>Server's Discord Application ID</remarks>
        public static string ApplicationID()
        {
            return ApplicationID(string.Empty);
        }
        /// <summary>
        /// Retives Discord Application ID by first checking the Server JSON, with the Server List being Second, and the Fallback being the Launcher's ID
        /// </summary>
        /// <param name="FailSafe_App_ID">Custom Application ID for RPC. Default is Soapbox Race World's App</param>
        /// <remarks>Server's Discord Application ID</remarks>
        public static string ApplicationID(string FailSafe_App_ID)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Discord_ID ?? string.Empty))
                {
                    return Launcher_Value.Launcher_Select_Server_JSON.Server_Discord_ID;
                }
                else if (!string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_Data.DiscordAppID ?? string.Empty))
                {
                    return Launcher_Value.Launcher_Select_Server_Data.DiscordAppID;
                }
                else
                {
                    return long.TryParse(FailSafe_App_ID, out long App_ID_Checked) ? App_ID_Checked.ToString() : "540651192179752970";
                }
            }
            catch (OverflowException)
            {
                return "540651192179752970";
            }
            catch (Exception)
            {
                return "540651192179752970";
            }
        }
    }
}
