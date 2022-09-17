using SBRW.Launcher.Core.Extension.Logging_;
using System;
using System.IO;
using System.Reflection;

namespace SBRW.Launcher.Core.Discord.FileReadWrite_
{
    internal class Extract_Resource
    {
        internal static byte[] AsByte(string File_Name)
        {
            if (string.IsNullOrWhiteSpace(File_Name))
            {
                return default;
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
                            return default; 
                        }
                        else
                        {
                            byte[] ba = new byte[LiveStream.Length];
                            LiveStream.Read(ba, 0, ba.Length);
                            return ba;
                        }
                    }
                }
                catch (Exception Error)
                {
                    Log_Detail.Full("Extract Resource AsByte", Error);
                    return default;
                }
            }
        }

        internal static string AsString(string File_Name)
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
                                return StreamViewer.ReadToEnd();
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
