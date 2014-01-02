using System;
using System.Collections.Generic;
using System.Net;
using Loki.Collections;
using Loki.IO;
using Loki.Maple;
using Loki.Net;

namespace Loki.Interoperability
{
    public class World : DynamicKeyedCollection<byte, ChannelServerHandler>
    {
        public byte ID { get; private set; }
        public IPAddress HostIP { get; private set; }
        public string RecommendedMessage { get; private set; }
        public int ExperienceRate { get; private set; }
        public int QuestExperienceRate { get; private set; }
        public int PartyQuestExperienceRate { get; private set; }
        public int MesoRate { get; private set; }
        public int DropRate { get; private set; }
        public bool IsStaffOnly { get; private set; }
        public ServerFlag Flag { get; private set; }

        public PendingKeyedQueue<int, List<byte[]>> CharacterListPool = new PendingKeyedQueue<int, List<byte[]>>();
        public PendingKeyedQueue<string, bool> NameCheckPool = new PendingKeyedQueue<string, bool>();
        public PendingKeyedQueue<int, byte[]> CreatedCharacterPool = new PendingKeyedQueue<int, byte[]>();
        public Dictionary<byte, List<int>> CharacterStorage = new Dictionary<byte,List<int>>();

        public string Name
        {
            get
            {
                return WorldNameResolver.GetName(this.ID);
            }
        }

        public string EventMessage
        {
            get
            {
                return string.Format("Welcome to the{4}{0} server!{5}\r\n\r\nExperience: {1}x\r\nMesos: {2}x\r\n Items: {3}x",
                   this.Name,
                   this.ExperienceRate,
                   this.MesoRate,
                   this.DropRate,
                   this.Flag == ServerFlag.New ? " new " : " ",
                   this.IsStaffOnly ? "\r\n[Staff Only]" : null);
            }
        }

        public ServerStatus Status
        {
            get
            {
                return ServerStatus.Normal; // NOTE: Unless someone wants to impose a maximum registered users, this is useless.
            }
        }

        public ChannelServerHandler RandomChannel
        {
            get
            {
                return this[Application.Random.Next(this.Count)];
            }
        }

        public ChannelServerHandler LeastLoadedChannel
        {
            get
            {
                MultiThreadedDictionary<byte, float, byte> dictionary = new MultiThreadedDictionary<byte, float, byte>(this.Count);

                foreach (ChannelServerHandler loopChannel in this)
                {
                    dictionary.AddFromThread(new Func<byte, float>(this.GetChannelLoad), loopChannel.InternalID, loopChannel.InternalID);
                }

                dictionary.WaitUntilDone();

                float minimumValue = float.MaxValue;
                byte minimumID = 0;

                foreach (KeyValuePair<byte, float> pair in dictionary)
                {
                    if (pair.Value < minimumValue)
                    {
                        minimumID = pair.Key;
                    }
                }

                return this[minimumID];
            }
        }

        public World(byte WorldID)
        {
            this.ID = WorldID;

            Settings.Refresh();

            this.HostIP = Settings.GetIPAddress("{0}/HostIP", this.Name);
            this.RecommendedMessage = Settings.GetString("{0}/RecommendedMessage", this.Name);
            this.IsStaffOnly = Settings.GetBool("{0}/StaffOnly", this.Name);
            this.Flag = Settings.GetEnum<ServerFlag>("{0}/Flag", this.Name);
            this.ExperienceRate = Settings.GetInt("{0}/ExperienceRate", this.Name);
            this.QuestExperienceRate = Settings.GetInt("{0}/QuestExperienceRate", this.Name);
            this.PartyQuestExperienceRate = Settings.GetInt("{0}/PartyQuestExperienceRate", this.Name);
            this.MesoRate = Settings.GetInt("{0}/MesoDropRate", this.Name);
            this.DropRate = Settings.GetInt("{0}/ItemDropRate", this.Name);
        }

        protected override byte GetKeyForItem(ChannelServerHandler item)
        {
            return item.InternalID;
        }

        private float GetChannelLoad(byte internalChannelID)
        {
            return this[internalChannelID].LoadProportion;
        }

        public List<byte[]> GetCharacters(int accountID, bool fromViewAll)
        {
            using (Packet outPacket = new Packet(InteroperabilityOperationCode.CharacterEntriesRequest))
            {
                outPacket.WriteBool(fromViewAll);
                outPacket.WriteInt(accountID);

                this.RandomChannel.Send(outPacket);
            }

            return this.CharacterListPool.Dequeue(accountID);
        }

        public void DeleteCharacter(int characterID)
        {
            using (Packet outPacket = new Packet(InteroperabilityOperationCode.CharacterDeletionRequest))
            {
                outPacket.WriteInt(characterID);

                this.RandomChannel.Send(outPacket);
            }
        }

        public bool IsNameTaken(string characterName)
        {
            using (Packet outPacket = new Packet(InteroperabilityOperationCode.CharacterNameCheckRequest))
            {
                outPacket.WriteString(characterName);

                this.RandomChannel.Send(outPacket);
            }

            return this.NameCheckPool.Dequeue(characterName);
        }

        public byte[] CreateCharacter(int accountID, byte[] characterData, bool isMaster)
        {
            using (Packet outPacket = new Packet(InteroperabilityOperationCode.CharacterCreationRequest))
            {
                outPacket.WriteInt(accountID);
                outPacket.WriteBool(isMaster);
                outPacket.WriteBytes(characterData);

                this.RandomChannel.Send(outPacket);
            }

            return this.CreatedCharacterPool.Dequeue(accountID);
        }

        public void CharacterWorldInteraction(Packet inPacket)
        {
            string characterName;
            byte channel;

            switch ((CharacterWorldInteractionAction)inPacket.ReadByte())
            {
                case CharacterWorldInteractionAction.UpdateBuddies:
                    characterName = inPacket.ReadString();
                    channel = inPacket.ReadByte();
                    int buddyID;

                    while (inPacket.Remaining > 0)
                    {
                        buddyID = inPacket.ReadInt();

                        foreach (byte loopChannel in this.CharacterStorage.Keys)
                        {
                            if (this.CharacterStorage[loopChannel].Contains(buddyID))
                            {
                                this[loopChannel].UpdateBuddy(buddyID, characterName, channel);
                            }
                        }
                    }

                    break;

                case CharacterWorldInteractionAction.SendBuddyRequest:
                    this[inPacket.ReadByte()].SendBuddyRequest(inPacket);
                    break;
            }
        }

        public void GetBuddyAddResult(Packet inPacket)
        {
            byte addBuddyChannel = inPacket.ReadByte();
            int addBuddyID = inPacket.ReadInt();
            int characterID = inPacket.ReadInt();
            byte characterChannel = inPacket.ReadByte();

            BuddyAddResult result = this[addBuddyChannel].RequestBuddyAddResult(addBuddyID, characterID);

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.BuddyAddResultResponse))
            {
                outPacket.WriteInt(characterID);
                outPacket.WriteByte((byte)result);

                this[characterChannel].Send(outPacket);
            }
        }
    }
}
