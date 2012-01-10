using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Loki.Data;
using Loki.Interoperability;
using Loki.Maple;
using Loki.Security;

namespace Loki.Net
{
    public class LoginClientHandler : MapleClientHandler
    {
        public byte WorldID { get; private set; }
        public byte ChannelID { get; private set; }
        public Account Account { get; private set; }
        public string LastUsername { get; private set; }
        public string LastPassword { get; private set; }
        public string[] MacAddresses { get; private set; }

        public World World
        {
            get
            {
                return LoginServer.Worlds[this.WorldID];
            }
            set
            {
                this.WorldID = value.ID;
            }
        }

        public ChannelServerHandler Channel
        {
            get
            {
                return this.World[this.ChannelID];
            }
            set
            {
                this.ChannelID = value.InternalID;
            }
        }

        public LoginClientHandler(Socket socket) : base(socket) { }

        protected override void Register()
        {
            LoginServer.Clients.Add(this);
        }

        protected override void Terminate()
        {
            if (this.Account != null)
            {
                this.Account.IsLoggedIn = false;
                this.Account.Save();
            }
        }

        protected override void Unregister()
        {
            LoginServer.Clients.Remove(this);
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
            switch ((MapleClientOperationCode)inPacket.OperationCode)
            {
                case MapleClientOperationCode.Login:
                    this.Login(inPacket);
                    break;

                case MapleClientOperationCode.AfterLogin:
                    this.CheckPin(inPacket);
                    break;

                case MapleClientOperationCode.RegisterPin:
                    this.RegisterPin(inPacket);
                    break;

                case MapleClientOperationCode.ServerList:
                case MapleClientOperationCode.ServerRelist:
                    this.ListServers();
                    break;

                case MapleClientOperationCode.ServerStatus:
                    this.InformStatus(inPacket);
                    break;

                case MapleClientOperationCode.CharacterList:
                    this.ListCharacters(inPacket);
                    break;

                case MapleClientOperationCode.ViewAllCharacters:
                    this.ListAllCharacters();
                    break;

                case MapleClientOperationCode.DeleteCharacter:
                    this.DeleteCharacter(inPacket);
                    break;

                case MapleClientOperationCode.EnterExitViewAll:
                    // Only a log packet.
                    break;

                case MapleClientOperationCode.CheckCharacterName:
                    this.CheckName(inPacket);
                    break;

                case MapleClientOperationCode.CreateCharacter:
                    this.CreateCharacter(inPacket);
                    break;

                case MapleClientOperationCode.CharacterSelect:
                    this.SelectCharacter(inPacket, false);
                    break;

                case MapleClientOperationCode.CharacterSelectFromViewAll:
                    this.SelectCharacter(inPacket, true);
                    break;

                case MapleClientOperationCode.Relog:
                    this.Relog();
                    break;

                case MapleClientOperationCode.RegisterPic:
                    this.RegisterPic(inPacket);
                    break;

                case MapleClientOperationCode.SelectCharacterWithPic:
                    this.SelectCharacterWithPic(inPacket);
                    break;

                case MapleClientOperationCode.ClientStartError:
                    this.ClientError(inPacket);
                    break;

                case MapleClientOperationCode.ClientError:
                    this.ClientError(inPacket);
                    break;
            }
        }

