using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loki.Maple
{
    public class Buddy
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public int CharacterID { get; set; }
        public byte Channel { get; set; }
        public bool Assigned { get; set; }

        public Buddy(string name, string group, int cid, byte channel, bool assigned)
        {
            this.Name = name;
            this.Group = group;
            this.CharacterID = cid;
            this.Channel = channel;
            this.Assigned = assigned;
        }

        public bool IsOnline
        {
            get
            {
                return this.Channel > 0;
            }
        }
    }
}
