using SBRW.Launcher.Core.Classes.Cache;
using SBRW.Launcher.Core.Classes.Extension.Logging_;
using SBRW.Launcher.Core.Classes.Reference.Json_.Newtonsoft_;
using System;
using System.Diagnostics;
using System.IO;

namespace SBRW.Launcher.Core.Extras.Launcher_
{
    public class Cache
    {
        public static Process GameProcess;

        /* Selected Server List Key Information */
        public static Json_List_Server SelectedServerData;

        /* Selected Server JSON (GetServerInformation) */
        public static Json_Server_Info SelectedServerJSON = new Json_Server_Info();

        public static string Name_Settings_Ini = "Settings.ini";
        public static string Name_Account_Ini = "Account.ini";
        public static string Launcher_Settings = Launcher_Value.System_Unix ? Name_Settings_Ini : Path.Combine(Log_Location.LauncherFolder, Name_Settings_Ini);

        public static readonly string RoamingAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string RoamingAppDataFolder_Launcher = Path.Combine(RoamingAppDataFolder, "Soapbox Race World", "Launcher");

        public static string Launcher_Account = Launcher_Value.System_Unix ? Name_Account_Ini : Path.Combine(RoamingAppDataFolder_Launcher, Name_Account_Ini);
    }
}
