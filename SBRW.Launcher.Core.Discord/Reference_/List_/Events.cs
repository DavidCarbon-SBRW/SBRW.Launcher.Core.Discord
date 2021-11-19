using Newtonsoft.Json;
using SBRW.Launcher.Core.Classes.Extension.String_;
using SBRW.Launcher.Core.Extras.FileReadWrite_;

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
        public static string List { get; set; }
        /// <summary>
        /// Searches the name of the event
        /// </summary>
        /// <param name="Event_Id">Event ID Number</param>
        /// <returns>Event Name</returns>
        public static string Get_Name(int Event_Id)
        {
            /* Let's load the "From Server" version first */
            if (!string.IsNullOrWhiteSpace(List))
            {
                dynamic Remote_Json = JsonConvert.DeserializeObject(Strings.Encode(List));

                foreach (dynamic List_Item in Remote_Json)
                {
                    if (List_Item.id == Event_Id)
                    {
                        return List_Item.trackname;
                    }
                }
            }
            /* If we don't have a Server version, load "default" version */
            else
            {
                dynamic dynJson = JsonConvert.DeserializeObject(Strings.Encode(
                    Extract_Resource.AsString("SBRW.Launcher.Core.Extras.Discord_.Reference_.Json_.Events.json")));

                foreach (dynamic List_Item in dynJson)
                {
                    if (List_Item.id == Event_Id)
                    {
                        return List_Item.trackname;
                    }
                }
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
            /* Let's load the "From Server" version first */
            if (!string.IsNullOrWhiteSpace(List))
            {
                dynamic Remote_Json = JsonConvert.DeserializeObject(Strings.Encode(List));

                foreach (dynamic List_Item in Remote_Json)
                {
                    if (List_Item.id == Event_Id)
                    {
                        return List_Item.type;
                    }
                }
            }
            /* If we don't have a Server version, load "default" version */
            else
            {
                dynamic Remote_Json = JsonConvert.DeserializeObject(Strings.Encode(
                    Extract_Resource.AsString("SBRW.Launcher.Core.Extras.Discord_.Reference_.Json_.Events.json")));

                foreach (dynamic List_Item in Remote_Json)
                {
                    if (List_Item.id == Event_Id)
                    {
                        return List_Item.type;
                    }
                }
            }

            /* And if it's not found, do this instead */
            return "gamemode_freeroam";
        }
    }
}
