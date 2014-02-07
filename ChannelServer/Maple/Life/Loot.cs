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

        public Loot(dynamic datum, bool fromMob = true)
        {
            this.MapleID = datum.itemid;

            if (fromMob)
            {
                this.MinimumQuantity = datum.minimum_quantity;
                this.MaximumQuantity = datum.maximum_quantity;
            }

            this.QuestID = datum.questid;
            this.Chance = datum.chance;
        }

        public Loot(int minMeso, int maxMeso, int chance)
        {
            this.MapleID = 0;
            this.MinimumQuantity = minMeso;
            this.MaximumQuantity = maxMeso;
            this.Chance = chance;
        }
    }
}
