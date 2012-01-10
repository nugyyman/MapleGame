using Loki.Data;
using Loki.Maple.Characters;
using Loki.Maple.Data;
using System.Dynamic;

namespace Loki.Maple.Life.Implementation
{
    public class Npc9900001 : Npc
    {
        public Npc9900001(dynamic datum) : base((Datum)datum) { }

        public override void Converse(Character talker, NpcEventArgs args)
        {
            if (talker.IsMaster)
            {
                bool advanced = talker.Job == Job.SuperGM;

                foreach (int loopWizetItem in World.CachedItems.WizetItemIDs)
                {
                    if (!talker.Items.Contains(loopWizetItem))
                    {
                        advanced = false;
                    }
                }

                if (advanced)
                {
                    ShowOkDialog(talker, "You have already fully advanced to #bGameMaster#k.");
                }
                else
                {
                    if (talker.Job != Job.SuperGM)
                    {
                        talker.Job = Job.SuperGM;
                    }

                    talker.MaxHP = talker.MaxMP = 10000;
                    talker.CurrentHP = talker.CurrentMP = 10000;

                    foreach (int loopWizetItem in World.CachedItems.WizetItemIDs)
                    {
                        if (!talker.Items.Contains(loopWizetItem))
                        {
                            talker.Items.Add(new Item(loopWizetItem));
                        }
                    }

                    foreach (Item loopItem in talker.Items)
                    {
                        foreach (int loopWizetItem in World.CachedItems.WizetItemIDs)
                        {
                            if (loopItem.MapleID == loopWizetItem)
                            {
                                loopItem.Equip();
                            }
                        }
                    }

                    ShowOkDialog(talker, "You have been successfully advanced as a #bGameMaster#k.");
                }
            }
            else
            {
                ShowYesNoDialog(talker, "You seem lost. Do you want to be warped out of here?", new NpcEventHandler(OnYesWarpOut));
            }
        }

        void OnYesWarpOut(Character talker, NpcEventArgs args)
        {
            talker.ChangeMap(100000000);
        }
    }
}
