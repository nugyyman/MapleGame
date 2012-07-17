using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Data;
using Loki.Maple.Characters;

namespace Loki.Maple.Life.Implementation
{
    class Npc9030100 : Npc
    {
        public Npc9030100(dynamic datum) : base((Datum)datum) { }

        public override void Converse(Character talker, NpcEventArgs args)
        {
            talker.Storage.Open(this.MapleID);
        }
    }
}
