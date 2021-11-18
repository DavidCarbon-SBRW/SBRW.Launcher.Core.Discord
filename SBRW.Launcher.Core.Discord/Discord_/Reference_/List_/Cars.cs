using Newtonsoft.Json;
using SBRW.Launcher.Core.Classes.Extension.String_;
using SBRW.Launcher.Core.Extras.FileReadWrite_;
using System;

namespace SBRW.Launcher.Core.Discord.Discord_.Reference_.List_
{
    public class Cars
    {
        public static String remoteCarsList = String.Empty;

        public static string GetCarName(string id)
        {
            /* Let's load the "Cached From Server" version first */
            if (remoteCarsList != String.Empty)
            {
                dynamic dynJson = JsonConvert.DeserializeObject(Strings.Encode(remoteCarsList));

                foreach (var item in dynJson)
                {
                    if (item.carid == id)
                    {
                        return item.carname;
                    }
                }
            }

            /* If we don't have a Server version, load "default" version */
            if (remoteCarsList == String.Empty)
            {
                dynamic dynJson = JsonConvert.DeserializeObject(Strings.Encode(ExtractResource.AsString("SBRW.Launcher.Core.Extras.Discord_.Reference_.Json_.Cars.json")));

                foreach (var item in dynJson)
                {
                    if (item.carid == id)
                    {
                        return item.carname;
                    }
                }
            }

            /* And if it's not found, do this instead */
            return "Traffic Car (" + id + ")";
        }
    }
}
