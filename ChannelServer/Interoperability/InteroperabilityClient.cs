using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Loki.Collections;
using Loki.Data;
using Loki.IO;
using Loki.Maple;
using Loki.Maple.Characters;
using Loki.Net;
using Loki.Security;
using Loki.Maple.Data;

namespace Loki.Interoperability
{
    public class InteroperabilityClient : ServerHandler<InteroperabilityOperationCode, InteroperabilityOperationCode, BlankCryptograph>
    {
        public InteroperabilityClient(IPEndPoint remoteEP, string code, byte World) : base(remoteEP, "Login server", new object[] { code, World }) { }

        private ManualResetEvent ResponseReceived = new ManualResetEvent(false);
        private Packet ResponsePacket;

        protected override bool IsServerAlive
        {
            get
            {
                return ChannelServer.IsAlive;
            }
        }

        protected override void StopServer()
        {
            ChannelServer.Stop();
        }

        public static void Main()
        {
            try
            {
                ChannelServer.LoginServerConnection = new InteroperabilityClient(new IPEndPoint(
                        Settings.GetIPAddress("Login/IP"),
                        Settings.GetInt("Login/Port")),
                        Settings.GetString("Login/SecurityCode"),
                        WorldNameResolver.GetID(Settings.GetString("Server/World")));

                ChannelServer.LoginServerConnection.Loop();
            }
            catch (Exception e)
            {
                Log.Error("Server connection failed: \n{0}", e.Message);

                ChannelServer.Stop();
            }
        }

        protected override void Initialize(params object[] args)
        {
            using (Packet outPacket = new Packet(InteroperabilityOperationCode.RegistrationRequest))
            {
                outPacket.WriteString((string)args[0]);
                outPacket.WriteByte((byte)args[1]);
                outPacket.WriteBytes(ChannelServer.RemoteEndPoint.Address.GetAddressBytes());
                outPacket.WriteShort((short)ChannelServer.RemoteEndPoint.Port);

                this.Send(outPacket);
            }

            ByteBuffer buffer = new ByteBuffer();

            this.Socket.BeginReceive(buffer.Array, buffer.Position, buffer.Capacity, SocketFlags.None, new AsyncCallback(this.OnReceive), buffer);

            this.ResponseReceived.WaitOne();

            using (Packet inPacket = this.ResponsePacket)
            {
                ChannelRegistrationResponse outPacket = (ChannelRegistrationResponse)inPacket.ReadByte();

                switch (outPacket)
                {
                    case ChannelRegistrationResponse.Valid:
                        ChannelServer.ExperienceRate = inPacket.ReadInt();
                        ChannelServer.QuestExperienceRate = inPacket.ReadInt();
                        ChannelServer.PartyQuestExperienceRate = inPacket.ReadInt();
                        ChannelServer.MesoRate = inPacket.ReadInt();
                        ChannelServer.DropRate = inPacket.ReadInt();
                        ChannelServer.WorldID = inPacket.ReadByte();
                        ChannelServer.ChannelID = inPacket.ReadByte();

                        Log.Success("Registered channel as {0}-{1} at {2}.", WorldNameResolver.GetName(ChannelServer.WorldID), ChannelServer.ChannelID, ChannelServer.RemoteEndPoint);
                        Log.Inform("Rates: {0}x / {1}x / {2}x / {3}x / {4}x.",
                            ChannelServer.ExperienceRate,
                            ChannelServer.QuestExperienceRate,
                            ChannelServer.PartyQuestExperienceRate,
                            ChannelServer.MesoRate,
                            ChannelServer.DropRate);
                        break;

                    default:
                        throw new NetworkException(RegistrationResponseResolver.Explain(outPacket));
                }
            }
        }

        protected override void Dispatch(Packet inPacket)
        {
            switch ((InteroperabilityOperationCode)inPacket.OperationCode)
            {
                case InteroperabilityOperationCode.RegistrationResponse:
                    inPacket.Position = 0;
                    this.ResponsePacket = new Packet(inPacket.GetContent());
                    this.ResponseReceived.Set();
                    break;

                case InteroperabilityOperationCode.CharacterEntriesRequest:
                    this.SendCharacters(inPacket);
                    break;

                case InteroperabilityOperationCode.CharacterDeletionRequest:
                    this.DeleteCharacter(inPacket);
                    break;

                case InteroperabilityOperationCode.CharacterNameCheckRequest:
                    this.CheckCharacterName(inPacket);
                    break;

                case InteroperabilityOperationCode.CharacterCreationRequest:
                    this.CreateCharacter(inPacket);
                    break;

                case InteroperabilityOperationCode.LoggedInCheck:
                    this.CheckLogin(inPacket);
                    break;

                case InteroperabilityOperationCode.ChannelIDUpdate:
                    ChannelServer.ChannelID = inPacket.ReadByte();
                    break;

                case InteroperabilityOperationCode.ChannelPortResponse:
                    this.ChannelPortPool.Enqueue(inPacket.ReadByte(), inPacket.ReadShort());
                    break;

                case InteroperabilityOperationCode.LoadInformationRequest:
                    this.SendLoadInformation();
                    break;

                case InteroperabilityOperationCode.IsMasterCheck:
                    this.IsMasterPool.Enqueue(inPacket.ReadInt(), inPacket.ReadBool());
                    break;

                case InteroperabilityOperationCode.GetCashResponse:
                    this.CashPool.Enqueue(inPacket.ReadInt(), inPacket.ReadInt());
                    break;
            }
        }

