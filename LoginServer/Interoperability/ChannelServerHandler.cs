using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Loki.Collections;
using Loki.Data;
using Loki.Maple;
using Loki.Net;
using Loki.Security;

namespace Loki.Interoperability
{
    public class ChannelServerHandler : ClientHandler<InteroperabilityOperationCode, InteroperabilityOperationCode, BlankCryptograph>
    {
        private byte ID;

        public byte WorldID { get; private set; }
        public static string SecurityCode { get; set; }
        public new IPEndPoint RemoteEndPoint { get; private set; }

        public PendingKeyedQueue<int, bool> LoggedInPool = new PendingKeyedQueue<int, bool>();
        public PendingQueue<float> LoadPool = new PendingQueue<float>();

        public World World
        {
            get
            {
                return LoginServer.Worlds[this.WorldID];
            }
        }

        public byte InternalID
        {
            get
            {
                return (byte)(this.ExternalID - 1);
            }
        }

        public byte ExternalID
        {
            get
            {
                return ID;
            }
            private set
            {
                this.ID = value;
                this.UpdateID(this.ExternalID);
            }
        }

        public ChannelServerHandler(Socket socket) : base(socket, "Channel") { }

        protected override void Register()
        {
            InteroperabilityServer.Servers.Add(this);
        }

        protected override void Prepare(params object[] args)
        {
            this.RemoteEndPoint = (IPEndPoint)this.Socket.RemoteEndPoint;
        }

        protected override void Terminate()
        {
            if (LoginServer.Worlds.Contains(this.WorldID))
            {
                if (this.World.Contains(this))
                {
                    this.World.Remove(this);
                    Log.Warn("Unregistered channel server {0}-{1}.", this.World.Name, this.ExternalID);
                }

                foreach (ChannelServerHandler channel in this.World)
                {
                    if (channel.ExternalID > this.ExternalID)
                    {
                        channel.ExternalID--;
                    }
                }

                Log.Inform("Updated according channel IDs.");

                if (this.World.Count == 0)
                {
                    string name = this.World.Name;
                    LoginServer.Worlds.Remove(this.World);
                    Log.Inform("Removed the empty '{0}' World entry.", name);
                }
            }
        }

        protected override void Unregister()
        {
            InteroperabilityServer.Servers.Remove(this);
        }

        protected override bool IsServerAlive
        {
            get
            {
                return LoginServer.IsAlive;
            }
        }

        protected override void Dispatch(Packet inPacket)
        {
            switch ((InteroperabilityOperationCode)inPacket.OperationCode)
            {
                case InteroperabilityOperationCode.RegistrationRequest:
                    this.HandleRegistrationRequest(inPacket);
                    break;

                case InteroperabilityOperationCode.CharacterEntriesResponse:
                    int id = inPacket.ReadInt();

                    List<byte[]> entries = new List<byte[]>();

                    while (inPacket.Remaining > 0)
                    {
                        entries.Add(inPacket.ReadBytes(inPacket.ReadByte()));
                    }

                    this.World.CharacterListPool.Enqueue(id, entries);
                    break;

                case InteroperabilityOperationCode.CharacterNameCheckResponse:
                    this.World.NameCheckPool.Enqueue(inPacket.ReadString(), inPacket.ReadBool());
                    break;

                case InteroperabilityOperationCode.CharacterCreationResponse:
                    this.World.CreatedCharacterPool.Enqueue(inPacket.ReadInt(), inPacket.ReadBytes());
                    break;

                case InteroperabilityOperationCode.LoadInformationResponse:
                    this.LoadPool.Enqueue(inPacket.ReadFloat());
                    break;

                case InteroperabilityOperationCode.LoggedInCheck:
                    this.LoggedInPool.Enqueue(inPacket.ReadInt(), inPacket.ReadBool());
                    break;

                case InteroperabilityOperationCode.ChannelPortRequest:
                    this.SendChannelPort(inPacket);
                    break;

                case InteroperabilityOperationCode.IsMasterCheck:
                    this.CheckIsMaster(inPacket);
                    break;

                case InteroperabilityOperationCode.GetCashRequest:
                    this.SendCash(inPacket);
                    break;

                case InteroperabilityOperationCode.SetCashRequest:
                    this.SetCash(inPacket);
                    break;
            }
        }

