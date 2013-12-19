using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Loki.Data;
using Loki.Interoperability;
using Loki.IO;
using Loki.Maple;
using Loki.Maple.Data;
using Loki.Shell;

namespace Loki.Net
{
    public static class ChannelServer
    {
        private static bool isAlive;
        private static byte channelID;
        private static ManualResetEvent AcceptDone = new ManualResetEvent(false);

        public static TcpListener Listener { get; private set; }
        public static IPEndPoint RemoteEndPoint { get; private set; }
        public static InteroperabilityClient LoginServerConnection { get; set; }
        public static List<int> LoggedIn { get; private set; }
        public static List<ChannelClientHandler> Clients { get; private set; }
        public static byte WorldID { get; set; }
        public static int AutoRestartTime { get; set; }

        public static int MaxUsers { get; set; }
        public static int ExperienceRate { get; set; }
        public static int QuestExperienceRate { get; set; }
        public static int PartyQuestExperienceRate { get; set; }
        public static int MesoRate { get; set; }
        public static int DropRate { get; set; }
        public static bool AllowMultiLeveling { get; set; } // TODO.

        public static bool IsAlive
        {
            get
            {
                return isAlive;
            }
            set
            {
                isAlive = value;

                if (!value)
                {
                    ChannelServer.AcceptDone.Set();
                }
            }
        }

        public static byte ChannelID
        {
            get
            {
                return channelID;
            }
            set
            {
                channelID = value;

                Console.Title = string.Format("Channel Server v.{0}.{1} ({2}-{3})",
                    Application.MapleVersion,
                    Application.PatchVersion,
                    WorldNameResolver.GetName(ChannelServer.WorldID),
                    ChannelServer.ChannelID);
            }
        }

        public static byte InternalChannelID
        {
            get
            {
                return (byte)(ChannelServer.ChannelID - 1);
            }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length == 1 && args[0].ToLower() == "setup" || !File.Exists(Application.ExecutablePath + "Configuration.ini"))
            {
                ChannelServerSetup.Run();
            }

            int port = 0;

        start:
            ChannelServer.Clients = new List<ChannelClientHandler>();

            Log.Entitle("Channel Server v.{0}.{1}", Application.MapleVersion, Application.PatchVersion);

            try
            {
                if (port == 0)
                {
                    try
                    {
                        port = int.Parse(args[0]);
                    }
                    catch
                    {
                        port = Log.Input("Port: ", 7575);
                    }
                }

                Settings.Initialize();
                Shortcuts.Apply();

                ChannelServer.AutoRestartTime = Settings.GetInt("Server/AutoRestartTime");
                Log.Inform("Automatic restart time set to {0} seconds.", ChannelServer.AutoRestartTime);

                Database.Test();
                Database.Analyze(true);

                ChannelServer.LoggedIn = new List<int>(Settings.GetInt("Server/MaxUsers"));
                Log.Inform("Maximum of {0} simultaneous online users.", ChannelServer.LoggedIn.Capacity);

                ChannelServer.RemoteEndPoint = new IPEndPoint(Settings.GetIPAddress("Server/ExternalIP"), port);

                ChannelData.Initialize();

                ChannelServer.Listener = new TcpListener(IPAddress.Any, ChannelServer.RemoteEndPoint.Port);
                ChannelServer.Listener.Start();
                Log.Inform("Initialized clients listener on {0}.", ChannelServer.Listener.LocalEndpoint);

                ChannelServer.IsAlive = true;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            if (ChannelServer.IsAlive)
            {
                Log.Success("Channel server started.");

                new Thread(new ThreadStart(InteroperabilityClient.Main)).Start();
            }
            else
            {
                Log.Inform("Could not start server because of errors.");
            }

            while (ChannelServer.IsAlive)
            {
                ChannelServer.AcceptDone.Reset();

                ChannelServer.Listener.BeginAcceptSocket(new AsyncCallback(ChannelServer.OnAcceptSocket), null);

                ChannelServer.AcceptDone.WaitOne();
            }

            ChannelClientHandler[] remainingClients = ChannelServer.Clients.ToArray();

            foreach (ChannelClientHandler client in remainingClients)
            {
                client.Stop();
            }

            ChannelServer.Dispose();

            Log.Warn("Server stopped.");

            if (ChannelServer.AutoRestartTime > 0)
            {
                Log.Inform("Attempting auto-restart in {0} seconds.", ChannelServer.AutoRestartTime);
                Thread.Sleep(ChannelServer.AutoRestartTime * 1000);
                goto start;
            }
            else
            {
                Console.Read();
            }
        }

        private static void OnAcceptSocket(IAsyncResult ar)
        {
            ChannelServer.AcceptDone.Set();

            try
            {
                new ChannelClientHandler(ChannelServer.Listener.EndAcceptSocket(ar));
            }
            catch (ObjectDisposedException) { }
        }

        public static void Stop()
        {
            ChannelServer.IsAlive = false;
        }

        public static void Dispose()
        {
            if (ChannelServer.LoginServerConnection != null)
            {
                ChannelServer.LoginServerConnection.Dispose();
            }

            if (ChannelServer.Listener != null)
            {
                ChannelServer.Listener.Stop();
            }

            Log.Inform("Server disposed from thread {0}.", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
