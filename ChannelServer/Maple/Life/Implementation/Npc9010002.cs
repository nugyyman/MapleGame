using Loki.Data;
using Loki.Maple.Characters;

namespace Loki.Maple.Life.Implementation
{
	// Mia
	public class Npc9010002 : Npc
	{
		public Npc9010002(dynamic datum) : base((Datum)datum) { }

		public override void Converse(Character talker, NpcEventArgs args)
		{
			ShowNextDialog(talker, "Omg, am I Mia? ...", new NpcEventHandler(Converse2));
		}

		void Converse2(Character talker, NpcEventArgs args)
		{
			ShowNextPreviousDialog(talker, "I must be...", new NpcEventHandler(Converse3), new NpcEventHandler(Converse));
		}

		void Converse3(Character talker, NpcEventArgs args)
		{
			ShowChoiceDialog(talker, "Whatever. Wanna travel?", new NpcEventHandler(Selection), "Henesys", "Ellinia");
		}

		void Selection(Character talker, NpcEventArgs args)
		{
			switch (args.Selection)
			{
				case 0:
					talker.ChangeMap(100000000);
					break;

				case 1:
					talker.ChangeMap(101000000);
					break;
			}
		}
	}
}