        public void HandleRegistrationRequest(Packet inPacket)
        {
            string securityCode = inPacket.ReadString();
            byte WorldID = inPacket.ReadByte();
            IPEndPoint endPoint = new IPEndPoint(inPacket.ReadIPAddress(), inPacket.ReadShort());

            bool worked = false;

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.RegistrationResponse))
            {
                if (securityCode != ChannelServerHandler.SecurityCode)
                {
                    outPacket.WriteByte((byte)ChannelRegistrationResponse.InvalidCode);
                    Log.Error(RegistrationResponseResolver.Explain(ChannelRegistrationResponse.InvalidCode));
                }
                else if (!WorldNameResolver.IsValid(WorldID))
                {
                    outPacket.WriteByte((byte)ChannelRegistrationResponse.InvalidWorld);
                    Log.Error(RegistrationResponseResolver.Explain(ChannelRegistrationResponse.InvalidWorld));
                }
                else if (!LoginServer.Worlds.Contains(WorldID) && LoginServer.Worlds.Count == 15)
                {
                    outPacket.WriteByte((byte)ChannelRegistrationResponse.WorldsFull);
                    Log.Error(RegistrationResponseResolver.Explain(ChannelRegistrationResponse.WorldsFull));
                }
                else
                {
                    if (!LoginServer.Worlds.Contains(WorldID))
                    {
                        LoginServer.Worlds.Add(new World(WorldID));
                    }

                    if (LoginServer.Worlds[WorldID].Count == 20)
                    {
                        outPacket.WriteByte((byte)ChannelRegistrationResponse.ChannelsFull);
                        Log.Error(RegistrationResponseResolver.Explain(ChannelRegistrationResponse.ChannelsFull));
                    }
                    else if (LoginServer.Worlds[WorldID].HostIP.ToString() != endPoint.Address.ToString())
                    {
                        outPacket.WriteByte((byte)ChannelRegistrationResponse.InvalidIP);
                        Log.Error(RegistrationResponseResolver.Explain(ChannelRegistrationResponse.InvalidIP));
                    }
                    else
                    {
                        this.RemoteEndPoint = endPoint;
                        this.WorldID = WorldID;

                        this.World.Add(this);
                        this.ID = (byte)this.World.Count;

                        outPacket.WriteByte((byte)ChannelRegistrationResponse.Valid);
                        outPacket.WriteInt(this.World.ExperienceRate);
                        outPacket.WriteInt(this.World.QuestExperienceRate);
                        outPacket.WriteInt(this.World.PartyQuestExperienceRate);
                        outPacket.WriteInt(this.World.MesoRate);
                        outPacket.WriteInt(this.World.DropRate);
                        outPacket.WriteByte(this.WorldID);
                        outPacket.WriteByte(this.ExternalID);

                        worked = true;
                    }
                }

                this.Send(outPacket);
            }

            if (worked)
            {
                Log.Success("Registered channel {0}-{1} at {2}.", LoginServer.Worlds[this.WorldID].Name, this.ExternalID, this.RemoteEndPoint);
            }
            else
            {
                Log.Warn("Channel server registration failed.");
                this.Stop();
            }
        }

        public void UpdateID(byte newID)
        {
            using (Packet outPacket = new Packet(InteroperabilityOperationCode.ChannelIDUpdate))
            {
                outPacket.WriteByte(newID);
                this.Send(outPacket);
            }
        }

        public void SendChannelPort(Packet inPacket)
        {
            byte channelID = inPacket.ReadByte();

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.ChannelPortResponse))
            {
                outPacket.WriteByte(channelID);
                outPacket.WriteShort((short)this.World[channelID].RemoteEndPoint.Port);

                this.Send(outPacket);
            }
        }

        public bool IsLoggedIn(int accountID)
        {
            using (Packet outPacket = new Packet(InteroperabilityOperationCode.LoggedInCheck))
            {
                outPacket.WriteInt(accountID);
                this.Send(outPacket);
            }

            return this.LoggedInPool.Dequeue(accountID);
        }

        public float LoadProportion
        {
            get
            {
                using (Packet outPacket = new Packet(InteroperabilityOperationCode.LoadInformationRequest))
                {
                    this.Send(outPacket);
                }

                return this.LoadPool.Dequeue();
            }
        }

        public void CheckIsMaster(Packet inPacket)
        {
            int accountID = inPacket.ReadInt();

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.IsMasterCheck))
            {
                outPacket.WriteInt(accountID);

                outPacket.WriteBool(Database.Fetch("accounts", "IsMaster", "ID = '{0}'", accountID));

                this.Send(outPacket);
            }
        }

        public void SendCash(Packet inPacket)
        {
            int accountID = inPacket.ReadInt();

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.GetCashResponse))
            {
                outPacket.WriteInt(accountID);

                switch (inPacket.ReadByte())
                {
                    case 1:
                        outPacket.WriteInt(Database.Fetch("accounts", "CardNX", "ID = '{0}'", accountID));
                        break;

                    case 2:
                        outPacket.WriteInt(Database.Fetch("accounts", "MaplePoints", "ID = '{0}'", accountID));
                        break;

                    case 3:
                        outPacket.WriteInt(Database.Fetch("accounts", "PaypalNX", "ID = '{0}'", accountID));
                        break;
                }

                this.Send(outPacket);
            }
        }

        public void SetCash(Packet inPacket)
        {
            int accountID = inPacket.ReadInt();
            dynamic datum = new Datum("accounts");

            switch (inPacket.ReadByte())
            {
                case 1:
                    datum.CardNX = inPacket.ReadInt();
                    break;

                case 2:
                    datum.MaplePoints = inPacket.ReadInt();
                    break;

                case 3:
                    datum.PaypalNX = inPacket.ReadInt();
                    break;
            }

            datum.Update("ID = '{0}'", accountID);
        }
    }
}
