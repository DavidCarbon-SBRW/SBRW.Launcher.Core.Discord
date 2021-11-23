using SBRW.Launcher.Core.Extension.Web_;
using SBRW.Launcher.Core.Discord.Reference_.List_;

namespace SBRW.Launcher.Core.Discord.RPC_
{
    /// <summary>
    /// Discord RPC Local Settings
    /// </summary>
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
        /// <summary>
        /// Clears and Resets Cached RPC Lists for Cars and Events
        /// </summary>
        public static void Clear_RPC_List()
        {
            Cars.List_File = Events.List_File = string.Empty;
            Cars.List_Cached.Clear();
            Events.List_Cached.Clear();
        }
    }
}
