using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Maple.Characters;
using Loki.Net;

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
            // TODO: load items.
        }

        public void Delete()
        {
            // TODO: delete items.
        }

        public void Save()
        {
            // TODO: save items.
        }

        public void Send(bool update)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.CashShopOperation))
            {
                if (update)
                {
                    outPacket.WriteByte(0x55);
                }
                else
                {
                    outPacket.WriteByte(0x4F);
                }

                foreach (int sn in this)
                {
                    outPacket.WriteInt(sn);
                }

                for (int i = this.Count; i < 10; i++)
                {
                    outPacket.WriteInt(0);
                }

                this.Parent.Client.Send(outPacket);
            }
        }
    }
}
