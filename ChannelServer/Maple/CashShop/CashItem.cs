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
        public int UniqueID { get; set; }
        public int ExpirationDays { get; private set; }
        public short Quantity { get; private set; }
        public int Price { get; private set; }
        public string Gender { get; private set; }
        public string GiftFrom { get; set; }

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
                this.UniqueID = 0;
                this.ExpirationDays = itemDatum.expiration_days;
                this.Quantity = itemDatum.quantity;
                this.Price = itemDatum.price;
                this.Gender = itemDatum.gender;
                this.GiftFrom = "";
            }
        }

        public CashItem(int serialNumber, short quantity = 0)
        {
            this.SerialNumber = serialNumber;
            this.MapleID = this.CachedReference.MapleID;
            this.UniqueID = this.CachedReference.UniqueID;
            this.ExpirationDays = this.CachedReference.ExpirationDays;

            if (quantity == 0)
            {
                this.Quantity = this.CachedReference.Quantity;
            }
            else
            {
                this.Quantity = quantity;
            }

            this.Price = this.CachedReference.Price;
            this.Gender = this.CachedReference.Gender;
            this.GiftFrom = this.CachedReference.GiftFrom;
        }

        public static CashItem GetCashItem(int serialNumber)
        {
            if (World.CachedCashItems.Contains(serialNumber))
            {
                return new CashItem(serialNumber);
            }

            return null;
        }

        public byte[] ToByteArray(int accountID, string giftMessage = null)
        {
            bool isGift = giftMessage != null;

            using (ByteBuffer buffer = new ByteBuffer())
            {
                buffer.WriteLong(this.UniqueID);

                if (!isGift)
                {
                    buffer.WriteInt(accountID);
                    buffer.WriteInt(0);
                }

                buffer.WriteInt(this.MapleID);

                if (!isGift)
                {
                    buffer.WriteInt(this.SerialNumber);
                    buffer.WriteShort(this.Quantity);
                    
                }

                buffer.WriteStringFixed(this.GiftFrom, 13);

                if (isGift)
                {
                    buffer.WriteStringFixed(giftMessage, 73);

                    buffer.Flip();
                    return buffer.GetContent();
                }

                // TODO: This is expiration time: Implement it.
                buffer.WriteByte();
                buffer.WriteBytes(PacketConstants.Item);
                buffer.WriteBytes((byte)0xBB, 0x46, (byte)0xE6, 0x17);
                buffer.WriteByte(2); // 1 to show it, 2 to hide it.

                buffer.WriteLong();

                buffer.Flip();
                return buffer.GetContent();
            }
        }

        public Item ToItem()
        {
            Item item = new Item(this.MapleID, this.Quantity);
            item.SerialNumber = this.SerialNumber;
            item.UniqueID = this.UniqueID;

            return item;
        }
    }
}
