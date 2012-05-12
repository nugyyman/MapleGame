using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Maple.Characters;
using Loki.Net;
using Loki.Data;

namespace Loki.Maple.CashShop
{
    public class CashInventory : List<CashItem>
    {
        public Character Parent { get; private set; }

        public CashInventory(Character parent)
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

        public void Send()
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.CashShopOperation))
            {
                outPacket.WriteByte(0x58);
                outPacket.WriteShort((short)this.Count);

                foreach(CashItem cashItem in this)
                {
                    //outPacket.WriteBytes(cashItem.ToByteArray(this.Parent.AccountID));
                }

                outPacket.WriteShort(); // Storage slots
                outPacket.WriteShort(); // Character slots
                outPacket.WriteShort();
                outPacket.WriteShort();

                this.Parent.Client.Send(outPacket);
            }
        }
    }
}
