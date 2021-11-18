using SBRW.Launcher.Core.Classes.Extension.Web_;

namespace SBRW.Launcher.Core.Discord.Discord_.RPC_
{
    public class Presence_Settings
    {
        /// <summary>
        /// Launcher Version
        /// </summary>
        /// <returns>Application Version</returns>
        public static string Launcher_Version { get; set; } = Custom_Header.Version();
        /// <summary>
        /// Prevents RPC from Starting Up
        /// </summary>
        public static bool Disable_RPC_Startup { get; set; }
    }
}
