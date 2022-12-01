using Newtonsoft.Json;
using SBRW.Launcher.Core.Extension.String_;
using SBRW.Launcher.Core.Discord.FileReadWrite_;
using System;
using SBRW.Launcher.Core.Extension.Logging_;
using System.Collections.Generic;
using SBRW.Launcher.Core.Reference.Json_.Newtonsoft_;

namespace SBRW.Launcher.Core.Discord.Reference_.List_
{
    /// <summary>
    /// Remote/Local RPC Cars File
    /// </summary>
    public static class Cars
    {
        /// <summary>
        /// Cached List for Car ID/Names
        /// </summary>
        public static string List_File { get; set; }
        internal static List<Json_External_RPC.GME_Car> List_Cached { get; set; }
        /// <summary>
        /// Searches for the Name of a Car
        /// </summary>
        /// <param name="Car_Id">Cars ID</param>
        /// <returns>Car Name</returns>
        public static string Get_Name(string Car_Id)
        {
            try
            {
                /* Let's load the "Cached From Server" version first and If we don't have a Server version, load "default" version */
                if (List_Cached == null || List_Cached.Count <= 0)
                {
                    List_Cached = new List<Json_External_RPC.GME_Car>();
                    List_Cached.AddRange(
                        JsonConvert.DeserializeObject<List<Json_External_RPC.GME_Car>>
                        (!string.IsNullOrWhiteSpace(List_File) ? List_File.Encode_UTF8() :
                            "SBRW.Launcher.Core.Discord.Reference_.Json_.Cars.json".ToString_UTF8()));
                }

                int Results_Index = List_Cached.FindIndex(i => string.Equals(i.ID, Car_Id));

                if (Results_Index >= 0)
                {
                    string Car_Result = List_Cached.Find(i => string.Equals(i.ID, Car_Id)).Name;

                    if (!string.IsNullOrWhiteSpace(Car_Result) && Car_Result != Car_Id)
                    {
                        return Car_Result;
                    }
                }
            }
            catch (Exception Error)
            {
                Log_Detail.Full("Car Name RPC Search", Error);
            }

            /* And if it's not found, do this instead */
            return "Addon Car (" + Car_Id + ")";
        }
    }
}
