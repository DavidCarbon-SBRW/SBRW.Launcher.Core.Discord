using Newtonsoft.Json;
using SBRW.Launcher.Core.Classes.Extension.String_;
using SBRW.Launcher.Core.Extras.FileReadWrite_;
using System;

namespace SBRW.Launcher.Core.Discord.Reference_.List_
{
    /// <summary>
    /// Remote/Local RPC Cars File
    /// </summary>
    public class Cars
    {
        /// <summary>
        /// Cached List for Car ID/Names
        /// </summary>
        public static string List { get; set; }
        /// <summary>
        /// Searches for the Name of a Car
        /// </summary>
        /// <param name="Car_Id">Cars ID</param>
        /// <returns>Car Name</returns>
        public static string Get_Name(string Car_Id)
        {
            /* Let's load the "Cached From Server" version first */
            if (!string.IsNullOrWhiteSpace(List))
            {
                dynamic Remote_Json = JsonConvert.DeserializeObject(Strings.Encode(List));

                foreach (dynamic List_Item in Remote_Json)
                {
                    if (List_Item.carid == Car_Id)
                    {
                        return List_Item.carname;
                    }
                }
            }
            /* If we don't have a Server version, load "default" version */
            else
            {
                dynamic Local_Json = JsonConvert.DeserializeObject(Strings.Encode(Extract_Resource.AsString("SBRW.Launcher.Core.Extras.Discord_.Reference_.Json_.Cars.json")));

                foreach (dynamic List_Item in Local_Json)
                {
                    if (List_Item.carid == Car_Id)
                    {
                        return List_Item.carname;
                    }
                }
            }

            /* And if it's not found, do this instead */
            return "Traffic Car (" + Car_Id + ")";
        }
    }
}
