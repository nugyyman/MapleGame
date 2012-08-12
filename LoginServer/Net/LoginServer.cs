using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Loki.Data;
using Loki.Interoperability;
using Loki.IO;

namespace Loki.Net
{
    public static class LoginServer
    {
        private static bool isAlive;
        private static ManualResetEvent AcceptDone = new ManualResetEvent(false);

        public static TcpListener Listener { get; private set; }
        public static Worlds Worlds { get; private set; }
        public static ChannelsHelper Channels { get; private set; }
        public static List<int> LoggedIn { get; private set; }
        public static List<LoginClientHandler> Clients { get; private set; }

        public static bool AutoRegister { get; private set; }
        public static bool RequireStaffIP { get; private set; }
        public static bool RequestPin { get; private set; }
        public static bool RequestPic { get; private set; }
        public static int MaxCharacters { get; private set; }
        public static bool EnableSpecialCharCreation { get; private set; }

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
                    LoginServer.AcceptDone.Set();
                }
            }
        }

        private static void Main(string[] args)
        {
            if (args.Length == 1 && args[0].ToLower() == "setup" || !File.Exists(Application.ExecutablePath + "Configuration.ini"))
            {
                LoginServerSetup.Run();
            }

            LoginServer.Worlds = new Worlds();
            LoginServer.Channels = new ChannelsHelper();
            LoginServer.LoggedIn = new List<int>();
            LoginServer.Clients = new List<LoginClientHandler>();

            Log.Entitle("Login Server v.{0}.{1}", Application.MapleVersion, Application.PatchVersion);

            try
            {
                Settings.Initialize();

                Database.Test();
                Database.Analyze(false);

                ChannelServerHandler.SecurityCode = Settings.GetString("Channels/SecurityCode");
                Log.Inform("Cross-servers code '{0}' assigned.", Log.MaskString(ChannelServerHandler.SecurityCode));

                LoginServer.RequireStaffIP = Settings.GetBool("Server/RequireStaffIP");
                Log.Inform("Staff will{0}be required to connect through a staff IP.", LoginServer.RequireStaffIP ? " " : " not ");

                LoginServer.AutoRegister = Settings.GetBool("Server/AutoRegister");
                Log.Inform("Automatic registration {0}.", LoginServer.AutoRegister ? "enabled" : "disabled");

                LoginServer.RequestPin = Settings.GetBool("Server/RequestPin");
                Log.Inform("Pin will{0}be requested upon login.", LoginServer.RequestPin ? " " : " not ");

                LoginServer.RequestPic = Settings.GetBool("Server/RequestPic");
                Log.Inform("Pic will{0}be requested upon char selection.", LoginServer.RequestPic ? " " : " not ");

                LoginServer.MaxCharacters = Settings.GetInt("Server/MaxCharacters");
                Log.Inform("Maximum of {0} characters per account.", LoginServer.MaxCharacters);

                LoginServer.EnableSpecialCharCreation = true;
                Log.Inform("Special char creation is{0}.", LoginServer.EnableSpecialCharCreation ? " enabled" : " disabled");

                LoginServer.Listener = new TcpListener(IPAddress.Any, Settings.GetInt("Server/Port"));
                LoginServer.Listener.Start();
                Log.Inform("Initialized clients listener on {0}.", LoginServer.Listener.LocalEndpoint);

                LoginServer.IsAlive = true;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            if (LoginServer.IsAlive)
            {
                Log.Success("Server started on thread {0}.", Thread.CurrentThread.ManagedThreadId);

                new Thread(new ThreadStart(InteroperabilityServer.Main)).Start();
            }
            else
            {
                Log.Inform("Could not start server because of errors.");
            }

            while (LoginServer.IsAlive)
            {
                LoginServer.AcceptDone.Reset();

                LoginServer.Listener.BeginAcceptSocket(new AsyncCallback(LoginServer.OnAcceptSocket), null);

                LoginServer.AcceptDone.WaitOne();
            }

            InteroperabilityServer.Stop();

            LoginClientHandler[] remainingClients = LoginServer.Clients.ToArray();

            foreach (LoginClientHandler client in remainingClients)
            {
                client.Stop();
            }

            LoginServer.Dispose();

            Log.Warn("Server stopped.");

            Console.Read();
        }

        private static void OnAcceptSocket(IAsyncResult ar)
        {
            LoginServer.AcceptDone.Set();
            new LoginClientHandler(LoginServer.Listener.EndAcceptSocket(ar));
        }

        public static void Stop()
        {
            LoginServer.IsAlive = false;
        }

        private static void Dispose()
        {
            if (LoginServer.Listener != null)
            {
                LoginServer.Listener.Stop();
            }

            Log.Inform("Server disposed.", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
