using Loki.Data;
using Loki.Maple.Characters;
using Loki.Net;

namespace Loki.Maple.Life.Implementation
{
    public class Npc1032002 : Npc
    {
        public Npc1032002(dynamic datum) : base((Datum)datum) { }

        public override void Converse(Character talker, NpcEventArgs args)
        {
            ShowNextDialog(talker, "Hi, please press next.", new NpcEventHandler(Converse2));
        }

        void Converse2(Character talker, NpcEventArgs args)
        {
            string text = @"I am collecting #bBlue Snail Shells#k.
							#i4000000#

							Bring me #r20#k of them. If you lie, I #rkill you#k.

							Do you have them?";

            ShowYesNoDialog(talker, text, new NpcEventHandler(ClickYes), new NpcEventHandler(ClickNo));
        }

        void ClickYes(Character talker, NpcEventArgs args)
        {
            if (talker.Items.Contains(4000000, 20))
            {
                talker.Items.Remove(4000000, 20);

                ShowNumberRequestDialog(talker, "Thank you so much! How many potions do you want?", new NpcEventHandler(Reward), 20, 10, 100);
            }
            else
            {
                ShowOkDialog(talker, "You lied. Prepare to die.", new NpcEventHandler(KillCharacter));
            }
        }

        void Reward(Character talker, NpcEventArgs args)
        {
            talker.Items.Add(new Item(2000000, (short)args.Selection));

            talker.Experience += 200 * ChannelServer.ExperienceRate;
        }

        void ClickNo(Character talker, NpcEventArgs args)
        {
            ShowOkDialog(talker, "You suck, bitch.");
        }

        void KillCharacter(Character talker, NpcEventArgs args)
        {
            talker.CurrentHP -= talker.CurrentHP;
        }
    }
}
