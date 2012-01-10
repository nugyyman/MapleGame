using Loki.Data;
using Loki.Maple.Characters;

namespace Loki.Maple.Life.Implementation
{
    public class NpcTemplate : Npc
    {
        public NpcTemplate(dynamic datum) : base((Datum)datum) { }

        public override void Converse(Character talker, NpcEventArgs args)
        {

        }
    }
}
