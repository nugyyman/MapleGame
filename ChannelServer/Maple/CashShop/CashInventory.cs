using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Maple.Characters;
using Loki.Net;
using Loki.Data;
using Loki.Collections;

namespace Loki.Maple.CashShop
{
    public class CashInventory : NumericalKeyedCollection<CashItem>
    {
        public Character Parent { get; private set; }

        public CashInventory(Character parent)
        {
            this.Parent = parent;
        }

        protected override void InsertItem(int index, CashItem item)
        {
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            CashItem item = this.GetAtIndex(index);

            Item cash = item.ToItem();
            this.Parent.Items.Add(cash);

            using (Packet outPacket = new Packet(MapleServerOperationCode.CashShopOperation))
            {
                outPacket.WriteByte((byte)(CashShopOperation.Operation + 36));
                outPacket.WriteShort((short)cash.Slot);
                outPacket.WriteBytes(cash.ToByteArray(true));
                outPacket.WriteInt();

                this.Parent.Client.Send(outPacket);
            }

            Database.Delete("cash_inventory", "UniqueID = '{0}'", item.UniqueID);
            base.RemoveItem(index);
        }

        public void Load()
        {
            foreach (dynamic datum in new Datums("cash_inventory").Populate("AccountID = '{0}'", this.Parent.AccountID))
            {
                CashItem cashItem = new CashItem(datum.SerialNumber, datum.Quantity);
                cashItem.UniqueID = datum.UniqueID;
                this.Add(cashItem);
            }
        }

        public void Delete()
        {
            Database.Delete("cash_inventory", "AccountID = '{0}'", this.Parent.AccountID);
        }

        public void Save()
        {
            foreach (CashItem cashItem in this)
            {
                dynamic datum = new Datum("cash_inventory");

                datum.AccountID = this.Parent.AccountID;
                datum.SerialNumber = cashItem.SerialNumber;
                datum.UniqueID = cashItem.UniqueID;
                datum.Quantity = cashItem.Quantity;

                if (!Database.Exists("cash_inventory", "AccountID = '{0}' AND UniqueID = '{1}'", this.Parent.AccountID, cashItem.UniqueID))
                {
                    datum.Insert();
                }
                else
                {
                    datum.Update("AccountID = '{0}' AND UniqueID = '{1}'", this.Parent.AccountID, cashItem.UniqueID);
                }
            }
        }

        public void Send()
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.CashShopOperation))
            {
                outPacket.WriteByte((byte)(CashShopOperation.Operation + 4));
                outPacket.WriteShort((short)this.Count);

                int equips = 0;
                foreach(CashItem cashItem in this)
                {
                    outPacket.WriteBytes(cashItem.ToByteArray(this.Parent.AccountID));

                    if (cashItem.ToItem().Type == ItemType.Equipment)
                    {
                        //equips++; I have no idea what this is in lithium
                    }
                }

                outPacket.WriteInt(equips);
                foreach(CashItem  cashItem in this)
                {
                    if (cashItem.ToItem().Type == ItemType.Equipment)
                    {
                        outPacket.WriteBytes(cashItem.ToItem().ToByteArray());
                    }
                }

                outPacket.WriteShort(); // Storage slots
                outPacket.WriteShort(); // Character slots
                outPacket.WriteShort();
                outPacket.WriteShort(4);

                this.Parent.Client.Send(outPacket);
            }
        }

        protected override int GetKeyForItem(CashItem item)
        {
            return item.UniqueID;
        }

        public int GenerateUniqueID()
        {
            int uniqueID = 0;

            foreach (dynamic id in new Datums("uniqueids").Populate())
            {
                uniqueID = id.ID + 1;
            }

            dynamic datum = new Datum("uniqueids");
            datum.ID = uniqueID;
            datum.Update("ID = '{0}'", uniqueID - 1);

            return uniqueID;
        }
    }
}