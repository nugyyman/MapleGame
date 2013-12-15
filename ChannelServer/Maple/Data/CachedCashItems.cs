using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Loki.Maple.CashShop;
using Loki.Data;

namespace Loki.Maple.Data
{
    public class CachedCashItems : KeyedCollection<int, CashItem>
    {
        public CachedCashItems()
            : base()
        {
            Packages = new Dictionary<int, List<CashItem>>();

            using (Log.Load("Cash Items"))
            {
                foreach (dynamic itemDatum in new Datums("cash_commodity_data").Populate())
                {
                    this.Add(new CashItem(itemDatum));
                }
            }

            using (Log.Load("Cash Packages"))
            {
                foreach (dynamic itemDatum in new Datums("cash_package_data").Populate())
                {
                    if (!this.Packages.ContainsKey(itemDatum.packageid))
                    {
                        this.Packages.Add(itemDatum.packageid, new List<CashItem>());
                    }
                    if (this.Contains(itemDatum.serial_number))
                    {
                        this.Packages[itemDatum.packageid].Add(this[itemDatum.serial_number]);
                    }
                }
            }
        }

        protected override int GetKeyForItem(CashItem item)
        {
            return item.SerialNumber;
        }

        public Dictionary<int, List<CashItem>> Packages;
    }
}
