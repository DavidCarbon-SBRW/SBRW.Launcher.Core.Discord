using Newtonsoft.Json;
using SBRW.Launcher.Core.Extension.String_;
using SBRW.Launcher.Core.Discord.FileReadWrite_;
using System.Collections.Generic;
using SBRW.Launcher.Core.Reference.Json_.Newtonsoft_;
using System;
using SBRW.Launcher.Core.Extension.Logging_;

namespace SBRW.Launcher.Core.Discord.Reference_.List_
{
    /// <summary>
    /// Remote/Local RPC Events File
    /// </summary>
    public static class Events
    {
        /// <summary>
        /// Cached List for Events ID/Names
        /// </summary>
        public static string List_File { get; set; }
        internal static List<Json_External_RPC.GME_Event> List_Cached { get; set; }
        /// <summary>
        /// Searches the name of the event
        /// </summary>
        /// <param name="Event_Id">Event ID Number</param>
        /// <returns>Event Name</returns>
        public static string Get_Name(int Event_Id)
        {
            try
            {
                /* Let's load the "Cached From Server" version first and If we don't have a Server version, load "default" version */
                if (List_Cached == null || List_Cached.Count <= 0)
                {
                    List_Cached = new List<Json_External_RPC.GME_Event>();
                    List_Cached.AddRange(
                        JsonConvert.DeserializeObject<List<Json_External_RPC.GME_Event>>
                        (!string.IsNullOrWhiteSpace(List_File) ? List_File.Encode_UTF8() :
                            "SBRW.Launcher.Core.Discord.Reference_.Json_.Events.json".ToString_UTF8()));
                }

                int Results_Index = List_Cached.FindIndex(i => string.Equals(i.ID, Event_Id.ToString()));

                if (Results_Index >= 0)
                {
                    string Event_Result = List_Cached.Find(i => string.Equals(i.ID, Event_Id.ToString())).Name;

                    if (!string.IsNullOrWhiteSpace(Event_Result) && Event_Result != Event_Id.ToString())
                    {
                        return Event_Result;
                    }
                }
            }
            catch (Exception Error)
            {
                Log_Detail.Full("Event Name RPC Search", Error);
            }
            finally
            {
                GC.Collect();
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
            try
            {
                /* Let's load the "Cached From Server" version first and If we don't have a Server version, load "default" version */
                if (List_Cached == null || List_Cached.Count <= 0)
                {
                    List_Cached = new List<Json_External_RPC.GME_Event>();
                    List_Cached.AddRange(
                        JsonConvert.DeserializeObject<List<Json_External_RPC.GME_Event>>
                        (!string.IsNullOrWhiteSpace(List_File) ? List_File.Encode_UTF8() :
                            "SBRW.Launcher.Core.Discord.Reference_.Json_.Events.json".ToString_UTF8()));
                }

                int Results_Index = List_Cached.FindIndex(i => string.Equals(i.ID, Event_Id.ToString()));

                if (Results_Index >= 0)
                {
                    string Event_Result = List_Cached.Find(i => string.Equals(i.ID, Event_Id.ToString())).Type;

                    if (!string.IsNullOrWhiteSpace(Event_Result) && Event_Result != Event_Id.ToString())
                    {
                        return Event_Result;
                    }
                }
            }
            catch (Exception Error)
            {
                Log_Detail.Full("Event Type RPC Search", Error);
            }

            /* And if it's not found, do this instead */
            return "gamemode_freeroam";
        }
    }
}
