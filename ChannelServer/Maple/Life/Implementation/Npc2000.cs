using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Maple.Characters;
using Loki.Net;
using Loki.Data;

namespace Loki.Maple.Life.Implementation
{
    public class Npc2000 : Npc
    {
        public Npc2000(dynamic datum) : base((Datum)datum) { }

        public override void Converse(Character talker, NpcEventArgs args)
        {
            if (!talker.Quests.Started.ContainsKey(talker.LastQuest.ID)) //Start
            {
                ShowYesNoDialog(talker, "Do you want to start the quest?", new NpcEventHandler(ClickYes));
            }
            else // End
            {
                if (!talker.Items.Contains(2010007))
                {
                    talker.GainExperience(10, true);
                    talker.Items.Add(new Item(2010000, 3));
                    talker.Items.Add(new Item(2010009, 3));
                    talker.Quests.Finish(talker.LastQuest, 0);
                }
            }
        }

        void ClickYes(Character talker, NpcEventArgs args)
        {
            talker.CurrentHP = 25;
            talker.Items.Add(new Item(2010007), true);
            talker.Quests.Start(talker.LastQuest, this.MapleID);
        }
    }
}
