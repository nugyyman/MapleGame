using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Net;
using Loki.Data;
using Loki.Maple.Data;
using System.Collections.ObjectModel;

namespace Loki.Maple.Characters
{
    public class CharacterBuddyList : KeyedCollection<int, Buddy>
    {
        public Character Parent { get; private set; }
        public List<Buddy> pendingBuddies = new List<Buddy>();

        public CharacterBuddyList(Character parent)
            : base()
        {
            this.Parent = parent;
        }

        public bool IsFull
        {
            get
            {
                return this.Count >= this.Parent.MaxBuddies;
            }
        }

        public void Load()
        {
            string name;

            foreach (dynamic datum in new Datums("buddies").Populate("CharacterID = '{0}'", this.Parent.ID))
            {
                name = Database.Fetch("characters", "Name", "ID = '{0}'", datum.BuddyID);

                if (datum.Pending == 0)
                {
                    this.Add(new Buddy(name, datum.GroupName, datum.BuddyID, 0, true));
                }
                else
                {
                    this.pendingBuddies.Add(new Buddy(name, datum.GroupName, datum.BuddyID, 0, false));
                }

                Database.Delete("buddies", "CharacterID = '{0}' AND Pending = '1'", this.Parent.ID);
            }

            Dictionary<byte, List<int>> characterStorage = ChannelData.GetCharacterStorage(this.Parent.ID);

            foreach (Buddy loopBuddy in this)
            {
                if (this.IsMutualBuddy(loopBuddy.CharacterID))
                {
                    foreach (byte loopChannel in characterStorage.Keys)
                    {
                        if (characterStorage[loopChannel].Contains(loopBuddy.CharacterID))
                        {
                            loopBuddy.Channel = (byte)(loopChannel + 1);
                        }
                    }
                }
            }
        }

        public void Delete()
        {
            Database.Delete("buddies", "CharacterID = '{0}'", this.Parent.ID);
        }

        public void DeleteBuddy(int buddyID)
        {
            Database.Delete("buddies", "CharacterID = '{0}' AND BuddyID = '{1}'", this.Parent.ID, buddyID);
        }

        public void Save()
        {
            foreach (Buddy loopBuddy in this)
            {
                dynamic datum = new Datum("buddies");

                datum.CharacterID = this.Parent.ID;
                datum.BuddyID = loopBuddy.CharacterID;
                datum.Pending = 0;
                datum.GroupName = loopBuddy.Group;

                if (loopBuddy.Assigned)
                {
                    datum.Update("CharacterID = '{0}' AND BuddyID = '{1}'", this.Parent.ID, loopBuddy.CharacterID);
                }
                else
                {
                    datum.Insert();
                }
            }
        }

        public void Update(byte operation)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.BuddyList))
            {
                outPacket.WriteByte(operation);
                outPacket.WriteByte((byte)this.Count);

                foreach (Buddy buddy in this)
                {
                    outPacket.WriteInt(buddy.CharacterID);
                    outPacket.WriteStringFixed(buddy.Name, 13);
                    outPacket.WriteByte();
                    outPacket.WriteInt(buddy.Channel - 1);
                    outPacket.WriteStringFixed(buddy.Group, 17);
                }

                for (int i = 0; i < this.Count; i++)
                    outPacket.WriteInt();

                this.Parent.Client.Send(outPacket);
            }
        }

        public void UpdateBuddyChannel(Buddy buddy)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.BuddyList))
            {
                outPacket.WriteByte(0x14);
                outPacket.WriteInt(buddy.CharacterID);
                outPacket.WriteByte();
                outPacket.WriteInt(buddy.Channel - 1);


                this.Parent.Client.Send(outPacket);
            }
        }

        public void UpdateCapacity(byte capacity)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.BuddyList))
            {
                outPacket.WriteByte(0x15);
                outPacket.WriteByte(capacity);

                this.Parent.Client.Send(outPacket);
            }
        }

        /// <param name="message">
        /// 11: Your buddy list is full.
        /// 12: The user's buddy list is full
        /// 13: That character is already registered as your buddy.
        /// 14: Gamemaster is not available as a buddy.
        /// 15: That character is not registered.
        /// 23: You've already made the Friend Request. Please try again later.
        /// </param>
        public void SendMessage(byte message)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.BuddyList))
            {
                outPacket.WriteByte(message);

                this.Parent.Client.Send(outPacket);
            }
        }

        public Buddy this[string name]
        {
            get
            {
                foreach (Buddy buddy in this)
                {
                    if (buddy.Name.ToLower().Equals(name.ToLower()))
                        return buddy;
                }

                return null;
            }
        }

        public bool IsMutualBuddy(int buddyID)
        {
            if (this[buddyID].IsOnline)
            {
                if (ChannelServer.LoginServerConnection.RequestBuddyAddResult(buddyID, (byte)(this[buddyID].Channel - 1), this.Parent.ID) == BuddyAddResult.AlreadyOnList)
                {
                    return true;
                }

                return false;
            }
            else
            {
                return Database.Exists("buddies", "CharacterID = '{0}' AND BuddyID = '{1}' AND Pending = '0'", buddyID, this.Parent.ID);
            }
        }

        protected override int GetKeyForItem(Buddy item)
        {
            return item.CharacterID;
        }
    }
}
