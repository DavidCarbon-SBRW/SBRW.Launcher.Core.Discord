using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordRPC;
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
    public class Presence_Launcher
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
        /// Sets the current Status of the Launcher's State<br></br>
        /// </summary>
        /// <remarks>RPC Status<br></br></remarks>
        /// <param name="RPC_State">String - Which RPC Status Text to Set<br></br></param>
        /// <param name="RPC_Status">String - Additional RPC Status Details to Display<br></br></param>
        /// <param name="RPC_Beta">Bool - Displays a Different Icon for Beta Launcher Builds<br></br></param>
        public static void Status(string RPC_State, string RPC_Status, bool RPC_Beta = false)
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

                if (RPC_State == "Start Up")
                {
                    Presence.State = RPC_Status;
                    Presence.Details = "In-Launcher: " + Presence_Settings.Launcher_Version;
                    Presence.Assets = new Assets
                    {
                        LargeImageText = "Launcher",
                        LargeImageKey = RPC_Beta ? "nfsw_beta" : "nfsw"
                    };
                }
                else if (RPC_State == "Unpack Game Files")
                {
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
                }
                else if (RPC_State == "Download Game Files")
                {
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
                }
                else if (RPC_State == "Download Game Files Error")
                {
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
                }
                else if (RPC_State == "Idle Ready")
                {
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
                }
                else if (RPC_State == "Checking ModNet")
                {
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
                }
                else if (RPC_State == "ModNet File Check Passed")
                {
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
                }
                else if (RPC_State == "Download ModNet")
                {
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
                }
                else if (RPC_State == "Download ModNet Error")
                {
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
                }
                else if (RPC_State == "Download Server Mods")
                {
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
                }
                else if (RPC_State == "Download Server Mods Error")
                {
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
                }

                if (Download == false)
                {
                    if (RPC_State == "Security Center")
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
                    else if (RPC_State == "Register")
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
                    else if (RPC_State == "Settings")
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
                    else if (RPC_State == "User XML Editor")
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
                    else if (RPC_State == "Verify")
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
                    else if (RPC_State == "Verify Scan")
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
                    else if (RPC_State == "Verify Bad")
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
                    else if (RPC_State == "Verify Good")
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
                    else if (RPC_State == "In-Game")
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

                        if (!String.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home) ||
                            !String.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Discord) ||
                            !String.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Panel))
                        {
                            ButtonsList.Clear();

                            if (!String.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Panel))
                            {
                                /* Let's format it now, if possible */
                                ButtonsList.Add(new DiscordButton()
                                {
                                    Label = "View Panel",
                                    Url = Launcher_Value.Launcher_Select_Server_JSON.Server_Panel.Split(new string[] { "{sep}" }, StringSplitOptions.None)[0]
                                });
                            }
                            else if (!String.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home) &&
                                Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home != Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Discord)
                            {
                                ButtonsList.Add(new DiscordButton()
                                {
                                    Label = "Website",
                                    Url = Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home
                                });
                            }

                            if (!String.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Discord))
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
                }

                if (Running() && Launcher_Value.Launcher_Select_Server_Category != "DEV")
                    Client.SetPresence(Presence);
            }
            catch (Exception Error)
            {
                Log_Detail.OpenLog("DISCORD LAUNCHER PRESENCE", null, Error, null, true);
            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// Sets the current Status of the Launcher's RPC_State as a Task
        /// </summary>
        /// <param name="Object_Data"><inheritdoc cref="Status"/></param>
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
                        Status(Live_Data[0] as string, Live_Data[1] as string, Object_Bool);
                    }
                    else
                    {
                        Status(Live_Data[0] as string, Live_Data[1] as string);
                    }
                }
                else
                {
                    Status(Live_Data[0] as string, Live_Data[1] as string);
                }
            }
            catch (Exception Error)
            {
                Log_Detail.OpenLog("DISCORD LAUNCHER PRESENCE [Task]", string.Empty, Error, string.Empty, true);
            }
            finally
            {
                GC.Collect();
            }
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets the current Status of the Launcher's RPC_State
        /// </summary>
        /// <remarks>RPC Status</remarks>
        /// <param name="RPC_State">Which RPC Status Text to Set</param>
        /// <param name="RPC_Status">Additional RPC Status Details to Display</param>
        /// <param name="RPC_Beta">Displays a Different Icon for Beta Launcher Builds</param>
        public static async void Status_Async(string RPC_State, string RPC_Status, bool RPC_Beta = false)
        {
            try
            {
                await Task.Run(() => Status_Task(new object[] { RPC_State, RPC_Status, RPC_Beta })).ConfigureAwait(false);
            }
            catch (Exception Error)
            {
                Log_Detail.OpenLog("DISCORD LAUNCHER PRESENCE [Async]", string.Empty, Error, string.Empty, true);
            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// Starts Game Launcher's RPC Status. If a Discord Client is Running on the Machine, it will Display the status on the User's Profile.
        /// </summary>
        /// <param name="RPC_State"></param>
        /// <param name="RPC_ID"></param>
        /// <param name="FailSafe_App_ID">Custom Application ID for RPC. Default is Soapbox Race World's App</param>
        /// <remarks>Displays Launcher and Server Status</remarks>
        public static void Start(string RPC_State, string RPC_ID, string FailSafe_App_ID = null)
        {
            try
            {
                if (!Presence_Settings.Disable_RPC_Startup)
                {
                    if (RPC_State == "Start Up")
                    {
                        Log.Core("DISCORD: Initializing Rich Presence Core");

                        Client = new DiscordRpcClient(long.TryParse(FailSafe_App_ID, out long App_ID_Checked) ? App_ID_Checked.ToString() : "576154452348633108");

                        Client.OnReady += (sender, e) =>
                        {
                            Log.Info("DISCORD: Discord ready. Detected user: " + e.User.Username + ". Discord version: " + e.Version);
                            Launcher_Value.Launcher_Discord_UserID = e.User.ID.ToString();
                        };

                        Client.OnError += (sender, Error) =>
                        {
                            Log.Error("DISCORD: " + Error.Message);
                        };
                        Client.SkipIdenticalPresence = true;
                        Client.ShutdownOnly = true;

                        Client.Initialize();
                        Update();
                    }
                    else if (RPC_State == "New RPC")
                    {
                        if (Launcher_Value.Launcher_Select_Server_Category != "DEV")
                        {
                            Log.Core("DISCORD: Initializing Rich Presence Core For Server");

                            Stop("Update");
                            Client = new DiscordRpcClient(RPC_ID);

                            Client.OnError += (sender, Error) =>
                            {
                                Log.Error("DISCORD: " + Error.Message);
                            };

                            Client.Initialize();
                        }
                        else
                        {
                            Stop("Close");
                        }
                    }
                    else
                    {
                        Log.Error("DISCORD: Unable to determine the RPC RPC_State");
                    }
                }
                else
                {
                    Log.Warning("DISCORD: User disabled Rich Presence Core from Launcher Settings");
                }
            }
            catch (Exception Error)
            {
                Log_Detail.OpenLog("DISCORD", null, Error, null, true);
            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// Invokes a new Instance of RPC
        /// </summary>
        /// <remarks>Starts a new RPC for Launcher</remarks>
        public static void Update()
        {
            if (Running())
                Client.Invoke();
        }

        /// <summary>
        /// Invokes a Stop Command to RPC by Clearing Current Status before Disposing RPC
        /// </summary>
        /// <remarks>Clears and Stops RPC</remarks>
        public static void Stop(string RPC_State)
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
                catch { }
                Log.Core("DISCORD: Client RPC Service has been " + RPC_State + "d.");
                Client.Dispose();
                Client = null;
            }
        }

        /// <summary>
        /// Retives Discord Application ID by first checking the Server JSON, with the Server List being Second, and the Fallback being the Launcher's ID
        /// </summary>
        /// <param name="FailSafe_App_ID">Custom Application ID for RPC. Default is Soapbox Race World's App</param>
        /// <remarks>Server's Discord Application ID</remarks>
        public static string ApplicationID(string FailSafe_App_ID = null)
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
    }
}
