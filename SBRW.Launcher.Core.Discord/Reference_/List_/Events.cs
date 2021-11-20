using Newtonsoft.Json;
using SBRW.Launcher.Core.Classes.Extension.String_;
using SBRW.Launcher.Core.Discord.Reference_.Json_;
using SBRW.Launcher.Core.Discord.FileReadWrite_;
using System.Collections.Generic;

namespace SBRW.Launcher.Core.Discord.Reference_.List_
{
    /// <summary>
    /// Remote/Local RPC Events File
    /// </summary>
    public class Events
    {
        /// <summary>
        /// Cached List for Events ID/Names
        /// </summary>
        public static string List_File { get; set; }
        internal static List<Formats.Event> List_Cached { get; set; }
        /// <summary>
        /// Searches the name of the event
        /// </summary>
        /// <param name="Event_Id">Event ID Number</param>
        /// <returns>Event Name</returns>
        public static string Get_Name(int Event_Id)
        {
            /* Let's load the "Cached From Server" version first and If we don't have a Server version, load "default" version */
            if (List_Cached == null || List_Cached.Count <= 0)
            {
                List_Cached = new List<Formats.Event>();
                List_Cached.AddRange(
                    JsonConvert.DeserializeObject<List<Formats.Event>>
                    (Strings.Encode(
                        !string.IsNullOrWhiteSpace(List_File) ? List_File :
                        Extract_Resource.AsString("SBRW.Launcher.Core.Discord.Reference_.Json_.Events.json"))));
            }

            int Results_Index = List_Cached.FindIndex(i => Equals(i.id, Event_Id));

            if (Results_Index >= 0)
            {
                return List_Cached.Find(i => Equals(i.id, Event_Id)).trackname;
            }

            /* And if it's not found, do this instead */
            return "EVENT:" + Event_Id;
        }
        /// <summary>
        /// Searches the type of event
        /// </summary>
        /// <param name="Event_Id">Event ID Number</param>
        /// <returns>Event Type</returns>
        public static string Get_Type(int Event_Id)
        {
            /* Let's load the "Cached From Server" version first and If we don't have a Server version, load "default" version */
            if (List_Cached == null || List_Cached.Count <= 0)
            {
                List_Cached = new List<Formats.Event>();
                List_Cached.AddRange(
                    JsonConvert.DeserializeObject<List<Formats.Event>>
                    (Strings.Encode(
                        !string.IsNullOrWhiteSpace(List_File) ? List_File :
                        Extract_Resource.AsString("SBRW.Launcher.Core.Discord.Reference_.Json_.Events.json"))));
            }

            int Results_Index = List_Cached.FindIndex(i => Equals(i.id, Event_Id));

            if (Results_Index >= 0)
            {
                return List_Cached.Find(i => Equals(i.id, Event_Id)).type;
            }

            /* And if it's not found, do this instead */
            return "gamemode_freeroam";
        }
    }
}
