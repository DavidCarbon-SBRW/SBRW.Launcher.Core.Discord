using Nancy.Hosting.Self;
using SBRW.Launcher.Core.Classes.Cache;
using SBRW.Launcher.Core.Classes.Extension.Logging_;
using SBRW.Launcher.Core.Extras.Proxy_.Singleton_Instance;
using System;

namespace SBRW.Launcher.Core.Extras.Proxy_.Nancy_
{
    public class Proxy_Server : Singleton<Proxy_Server>
    {
        public static int ProxyPort;

        private string _serverUrl;
        private string _serverName;
        public static NancyHost Host;
        public static bool Client_Session = false;

        /// <summary>
        /// Boolean Value on Launcher Proxy if its Running
        /// </summary>
        /// <returns>True or False</returns>
        public static bool Running() => Launcher_Value.Launcher_Proxy = Host != null;

        public string GetServerUrl() => _serverUrl;
        public void SetServerUrl(string serverUrl)
        {
            _serverUrl = serverUrl;
        }

        public string GetServerName() => _serverName;
        public void SetServerName(string serverName)
        {
            _serverName = serverName;
        }

        public void Start(string From)
        {
            try
            {
                if (Running())
                {
                    Log.Warning("PROXY: Local Proxy Server already Running! (" + From + ")");
                }
                else
                {
                    Log.Info("PROXY: Local Proxy Server has Fully Initialized (" + From + ")");

                    HostConfiguration Configs = new HostConfiguration()
                    {
                        AllowChunkedEncoding = false,
                        RewriteLocalhost = false,
                        UrlReservations = new UrlReservations()
                        {
                            CreateAutomatically = true
                        }
                    };

                    Host = new NancyHost(new Uri("http://127.0.0.1:" + ProxyPort), new Nancy_Bootstrapper(), Configs);
                    Host.Start();
                }
            }
            catch (AutomaticUrlReservationCreationFailureException Error)
            {
                Log_Detail.OpenLog("PROXY [U.R.]", null, Error, null, true);
            }
            catch (Exception Error)
            {
                Log_Detail.OpenLog("PROXY", null, Error, null, true);
            }
        }

        public void Stop(string From)
        {
            if (Running())
            {
                Log.Info("PROXY: Local Proxy Server has Shutdown (" + From + ")");
                Host.Stop();
                Host.Dispose();
                Host = null;
            }
            else
            {
                Log.Warning("PROXY: Local Proxy Server is already Shutdown (" + From + ")");
            }
        }
    }
}