        private void RespondPin(PinResponse inPacket)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.PinOperation))
            {
                outPacket.WriteByte((byte)inPacket);

                this.Send(outPacket);
            }
        }

        private void RespondLogin(LoginResponse inPacket)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.Login))
            {
                if (inPacket == LoginResponse.Valid)
                {
                    outPacket.WriteInt();
                    outPacket.WriteShort();
                    outPacket.WriteInt(this.Account.ID);
                    outPacket.WriteByte(/*0x0A*/); // OBSOLETE: If 0x0A, request gender.
                    outPacket.WriteBool(this.Account.IsMaster); // NOTE: Disables trade, enables admin commands.
                    outPacket.WriteByte();
                    outPacket.WriteByte();
                    outPacket.WriteString(this.Account.Username);
                    outPacket.WriteByte();
                    outPacket.WriteBool(false); // OBSOLETE: Quiet ban.
                    outPacket.WriteLong();
                    outPacket.WriteDateTime(this.Account.Creation);
                    outPacket.WriteInt();
                    outPacket.WriteByte(0); // pin 0 = Enable, 1 = Disable
                    outPacket.WriteByte(2); // pic 0 = Register, 1 = Request, 2 = Disable
                }
                else
                {
                    outPacket.WriteInt((int)inPacket);
                    outPacket.WriteShort();
                }

                this.Send(outPacket);
            }
        }

        private void Login(Packet inPacket)
        {
            string username = inPacket.ReadString();
            string password = inPacket.ReadString();

            if (!username.IsAlphaNumeric())
            {
                this.RespondLogin(LoginResponse.NotRegistered);
            }
            else
            {
                this.Account = new Account(this);

                try
                {
                    this.Account.Load(username);

                    if (ShaCryptograph.Encrypt(ShaMode.SHA512, password + this.Account.Salt) != this.Account.Password)
                    {
                        this.RespondLogin(LoginResponse.IncorrectPassword);
                    }
                    else if (this.Account.IsBanned || Database.Exists("banned_ip", "Address = '{0}'", this.RemoteEndPoint.Address))
                    {
                        this.RespondLogin(LoginResponse.Banned);
                    }
                    else if (this.Account.IsLoggedIn)
                    {
                        this.RespondLogin(LoginResponse.AlreadyLoggedIn);
                    }
                    else
                    {
                        if (this.Account.IsMaster && LoginServer.RequireStaffIP && !Database.Exists("master_ip", "IP = '{0}'", this.RemoteEndPoint.Address))
                        {
                            this.RespondLogin(LoginResponse.NotMasterIP);
                        }
                        else
                        {
                            this.RespondLogin(LoginResponse.Valid);
                        }
                    }
                }
                catch (NoAccountException)
                {
                    if (LoginServer.AutoRegister && username == this.LastUsername && password == this.LastPassword)
                    {
                        this.Account.Username = username;
                        this.Account.Salt = HashGenerator.GenerateMD5();
                        this.Account.Password = ShaCryptograph.Encrypt(ShaMode.SHA512, password + this.Account.Salt);
                        this.Account.Birthday = DateTime.UtcNow;
                        this.Account.Creation = DateTime.UtcNow;
                        this.Account.IsBanned = false;
                        this.Account.IsMaster = false;
                        this.Account.Pin = string.Empty;
                        this.Account.Pic = string.Empty;
                        this.Account.MaplePoints = 0;
                        this.Account.PaypalNX = 0;
                        this.Account.CardNX = 0;

                        this.Account.Save();

                        this.RespondLogin(LoginResponse.Valid);
                    }
                    else
                    {
                        this.RespondLogin(LoginResponse.NotRegistered);

                        this.LastUsername = username;
                        this.LastPassword = password;
                    }
                }
            }
        }

        private void CheckPin(Packet inPacket)
        {
            // TODO: Figure any logic in the values or names of alpha/beta.

            byte alpha = inPacket.ReadByte();
            byte beta = 0;

            if (inPacket.Remaining > 0)
            {
                beta = inPacket.ReadByte();
            }

            if (alpha == 1 && beta == 1) // Request login.
            {
                if (LoginServer.RequestPin)
                {
                    if (this.Account.Pin == string.Empty)
                    {
                        this.RespondPin(PinResponse.Register);
                    }
                    else
                    {
                        this.RespondPin(PinResponse.Request);
                    }
                }
                else
                {
                    this.Account.IsLoggedIn = true;
                    this.RespondPin(PinResponse.Valid);
                }
            }
            else if (beta == 0)
            {
                inPacket.Position = 4;

                if (alpha != 0) // Not canceled.
                {
                    if (ShaCryptograph.Encrypt(ShaMode.SHA256, inPacket.ReadString()) != this.Account.Pin)
                    {
                        this.RespondPin(PinResponse.Invalid);
                    }
                    else
                    {
                        if (alpha == 1) // Request pin validation.
                        {
                            this.Account.IsLoggedIn = true;
                            this.RespondPin(PinResponse.Valid);
                        }
                        else if (alpha == 2) // Request new pin registration.
                        {
                            this.RespondPin(PinResponse.Register);
                        }
                        else
                        {
                            this.RespondPin(PinResponse.Error);
                        }
                    }
                }
            }
            else
            {
                this.RespondPin(PinResponse.Error);
            }
        }

        private void RegisterPin(Packet inPacket)
        {
            byte operation = inPacket.ReadByte();
            string pin = inPacket.ReadString();

            if (operation != 0) // Not canceled. // TODO: Check if operation could be bool continue.
            {
                this.Account.Pin = ShaCryptograph.Encrypt(ShaMode.SHA256, pin);
                this.Account.Save();

                using (Packet outPacket = new Packet(MapleServerOperationCode.PinAssigned))
                {
                    outPacket.WriteByte();
                    this.Send(outPacket);
                }
            }
            else
            {
                this.RespondPin(PinResponse.Error);
            }
        }

        private void RegisterPic(Packet inPacket)
        {
            inPacket.ReadByte();
            int charId = inPacket.ReadInt();
            inPacket.ReadString();
            inPacket.ReadString();
            string pic = inPacket.ReadString();
            if (this.Account.Pic == null || this.Account.Pic == "")
            {
                this.Account.Pic = pic;
                this.Account.Save();
                using (Packet outPacket = new Packet(MapleServerOperationCode.ServerIP))
                {
                    outPacket.WriteShort();
                    outPacket.WriteIPAddress(this.Channel.RemoteEndPoint.Address);
                    outPacket.WriteShort((short)this.Channel.RemoteEndPoint.Port);
                    outPacket.WriteInt(charId);
                    outPacket.WriteInt();
                    outPacket.WriteByte();

                    this.Send(outPacket);
                }
            }
            else
            {
                this.Dispose();
            }
        }

        private void ListServers()
        {
            foreach (World loopWorld in LoginServer.Worlds)
            {
                using (Packet outPacket = new Packet(MapleServerOperationCode.ServerList))
                {
                    outPacket.WriteByte(loopWorld.ID);
                    outPacket.WriteString(loopWorld.Name);
                    outPacket.WriteByte((byte)loopWorld.Flag);
                    outPacket.WriteString(loopWorld.EventMessage);
                    outPacket.WriteShort(100); // UNK: Rate modifier?
                    outPacket.WriteShort(100); // UNK: Rate modifier?
                    outPacket.WriteByte();
                    outPacket.WriteByte((byte)loopWorld.Count);

                    // TODO: Must be something wrong here, the status thing.

                    foreach (ChannelServerHandler loopChannel in loopWorld)
                    {
                        outPacket.WriteString("{0}-{1}", loopWorld.Name, loopChannel.ExternalID);
                        outPacket.WriteInt((int)(loopChannel.LoadProportion * 800));
                        outPacket.WriteByte(1); // UNK.
                        outPacket.WriteShort((short)(loopChannel.InternalID));
                    }

                    outPacket.WriteShort();

                    this.Send(outPacket);
                }
            }

            using (Packet outPacket = new Packet(MapleServerOperationCode.ServerList))
            {
                outPacket.WriteByte(byte.MaxValue);
                this.Send(outPacket);
            }
        }

        private void InformStatus(Packet inPacket)
        {
            byte WorldID = inPacket.ReadByte();

            using (Packet outPacket = new Packet(MapleServerOperationCode.ServerStatus))
            {
                if (LoginServer.Worlds[WorldID].IsStaffOnly && !this.Account.IsMaster)
                {
                    outPacket.WriteShort((short)ServerStatus.Full);
                }
                else
                {
                    outPacket.WriteShort((short)LoginServer.Worlds[WorldID].Status);
                }

                this.Send(outPacket);
            }
        }

        private void ListCharacters(Packet inPacket)
        {
            inPacket.ReadByte();

            this.WorldID = inPacket.ReadByte();
            this.ChannelID = inPacket.ReadByte();

            List<byte[]> WorldCharacters = this.World.GetCharacters(this.Account.ID);

            using (Packet outPacket = new Packet(MapleServerOperationCode.CharacterList))
            {
                outPacket.WriteBool(false); // Not all characters.
                outPacket.WriteByte((byte)WorldCharacters.Count);

                foreach (byte[] characterBytes in WorldCharacters)
                {
                    outPacket.WriteBytes(characterBytes);
                }

                outPacket.WriteByte((byte)(this.Account.Pic == null || this.Account.Pic == "" ? 0 : 1));
                outPacket.WriteShort(1);
                outPacket.WriteInt(LoginServer.MaxCharacters);

                this.Send(outPacket);
            }
        }

        private void ListAllCharacters()
        {
            Dictionary<byte, List<byte[]>> allCharacters = new Dictionary<byte, List<byte[]>>();

            int count = 0;

            foreach (World loopWorld in LoginServer.Worlds)
            {
                List<byte[]> WorldCharacters = loopWorld.GetCharacters(this.Account.ID);
                count += WorldCharacters.Count;

                allCharacters.Add(loopWorld.ID, WorldCharacters);
            }

            using (Packet outPacket = new Packet(MapleServerOperationCode.AllCharactersList))
            {
                outPacket.WriteBool(true); // All characters.
                outPacket.WriteInt(count);
                outPacket.WriteInt(count + 3 - count % 3);

                this.Send(outPacket);
            }

            foreach (KeyValuePair<byte, List<byte[]>> loopPair in allCharacters)
            {
                using (Packet outPacket = new Packet(MapleServerOperationCode.AllCharactersList))
                {
                    outPacket.WriteByte();
                    outPacket.WriteByte(loopPair.Key);
                    outPacket.WriteByte((byte)loopPair.Value.Count);

                    foreach (byte[] characterBytes in loopPair.Value)
                    {
                        outPacket.WriteBytes(characterBytes);
                    }

                    this.Send(outPacket);
                }
            }
        }

        private void DeleteCharacter(Packet inPacket)
        {
            int intDate = inPacket.ReadInt();
            int characterID = inPacket.ReadInt();

            int inputYear = intDate / 10000;
            int inputMonth = (intDate - inputYear * 10000) / 100;
            int inputDay = intDate - inputYear * 10000 - (inputMonth * 100);

            string inputDate = string.Format("{0}-{1}-{2}", inputYear, inputMonth, inputDay);
            string referenceDate = this.Account.Birthday.ToString("yyyy'-'MM'-'DD");

            using (Packet outPacket = new Packet(MapleServerOperationCode.DeleteCharacter))
            {
                outPacket.WriteInt(characterID);

                if (inputDate == referenceDate || !LoginServer.RequestDeletionBirthday)
                {
                    this.World.DeleteCharacter(characterID);
                    outPacket.WriteByte((byte)CharacterDeletionResponse.Valid);
                }
                else
                {
                    outPacket.WriteByte((byte)CharacterDeletionResponse.Invalid);
                }

                this.Send(outPacket);
            }
        }

        private void CheckName(Packet inPacket)
        {
            string name = inPacket.ReadString();

            using (Packet outPacket = new Packet(MapleServerOperationCode.CharacterName))
            {
                outPacket.WriteString(name);
                outPacket.WriteBool(this.World.IsNameTaken(name));

                this.Send(outPacket);
            }
        }

        private void CreateCharacter(Packet inPacket)
        {
            byte[] characterData = inPacket.ReadBytes();

            using (Packet outPacket = new Packet(MapleServerOperationCode.AddNewCharacterEntry))
            {
                outPacket.WriteByte(); // NOTE: 1 for failure. Could be implemented as anti-packet editing.
                outPacket.WriteBytes(this.World.CreateCharacter(this.Account.ID, characterData));

                this.Send(outPacket);
            }
        }

        private void SelectCharacter(Packet inPacket, bool fromViewAll)
        {
            int characterID = inPacket.ReadInt();

            this.MacAddresses = inPacket.ReadString().Split(new char[] { ',', ' ' });

            bool isMacBanned = false;

            foreach (string loopMac in this.MacAddresses)
            {
                if (Database.Exists("banned_mac", "Address = '{0}'", loopMac))
                {
                    Log.Warn("Disconnecting banned MAC address {0}.", loopMac);
                    isMacBanned = true;
                    break;
                }
            }

            if (isMacBanned)
            {
                this.Stop();
            }
            else
            {
                if (fromViewAll)
                {
                    this.WorldID = (byte)inPacket.ReadInt();
                    this.Channel = this.World.LeastLoadedChannel;
                }

                using (Packet outPacket = new Packet(MapleServerOperationCode.ServerIP))
                {
                    outPacket.WriteShort();
                    outPacket.WriteIPAddress(this.Channel.RemoteEndPoint.Address);
                    outPacket.WriteShort((short)this.Channel.RemoteEndPoint.Port);
                    outPacket.WriteInt(characterID);
                    outPacket.WriteInt();
                    outPacket.WriteByte();

                    this.Send(outPacket);
                }
            }
        }

        private void Relog()
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.RelogResponse))
            {
                outPacket.WriteBool(true);
                this.Send(outPacket);
            }
        }

        private void SelectCharacterWithPic(Packet inPacket)
        {
            string pic = inPacket.ReadString();
            int charId = inPacket.ReadInt();
            inPacket.ReadString();
            if (this.Account.Pic.Equals(pic))
            {
                using (Packet outPacket = new Packet(MapleServerOperationCode.ServerIP))
                {
                    outPacket.WriteShort();
                    outPacket.WriteIPAddress(this.Channel.RemoteEndPoint.Address);
                    outPacket.WriteShort((short)this.Channel.RemoteEndPoint.Port);
                    outPacket.WriteInt(charId);
                    outPacket.WriteInt();
                    outPacket.WriteByte();

                    this.Send(outPacket);
                }
            }
            else
            {
                using (Packet outPacket = new Packet(MapleServerOperationCode.WrongPic))
                {
                    outPacket.WriteByte(20);

                    this.Send(outPacket);
                }
            }
        }

        private void ClientError(Packet inPacket)
        {
            Log.Warn(inPacket.ReadString());
        }
    }
}
