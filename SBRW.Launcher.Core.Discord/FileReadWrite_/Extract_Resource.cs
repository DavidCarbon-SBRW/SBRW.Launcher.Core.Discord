using SBRW.Launcher.Core.Classes.Extension.Logging_;
using System;
using System.IO;
using System.Reflection;

namespace SBRW.Launcher.Core.Discord.FileReadWrite_
{
    internal class Extract_Resource
    {
        internal static byte[] AsByte(String File_Name)
        {
            if (string.IsNullOrWhiteSpace(File_Name))
            {
                return null;
            }
            else
            {
                try
                {
                    Assembly TheRun = Assembly.GetExecutingAssembly();
                    using (Stream LiveStream = TheRun.GetManifestResourceStream(File_Name))
                    {
                        if (LiveStream == null) { return null; }
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
                    Log_Detail.OpenLog("Extract Resource AsByte", null, Error, null, true);
                    return null;
                }
            }
        }

        internal static String AsString(String File_Name)
        {
            if (string.IsNullOrWhiteSpace(File_Name))
            {
                return String.Empty;
            }
            else
            {
                try
                {
                    Assembly TheRun = Assembly.GetExecutingAssembly();
                    using (Stream LiveStream = TheRun.GetManifestResourceStream(File_Name))
                    {
                        if (LiveStream == null) { return String.Empty; }
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
                    Log_Detail.OpenLog("Extract Resource AsString", null, Error, null, true);
                    return String.Empty;
                }
            }
        }
    }
}
