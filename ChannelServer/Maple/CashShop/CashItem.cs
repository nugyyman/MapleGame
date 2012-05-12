using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.IO;
using Loki.Data;
using Loki.Maple.Data;

namespace Loki.Maple.CashShop
{
    public class CashItem
    {
        public int MapleID { get; private set; }
        public int SerialNumber { get; private set; }
        public long CashItemID { get; private set; }
        public int ExpirationDays { get; private set; }
        public int CashShopQuantity { get; private set; }
        public int CashShopPrice { get; private set; }
        public string Gender { get; private set; }

        public CashItem CachedReference
        {
            get
            {
                return World.CachedCashItems[this.SerialNumber];
            }
        }

        public CashItem(dynamic itemDatum)
        {
            if (!World.IsInitialized)
            {
                this.SerialNumber = itemDatum.serial_number;
                this.MapleID = itemDatum.itemid;
                this.CashItemID = new Random().Next(int.MaxValue) + 1;
                this.ExpirationDays = itemDatum.expiration_days;
                this.CashShopQuantity = itemDatum.quantity;
                this.CashShopPrice = itemDatum.price;
                this.Gender = itemDatum.gender;
            }
        }

        public CashItem(int serialNumber)
        {
            this.SerialNumber = serialNumber;
            this.MapleID = this.CachedReference.MapleID;
            this.CashItemID = this.CachedReference.CashItemID;
            this.ExpirationDays = this.CachedReference.ExpirationDays;
            this.CashShopQuantity = this.CachedReference.CashShopQuantity;
            this.CashShopPrice = this.CachedReference.CashShopPrice;
            this.Gender = this.CachedReference.Gender;
        }

        public static CashItem GetCashItem(int serialNumber)
        {
            if (World.CachedCashItems.Contains(serialNumber))
            {
                return new CashItem(serialNumber);
            }

            return null;
        }

        /*public byte[] ToByteArray(int accountID)
        {
            using (ByteBuffer buffer = new ByteBuffer())
            {
                buffer.WriteLong(this.CashItemID);
                buffer.WriteInt(accountID);
                buffer.WriteInt(0);
                buffer.WriteInt(this.MapleID);
                buffer.WriteInt(this.SerialNumber);
                //buffer.WriteShort(this.Quantity);
                //buffer.WriteStringFixed(this.Creator, 13);

                // TODO: This is expiration time: Implement it.
                buffer.WriteByte();
                buffer.WriteBytes(PacketConstants.Item);
                buffer.WriteBytes((byte)0xBB, 0x46, (byte)0xE6, 0x17);
                buffer.WriteByte(2); // 1 to show it, 2 to hide it.

                buffer.WriteLong();
                buffer.WriteLong();
                buffer.WriteShort();

                buffer.Flip();
                return buffer.GetContent();
            }
        }*/
    }
}
