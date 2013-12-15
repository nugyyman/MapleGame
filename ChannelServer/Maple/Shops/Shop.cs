using System;
using System.Collections.Generic;
using Loki.Data;
using Loki.Maple.Characters;
using Loki.Maple.Life;
using Loki.Net;

namespace Loki.Maple.Shops
{
    public class Shop
    {
        public static void LoadRechargeTiers()
        {
            Shop.RechargeTiers = new Dictionary<byte, Dictionary<int, double>>();

            foreach (dynamic datum in new Datums("shop_recharge_data").Populate())
            {
                if (!Shop.RechargeTiers.ContainsKey((byte)datum.tierid))
                {
                    Shop.RechargeTiers.Add((byte)datum.tierid, new Dictionary<int, double>());
                }

                Shop.RechargeTiers[(byte)datum.tierid].Add(datum.itemid, datum.price);
            }
        }

        private byte RechargeTierID { get; set; }

        public int ID { get; private set; }
        public Npc Parent { get; private set; }
        public List<ShopItem> Items { get; private set; }
        public static Dictionary<byte, Dictionary<int, double>> RechargeTiers { get; set; }

        public Dictionary<int, double> UnitPrices
        {
            get
            {
                return Shop.RechargeTiers[this.RechargeTierID];
            }
        }

        public Shop(Npc parent, dynamic shopDatum)
        {
            this.Parent = parent;

            this.ID = shopDatum.shopid;
            this.RechargeTierID = (byte)shopDatum.recharge_tier;

            this.Items = new List<ShopItem>();

            foreach (dynamic itemDatum in new Datums("shop_items").Populate("shopid = '{0}'", this.ID))
            {
                this.Items.Add(new ShopItem(this, itemDatum));
            }

            if (this.RechargeTierID > 0)
            {
                foreach (KeyValuePair<int, double> rechargeable in this.UnitPrices)
                {
                    this.Items.Add(new ShopItem(this, rechargeable.Key));
                }
            }
        }

        public void Show(Character customer)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.OpenNpcShop))
            {
                outPacket.WriteInt(this.ID);
                outPacket.WriteShort((short)this.Items.Count);

                foreach (ShopItem loopShopItem in this.Items)
                {
                    outPacket.WriteInt(loopShopItem.MapleID);
                    outPacket.WriteInt(loopShopItem.PurchasePrice);
                    outPacket.WriteInt(); // Discount
                    outPacket.WriteInt();
                    outPacket.WriteInt();
                    outPacket.WriteInt();
                    outPacket.WriteByte();

                    if (loopShopItem.IsRechargeable)
                    {
                        outPacket.WriteShort();
                        outPacket.WriteInt();
                        outPacket.WriteShort((short)(BitConverter.DoubleToInt64Bits(loopShopItem.UnitPrice) >> 48));
                        outPacket.WriteShort(loopShopItem.MaxPerStack);
                    }
                    else
                    {
                        outPacket.WriteShort(loopShopItem.Quantity);
                        outPacket.WriteShort(loopShopItem.MaxPerStack);
                    }
                }

                customer.Client.Send(outPacket);
            }
        }

        public void Handle(Character customer, Packet inPacket)
        {
            switch ((ShopAction)inPacket.ReadByte())
            {
                case ShopAction.Buy:
                    {
                        ShopItem item = this.Items[inPacket.ReadShort()];
                        int mapleId = inPacket.ReadInt();
                        short quantity = inPacket.ReadShort();

                        if (customer.Meso < item.PurchasePrice * quantity)
                        {
                            return;
                        }
                        else
                        {
                            Item purchase;
                            int price;

                            if (item.IsRechargeable)
                            {
                                purchase = new Item(item.MapleID, item.MaxPerStack);
                                price = item.PurchasePrice;
                            }
                            else if (item.Quantity > 1)
                            {
                                purchase = new Item(item.MapleID, item.Quantity);
                                price = item.PurchasePrice;
                            }
                            else
                            {
                                purchase = new Item(item.MapleID, quantity);
                                price = item.PurchasePrice * quantity;
                            }

                            if (customer.Items.SpaceTakenBy(purchase) > customer.Items.RemainingSlots(purchase.Type))
                            {
                                customer.Notify("Your inventory is full.", NoticeType.Popup);
                            }
                            else
                            {
                                customer.Meso -= price;
                                customer.Items.Add(purchase);
                            }

                            using (Packet outPacket = new Packet(MapleServerOperationCode.ConfirmShopTransaction))
                            {
                                outPacket.WriteByte(0);

                                customer.Client.Send(outPacket);
                            }
                        }
                    }

                    break;

                case ShopAction.Sell:
                    {
                        sbyte slot = (sbyte)inPacket.ReadShort();
                        int itemId = inPacket.ReadInt();
                        short quantity = inPacket.ReadShort();

                        Item item = customer.Items[itemId, slot];

                        if (item.IsBlocked)
                        {
                            throw new HackException("Selling blocked item.");
                        }
                        else
                        {
                            if (item.IsRechargeable)
                            {
                                quantity = item.Quantity;
                            }
                            else if (quantity == 0) // TODO: Might not be needed. Wtf is that check anyway.
                            {
                                quantity = 1;
                            }

                            if (quantity > item.Quantity)
                            {
                                throw new HackException("Selling more than available.");
                            }
                            else if (quantity == item.Quantity)
                            {
                                customer.Items.Remove(item, true);
                            }
                            else if (quantity < item.Quantity)
                            {
                                item.Quantity -= quantity;
                                item.Update();
                            }

                            if (item.IsRechargeable)
                            {
                                customer.Meso += item.SalePrice + (int)(this.UnitPrices[item.MapleID] * item.Quantity);
                            }
                            else
                            {
                                customer.Meso += item.SalePrice * quantity;
                            }

                            using (Packet outPacket = new Packet(MapleServerOperationCode.ConfirmShopTransaction))
                            {
                                outPacket.WriteByte(8);

                                customer.Client.Send(outPacket);
                            }
                        }
                    }

                    break;

                case ShopAction.Recharge:
                    {
                        Item item = customer.Items[ItemType.Usable, (sbyte)inPacket.ReadShort()];

                        int price = (int)(this.UnitPrices[item.MapleID] * (item.MaxPerStack - item.Quantity));

                        if (customer.Meso < price)
                        {
                            customer.Notify("You do not have enough mesos.", NoticeType.Popup);
                        }
                        else
                        {
                            customer.Meso -= price;

                            item.Quantity = item.MaxPerStack;
                            item.Update();
                        }

                        using (Packet outPacket = new Packet(MapleServerOperationCode.ConfirmShopTransaction))
                        {
                            outPacket.WriteByte(8);

                            customer.Client.Send(outPacket);
                        }
                    }

                    break;

                case ShopAction.Leave:
                    {
                        customer.LastNpc = null;
                    }

                    break;
            }
        }
    }
}
