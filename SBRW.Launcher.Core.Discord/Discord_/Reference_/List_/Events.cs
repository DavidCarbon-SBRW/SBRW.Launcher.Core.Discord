using Newtonsoft.Json;
using SBRW.Launcher.Core.Classes.Extension.String_;
using SBRW.Launcher.Core.Extras.FileReadWrite_;
using System;

namespace SBRW.Launcher.Core.Discord.Discord_.Reference_.List_
{
    public class Events
    {
        public static String remoteEventsList = String.Empty;

        public static string GetEventName(int id)
        {
            /* Let's load the "From Server" version first */
            if (remoteEventsList != String.Empty)
            {
                dynamic dynJson = JsonConvert.DeserializeObject(Strings.Encode(remoteEventsList));

                foreach (var item in dynJson)
                {
                    if (item.id == id)
                    {
                        return item.trackname;
                    }
                }
            }

            /* If we don't have a Server version, load "default" version */
            if (remoteEventsList == String.Empty)
            {
                dynamic dynJson = JsonConvert.DeserializeObject(Strings.Encode(
                    ExtractResource.AsString("SBRW.Launcher.Core.Extras.Discord_.Reference_.Json_.Events.json")));

                foreach (var item in dynJson)
                {
                    if (item.id == id)
                    {
                        return item.trackname;
                    }
                }
            }

            /* And if it's not found, do this instead */
            return "EVENT:" + id;
        }

        public static string GetEventType(int id)
        {
            /* Let's load the "From Server" version first */
            if (remoteEventsList != String.Empty)
            {
                dynamic dynJson = JsonConvert.DeserializeObject(Strings.Encode(remoteEventsList));

                foreach (var item in dynJson)
                {
                    if (item.id == id)
                    {
                        return item.type;
                    }
                }
            }

            /* If we don't have a Server version, load "default" version */
            if (remoteEventsList != String.Empty)
            {
                dynamic dynJson = JsonConvert.DeserializeObject(Strings.Encode(
                    ExtractResource.AsString("SBRW.Launcher.Core.Extras.Discord_.Reference_.Json_.Events.json")));

                foreach (var item in dynJson)
                {
                    if (item.id == id)
                    {
                        return item.type;
                    }
                }
            }

            /* And if it's not found, do this instead */
            return "gamemode_freeroam";
        }
    }
}
