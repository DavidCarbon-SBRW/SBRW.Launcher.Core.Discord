using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Extension.String_;
using System;
using System.IO;
using System.Reflection;

namespace SBRW.Launcher.Core.Discord.FileReadWrite_
{
    internal static class Extract_Resource
    {
        internal static string ToString_UTF8(this string File_Name)
        {
            if (string.IsNullOrWhiteSpace(File_Name))
            {
                return string.Empty;
            }
            else
            {
                try
                {
                    Assembly TheRun = Assembly.GetExecutingAssembly();
                    using (Stream LiveStream = TheRun.GetManifestResourceStream(File_Name))
                    {
                        if (LiveStream == null) 
                        { 
                            return string.Empty; 
                        }
                        else
                        {
                            using (StreamReader StreamViewer = new StreamReader(LiveStream))
                            {
                                return StreamViewer.ReadToEnd().Encode_UTF8();
                            }
                        }
                    }
                }
                catch (Exception Error)
                {
                    Log_Detail.Full("Extract Resource AsString", Error);
                    return string.Empty;
                }
            }
        }
    }
}
