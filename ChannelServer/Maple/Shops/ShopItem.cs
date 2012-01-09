using Loki.Data;
using Loki.Maple.Data;

namespace Loki.Maple.Shops
{
	public class ShopItem
	{
		public Shop Parent {get;private set;}

		public int MapleID { get; private set; }
		public short Quantity { get; private set; }
		public int PurchasePrice { get; private set; }
		public int Sort { get; private set; }

		public short MaxPerStack
		{
			get
			{
				return World.CachedItems[this.MapleID].MaxPerStack;
			}
		}

		public int SalePrice
		{
			get
			{
				return World.CachedItems[this.MapleID].SalePrice;
			}
		}

		public double UnitPrice
		{
			get
			{
				return this.Parent.UnitPrices[this.MapleID];
			}
		}

		public bool IsRechargeable
		{
			get
			{
				return World.CachedItems[this.MapleID].IsRechargeable;
			}
		}

		public ShopItem(Shop parent, dynamic datum)
		{
			this.Parent = parent;

			this.MapleID = datum.itemid;
			this.Quantity = datum.quantity;
			this.PurchasePrice = datum.price;
			this.Sort = datum.sort;
		}

		public ShopItem(Shop parent, int mapleId)
		{
			this.Parent = parent;

			this.MapleID = mapleId;
			this.Quantity = 1;
			this.PurchasePrice = 0;
		}
	}
}
