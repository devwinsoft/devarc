using System;
using System.Threading;
using WebSocketSharp.Server;

namespace Devarc
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var wssv = new WebSocketServer(4649);

            //var wssv = new WebSocketServer ("ws://[::0]:4649");
            //var wssv = new WebSocketServer ("wss://[::0]:5963");
            //var wssv = new WebSocketServer ("ws://[::1]:4649");
            //var wssv = new WebSocketServer ("wss://[::1]:5963");

            //var wssv = new WebSocketServer (System.Net.IPAddress.IPv6Any, 4649);
            //var wssv = new WebSocketServer (System.Net.IPAddress.IPv6Any, 5963, true);
            //var wssv = new WebSocketServer (System.Net.IPAddress.IPv6Loopback, 4649);
            //var wssv = new WebSocketServer (System.Net.IPAddress.IPv6Loopback, 5963, true);
#if DEBUG
            // To change the logging level.
            wssv.Log.Level = WebSocketSharp.LogLevel.Trace;

            // To change the wait time for the response to the WebSocket Ping or Close.
            //wssv.WaitTime = TimeSpan.FromSeconds (2);

            // Not to remove the inactive sessions periodically.
            //wssv.KeepClean = false;
#endif
            // To provide the secure connection.
            /*
            var cert = ConfigurationManager.AppSettings["ServerCertFile"];
            var passwd = ConfigurationManager.AppSettings["CertFilePassword"];
            wssv.SslConfiguration.ServerCertificate = new X509Certificate2 (cert, passwd);
             */

            // To provide the HTTP Authentication (Basic/Digest).
            /*
            wssv.AuthenticationSchemes = AuthenticationSchemes.Basic;
            wssv.Realm = "WebSocket Test";
            wssv.UserCredentialsFinder = id => {
                var name = id.Name;

                // Return user name, password, and roles.
                return name == "nobita"
                       ? new NetworkCredential (name, "password", "gunfighter")
                       : null; // If the user credentials are not found.
              };
             */

            // To resolve to wait for socket in TIME_WAIT state.
            //wssv.ReuseAddress = true;

            wssv.AddWebSocketService<GamePacketHandler>("/Game");

            wssv.Start();

            if (wssv.IsListening)
            {
                Console.WriteLine("Listening on port {0}, and providing WebSocket services:", wssv.Port);

                foreach (var path in wssv.WebSocketServices.Paths)
                    Console.WriteLine("- {0}", path);

                while (wssv.IsListening)
                {
                    Thread.Sleep(100);
                }
            }
            wssv.Stop();
        }
    }
}