        public void SendLoadInformation()
        {
            using (Packet outPacket = new Packet(InteroperabilityOperationCode.LoadInformationResponse))
            {
                outPacket.WriteFloat((float)ChannelServer.LoggedIn.Count / (float)ChannelServer.LoggedIn.Capacity);

                this.Send(outPacket);
            }
        }

        public void SendCharacters(Packet inPacket)
        {
            bool fromViewAll = inPacket.ReadBool();
            int accountID = inPacket.ReadInt();

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.CharacterEntriesResponse))
            {
                outPacket.WriteInt(accountID);

                foreach (dynamic datum in new Datums("characters").PopulateWith("ID", "AccountID = '{0}'", accountID))
                {
                    Character character = new Character(datum.ID);
                    character.Load();

                    byte[] entry = character.ToByteArray(fromViewAll);

                    outPacket.WriteByte((byte)entry.Length);
                    outPacket.WriteBytes(entry);
                }

                this.Send(outPacket);
            }
        }

        public void DeleteCharacter(Packet inPacket)
        {
            int characterID = inPacket.ReadInt();

            Character character = new Character(characterID);
            character.Delete();
        }

        public void CheckCharacterName(Packet inPacket)
        {
            string characterName = inPacket.ReadString();

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.CharacterNameCheckResponse))
            {
                outPacket.WriteString(characterName);
                outPacket.WriteBool(Database.Exists("characters", "Name = '{0}'", characterName));

                this.Send(outPacket);
            }
        }

        public void CreateCharacter(Packet inPacket)
        {
            int accountID = inPacket.ReadInt();
            bool isMaster = inPacket.ReadBool();

            string name = inPacket.ReadString();
            int job = inPacket.ReadInt();
            short specialJob = inPacket.ReadShort();
            byte gender = inPacket.ReadByte();
            inPacket.ReadShort();
            int face = inPacket.ReadInt();
            int hair = inPacket.ReadInt();
            int hair_color = 0;
            byte skin = 0;

            if (job < 5)
            {
                hair_color = inPacket.ReadInt();
            }

            if (job == 6)
            {
                skin = 13;
            }
            else if (job == 5)
            {
                skin = 12;
            }
            else
            {
                skin = (byte)inPacket.ReadInt();
            }

            int demonMark = job == 6 ? inPacket.ReadInt() : 0;
            int topID = inPacket.ReadInt();

            int bottomID = 0;
            if (job < 5)
            {
                bottomID = inPacket.ReadInt();
            }

            int shoesID = inPacket.ReadInt();
            int weaponID = inPacket.ReadInt();
            int shieldID = job == 6 ? inPacket.ReadInt() : 0;

            Character character = new Character();

            character.AccountID = accountID;

            character.Level = 1;
            if (job == -1) // UA
            {
                // TODO: Handle UA
            }
            else if (job == 0) // Resistance
            {
                character.Job = Job.Citizen;
            }
            else if (job == 1) // Adventurer
            {
                character.Job = Job.Beginner;
            }
            else if (job == 2) // Cygnus
            {
                character.Job = Job.Noblesse;
            }
            else if (job == 3) // Aran
            {
                character.Job = Job.Legend;
            }
            else if (job == 4) // Evan
            {
                character.Job = Job.Farmer;
            }
            else if (job == 5) // Mercedes
            {
                character.Job = Job.Mercedes;
            }
            else if (job == 6) // DemonSlayer
            {
                character.Job = Job.DemonSlayer;
            }

            character.MaxHP = 50;
            character.MaxMP = 5;
            character.CurrentHP = 50;
            character.CurrentMP = 5;
            character.Strength = 12;
            character.Dexterity = 5;
            character.Intelligence = 4;
            character.Luck = 4;
            character.Meso = 0;
            character.Fame = 0;
            character.AvailableAP = 0;
            character.AvailableSP = 0;
            character.SpawnPoint = 0;
            character.MaxBuddies = 20;
            character.SpecialJob = (SpecialJob)specialJob;

            character.Name = name;
            character.Face = face;
            character.Hair = hair + hair_color;
            character.Skin = skin;
            character.Gender = (Gender)gender;
            character.DemonMark = demonMark;

            if (job < 5 && job != 0)
            {
                character.Items.Add(new Item(topID, equipped: true));
                character.Items.Add(new Item(bottomID, equipped: true));
                character.Items.Add(new Item(shoesID, equipped: true));
                character.Items.Add(new Item(weaponID, equipped: true));
            }
            else
            {
                character.Items.Add(new Item(topID, equipped: true));
                character.Items.Add(new Item(shoesID, equipped: true));
                character.Items.Add(new Item(weaponID, equipped: true));

                if (job == 6)
                {
                    character.Items.Add(new Item(shieldID, equipped: true));
                }
            }

            int[] key = { 18, 65, 2, 23, 3, 4, 5, 6, 16, 17, 19, 25, 26, 27, 31, 34, 35, 37, 38, 40, 43, 44, 45, 46, 50, 56, 59, 60, 61, 62, 63, 64, 57, 48, 29, 7, 24, 33, 41 };
            byte[] type = { 4, 6, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 4, 4, 5, 6, 6, 6, 6, 6, 6, 5, 4, 5, 4, 4, 4, 4 };
            int[] action = { 0, 106, 10, 1, 12, 13, 18, 24, 8, 5, 4, 19, 14, 15, 2, 17, 11, 3, 20, 16, 9, 50, 51, 6, 7, 53, 100, 101, 102, 103, 104, 105, 54, 22, 52, 21, 25, 26, 23 };

            for (int i = 0; i < key.Length; i++)
            {
                character.KeyMap.Add(key[i], new Shortcut(type[i], action[i]));
            }

            bool charOk = true;

            if (Database.Exists("characters", "Name = '{0}'", name) || (World.ForbiddenNames.Contains(name) && !isMaster))
                charOk = false;

            //if (!World.CharacterCreationData.checkData(job, gender, face, hair, hair_color, skin, topID, bottomID, shoesID, weaponID))
              //  charOk = false;

            if (charOk)
            {
                character.Save();

                using (Packet outPacket = new Packet(InteroperabilityOperationCode.CharacterCreationResponse))
                {
                    outPacket.WriteInt(accountID);
                    outPacket.WriteBytes(character.ToByteArray(false));

                    this.Send(outPacket);
                }
            }
            else
            {
                using (Packet outPacket = new Packet(InteroperabilityOperationCode.CharacterCreationResponse))
                {
                    outPacket.WriteInt(accountID);
                    outPacket.WriteByte();

                    this.Send(outPacket);
                }
            }
        }

        public void CheckLogin(Packet inPacket)
        {
            int accountID = inPacket.ReadInt();

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.LoggedInCheck))
            {
                outPacket.WriteInt(accountID);
                outPacket.WriteBool(ChannelServer.LoggedIn.Contains(accountID));

                this.Send(outPacket);
            }
        }

        private PendingKeyedQueue<byte, short> ChannelPortPool = new PendingKeyedQueue<byte, short>();

        public short GetChannelPort(byte channelID)
        {
            using (Packet outPacket = new Packet(InteroperabilityOperationCode.ChannelPortRequest))
            {
                outPacket.WriteByte(channelID);
                this.Send(outPacket);
            }

            return this.ChannelPortPool.Dequeue(channelID);
        }

        private PendingKeyedQueue<int, bool> IsMasterPool = new PendingKeyedQueue<int, bool>();

        public bool IsMaster(int accountID)
        {
            using (Packet outPacket = new Packet(InteroperabilityOperationCode.IsMasterCheck))
            {
                outPacket.WriteInt(accountID);
                this.Send(outPacket);
            }

            return this.IsMasterPool.Dequeue(accountID);
        }

        private PendingKeyedQueue<int, int> CashPool = new PendingKeyedQueue<int, int>();

        public int GetCash(int accountID, byte type)
        {
            using (Packet outPacket = new Packet(InteroperabilityOperationCode.GetCashRequest))
            {
                outPacket.WriteInt(accountID);
                outPacket.WriteByte(type);
                this.Send(outPacket);
            }

            return this.CashPool.Dequeue(accountID);
        }

        public void SetCash(int accountID, byte type, int cash)
        {
            using (Packet outPacket = new Packet(InteroperabilityOperationCode.SetCashRequest))
            {
                outPacket.WriteInt(accountID);
                outPacket.WriteByte(type);
                outPacket.WriteInt(cash);
                this.Send(outPacket);
            }
        }
    }
}
