using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Net;
using Loki.Data;
using Loki.Maple.Data;

namespace Loki.Maple.Characters
{
    public class CharacterBuddyList : Dictionary<int, Buddy>
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

            foreach (dynamic datum in new Datums("buddies").Populate("CharacterID = '{0}' AND Pending = '0'", this.Parent.ID))
            {
                name = Database.Fetch("characters", "Name", "ID = '{0}'", datum.BuddyID);
                this.Add(datum.BuddyID, new Buddy(name, datum.GroupName, datum.BuddyID, 0, true));
            }
        }

        public void LoadPendingBuddies()
        {
            string name;

            foreach (dynamic datum in new Datums("buddies").Populate("CharacterID = '{0}' AND Pending = '1'", this.Parent.ID))
            {
                name = Database.Fetch("characters", "Name", "ID = '{0}'", datum.BuddyID);
                this.pendingBuddies.Add(new Buddy(name, datum.GroupName, datum.BuddyID, 0, false));
            }

            Database.Delete("buddies", "CharacterID = '{0}' AND Pending = '1'", this.Parent.ID);
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
            foreach (Buddy loopBuddy in this.Values)
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

        public void Update()
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.BuddyList))
            {
                outPacket.WriteByte(7);
                outPacket.WriteByte((byte)this.Count);

                foreach (Buddy buddy in this.Values)
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

        public void UpdateChannels()
        {
            List<int> buddies = new List<int>();

            foreach (int loopBuddy in this.Keys)
            {
                buddies.Add(loopBuddy);
            }

            foreach (int loopBuddy in buddies)
            {
                if (!this.IsMutualBuddy(loopBuddy))
                    buddies.Remove(loopBuddy);
            }

            Dictionary<int, byte> onlineBuddies = ChannelServer.LoginServerConnection.UpdateBuddies(this.Parent.ID, false, buddies);

            foreach (KeyValuePair<int, byte> loopBuddy in onlineBuddies)
            {
                this[loopBuddy.Key].Channel = (byte)(loopBuddy.Value + 1);
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

        public Buddy this[string name]
        {
            get
            {
                foreach (Buddy buddy in this.Values)
                {
                    if (buddy.Name.ToLower().Equals(name.ToLower()))
                        return buddy;
                }

                return null;
            }
        }

        private bool IsMutualBuddy(int buddyID)
        {
            return Database.Exists("buddies", "CharacterID = '{0}' AND BuddyID = '{1}' AND Pending = '0'", buddyID, this.Parent.ID);
        }
    }
}
