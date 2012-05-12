using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Maple.Characters;
using Loki.Net;
using Loki.IO;
using Loki.Data;

namespace Loki.Maple.CashShop
{
    public class WishList : List<int>
    {
        public Character Parent { get; private set; }

        public WishList(Character parent)
        {
            this.Parent = parent;
        }

        public void Load()
        {
            foreach (dynamic datum in new Datums("wishlists").Populate("CharacterID = '{0}'", this.Parent.ID))
            {
                this.Add(datum.SerialNumber);
            }
        }

        public void Delete()
        {
            Database.Delete("wishlists", "CharacterID = '{0}'", this.Parent.ID);
        }

        public void Save()
        {
            foreach (dynamic datum in new Datums("wishlists").Populate("CharacterID = '{0}'", this.Parent.ID))
            {
                if (!this.Contains(datum.SerialNumber))
                {
                    Database.Delete("wishlists", "CharacterID = '{0}' AND SerialNumber = '{1}'", this.Parent.ID, datum.SerialNumber);
                }
            }

            foreach (int sn in this)
            {
                dynamic datum = new Datum("wishlists");

                datum.CharacterID = this.Parent.ID;
                datum.SerialNumber = sn;

                if (!Database.Exists("wishlists", "CharacterID = '{0}' AND SerialNumber = '{1}'", this.Parent.ID, sn))
                {
                    datum.Insert();
                }
            }
        }

        public byte[] ToByteArray(bool cs = false)
        {
            using (ByteBuffer buffer = new ByteBuffer())
            {
                if (!cs)
                {
                    buffer.WriteByte((byte)this.Count);
                }

                foreach (int sn in this)
                {
                    buffer.WriteInt(sn);
                }

                if (cs)
                {
                    for (int i = this.Count; i < 10; i++)
                    {
                        buffer.WriteInt(0);
                    }
                }

                buffer.Flip();
                return buffer.GetContent();
            }
        }

        public void Send(bool update)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.CashShopOperation))
            {
                if (update)
                {
                    outPacket.WriteByte(0x62);
                }
                else
                {
                    outPacket.WriteByte(0x5C);
                }

                outPacket.WriteBytes(this.ToByteArray(true));

                this.Parent.Client.Send(outPacket);
            }
        }
    }
}
