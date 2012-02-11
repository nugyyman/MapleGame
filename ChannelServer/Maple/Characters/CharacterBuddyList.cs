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
        public int Capacity { get; set; }

        public CharacterBuddyList(Character parent)
            : base()
        {
            this.Parent = parent;
            this.Capacity = parent.MaxBuddies;
        }

        public bool IsFull()
        {
            return this.Count >= this.Capacity;
        }

        public void Load()
        {
            string name;

            foreach (dynamic datum in new Datums("buddies").Populate("CharacterID = '{0}'", this.Parent.ID))
            {
                name = Database.Fetch("characters", "Name", "ID = '{0}'", datum.BuddyID);
                this.Add(datum.BuddyID, new Buddy(name, datum.GroupName, datum.BuddyID, (byte)(ChannelServer.LoggedIn.Contains(Database.Fetch("characters", "AccountID", "ID = '{0}'", datum.BuddyID)) ? World.Characters[name].Channel : 0), true));
            }
        }

        public void Delete()
        {
            Database.Delete("buddies", "CharacterID = '{0}'", this.Parent.ID);
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

                foreach(Buddy buddy in this.Values)
                {
                    if (buddy.Visible)
                    {
                        outPacket.WriteInt(buddy.CharacterID);
                        outPacket.WriteStringFixed(buddy.Name, 13);
                        outPacket.WriteByte();
                        outPacket.WriteInt(buddy.Channel - 1);
                        outPacket.WriteStringFixed(buddy.Group, 17);
                    }
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
    }
}
