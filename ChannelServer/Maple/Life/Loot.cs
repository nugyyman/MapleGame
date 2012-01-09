using Loki.Data;

namespace Loki.Maple.Life
{
	public class Loot
	{
		public int MapleID { get; private set; }
		public int MinimumQuantity { get; private set; }
		public int MaximumQuantity { get; private set; }
		public int QuestID { get; private set; }
		public int Chance { get; private set; }
		public bool IsMeso { get; private set; }

		public Loot(dynamic datum)
		{
			this.MapleID = datum.itemid;
			this.MinimumQuantity = datum.minimum_quantity;
			this.MaximumQuantity = datum.maximum_quantity;
			this.QuestID = datum.questid;
			this.Chance = datum.chance;
			this.IsMeso = datum.flags.Contains("is_mesos");
		}
	}
}
