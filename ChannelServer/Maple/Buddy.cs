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
        public short Channel { get; set; }
        public bool Visible { get; set; }

        public Buddy(string name, string group, int cid, short channel, bool visible = false)
        {
            this.Name = name;
            this.Group = group;
            this.CharacterID = cid;
            this.Channel = channel;
            this.Visible = visible;
        }

        public bool IsOnline()
        {
            return this.Channel > -1;
        }
    }
}
