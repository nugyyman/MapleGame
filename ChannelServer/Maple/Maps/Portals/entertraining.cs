using Loki.Data;
using Loki.Maple.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loki.Maple.Maps.Portals
{
    public class entertraining : Portal
    {
        public entertraining(dynamic datum) : base((Datum)datum) { }

        public override void Enter(Character character)
        {
            if (character.Quests.Started.ContainsKey(1041))
            {
                character.ChangeMap(1010100, 4);
            }
            else if (character.Quests.Started.ContainsKey(1042))
            {
                character.ChangeMap(1010200, 4);
            } 
            else if (character.Quests.Started.ContainsKey(1043))
            {
                character.ChangeMap(1010300, 4);
            }
            else if (character.Quests.Started.ContainsKey(1044))
            {
                character.ChangeMap(1010400, 4);
            }
            else
            {
                character.Notify("Only the adventurers that have been trained by Mai may enter.");
            }
        }
    }
}
