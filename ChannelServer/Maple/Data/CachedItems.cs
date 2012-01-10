using System.Collections.Generic;
using System.Collections.ObjectModel;
using Loki.Data;
using System;

namespace Loki.Maple.Data
{
    public class CachedItems : KeyedCollection<int, Item>
    {
        public CachedItems()
            : base()
        {
            using (Log.Load("Items"))
            {
                foreach (dynamic itemDatum in new Datums("item_data").Populate())
                {
                    this.Add(new Item(itemDatum));
                }
            }

            using (Log.Load("Equipments"))
            {
                foreach (dynamic equipDatum in new Datums("item_equip_data").Populate())
                {
                    this[equipDatum.itemid].LoadEquipmentData(equipDatum);
                }
            }

            using (Log.Load("Scrolls"))
            {
                foreach (dynamic scrollDatum in new Datums("item_scroll_data").Populate())
                {
                    this[scrollDatum.itemid].LoadScrollData(scrollDatum);
                }
            }

            using (Log.Load("Skill Books"))
            {
                foreach (dynamic skillBookDatum in new Datums("item_skills").Populate())
                {
                    this[skillBookDatum.itemid].LoadSkillBookData(skillBookDatum);
                }
            }

            using (Log.Load("Item Consumes"))
            {
                foreach (dynamic itemConsumeDatum in new Datums("item_consume_data").Populate())
                {
                    this[itemConsumeDatum.itemid].LoadItemConsumeData(itemConsumeDatum);
                }
            }

            this.WizetItemIDs = new List<int>(4);

            this.WizetItemIDs.Add(1002140);
            this.WizetItemIDs.Add(1322013);
            this.WizetItemIDs.Add(1042003);
            this.WizetItemIDs.Add(1062007);
        }

        protected override int GetKeyForItem(Item item)
        {
            return item.MapleID;
        }

        public List<int> WizetItemIDs { get; private set; }
    }
}
