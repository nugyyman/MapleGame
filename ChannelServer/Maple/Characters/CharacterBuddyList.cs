using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Net;
using Loki.Data;

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
            foreach (dynamic datum in new Datums("buddies").Populate("CharacterID = '{0}'", this.Parent.ID))
            {
                this.Add(datum.KeyID, new Buddy(Database.Fetch("characters", "Name", "ID = '{0}'", datum.BuddyID), datum.Group, datum.BuddyID, -1, true));
            }
        }

        public void Delete()
        {
            Database.Delete("buddies", "CharacterID = '{0}'", this.Parent.ID);
        }

        public void Save()
        {
            this.Delete();

            foreach (Buddy loopBuddy in this.Values)
            {
                dynamic datum = new Datum("buddies");

                datum.CharacterID = this.Parent.ID;
                datum.BuddyID = loopBuddy.CharacterID;
                datum.Pending = 0;
                datum.Group = loopBuddy.Group;

                datum.Insert();
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
                        outPacket.WriteString(GetRightPaddedStr(buddy.Name, '\0', 13));
                        outPacket.WriteByte();
                        outPacket.WriteInt(buddy.Channel - 1);
                        outPacket.WriteString(GetRightPaddedStr(buddy.Group, '\0', 13));
                        outPacket.WriteInt();
                    }
                }

                for (int i = 0; i < this.Count; i++)
                    outPacket.WriteInt();

                this.Parent.Client.Send(outPacket);
            }
        }

        private static string GetRightPaddedStr(string n, char padchar, int length)
        {
            string name = n
                ;
            for (int i = name.Length; i < length; i++)
            {
                name += padchar;
            }

            return name;
        }
    }
}
