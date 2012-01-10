using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Loki.IO;
using Loki.Net;

namespace Loki.Interoperability
{
    public static class InteroperabilityServer
    {
        private static bool isAlive;
        private static ManualResetEvent AcceptDone = new ManualResetEvent(false);

        public static TcpListener Listener { get; private set; }
        public static List<ChannelServerHandler> Servers { get; private set; }

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
                    InteroperabilityServer.AcceptDone.Set();
                }
            }
        }

        public static void Main()
        {
            InteroperabilityServer.Servers = new List<ChannelServerHandler>();

            try
            {
                Settings.Initialize();

                InteroperabilityServer.Listener = new TcpListener(IPAddress.Any, Settings.GetInt("Channels/Port"));
                InteroperabilityServer.Listener.Start();
                Log.Inform("Initialized interoperability listener on {0}.", LoginServer.Listener.LocalEndpoint);

                InteroperabilityServer.IsAlive = true;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            if (InteroperabilityServer.IsAlive)
            {
                Log.Success("Interoperability started on thread {0}.", Thread.CurrentThread.ManagedThreadId);
            }
            else
            {
                Log.Inform("Could not start interoperability because of errors.");
            }

            while (InteroperabilityServer.IsAlive)
            {
                InteroperabilityServer.AcceptDone.Reset();

                InteroperabilityServer.Listener.BeginAcceptSocket(new AsyncCallback(InteroperabilityServer.OnAcceptSocket), null);

                InteroperabilityServer.AcceptDone.WaitOne();
            }

            ChannelServerHandler[] remainingServers = InteroperabilityServer.Servers.ToArray();

            foreach (ChannelServerHandler server in remainingServers)
            {
                server.Stop();
            }

            InteroperabilityServer.Dispose();

            Log.Warn("Interoperability stopped.");
        }

        private static void OnAcceptSocket(IAsyncResult ar)
        {
            InteroperabilityServer.AcceptDone.Set();
            new ChannelServerHandler(InteroperabilityServer.Listener.EndAcceptSocket(ar));
        }

        public static void Stop()
        {
            InteroperabilityServer.IsAlive = false;
        }

        private static void Dispose()
        {
            if (InteroperabilityServer.Listener != null)
            {
                LoginServer.Listener.Stop();
            }

            Log.Inform("Interoperability disposed.", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
