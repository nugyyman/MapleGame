using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Maple.Characters;
using Loki.Net;
using Loki.Maple.Data;
using Loki.Maple.Commands.Implementation;

namespace Loki.Maple.CashShop
{
    public class CharacterCashShop
    {
        public Character Parent { get; private set; }
        public CashInventory CashInventory { get; private set; }
        public WishList WishList { get; private set; }
        public bool Open { get; set; }

        public CharacterCashShop(Character parent)
        {
            this.Parent = parent;
            this.CashInventory = new CashInventory(this.Parent);
            this.WishList = new WishList(this.Parent);
            this.Open = false;
        }

        public void Load()
        {
            this.CashInventory.Load();
            this.WishList.Load();
        }

        public void Delete()
        {
            this.CashInventory.Delete();
            this.WishList.Delete();
        }

        public void Save()
        {
            this.CashInventory.Save();
            this.WishList.Save();
        }

        public void Enter()
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.CashShopOpen))
            {
                outPacket.WriteBytes(this.Parent.DataToByteArray());
                outPacket.WriteByte(1);

                outPacket.WriteInt(0); // Not Sale count. -> Send here removes the item from CS. For each: int(SN)

                outPacket.WriteShort(0); // Modified items count

                outPacket.WriteByte(0); // Category discount rate

                string[] cashShopData = "04 00 00 00 4B 6D 54 00 07 00 00 00 75 31 31 01 76 31 31 01 77 31 31 01 78 31 31 01 79 31 31 01 7A 31 31 01 7B 31 31 01 4A 6D 54 00 05 00 00 00 0E 63 3D 01 0F 63 3D 01 10 63 3D 01 11 63 3D 01 12 63 3D 01 49 6D 54 00 06 00 00 00 03 63 3D 01 04 63 3D 01 07 63 3D 01 08 63 3D 01 09 63 3D 01 0B 63 3D 01 48 6D 54 00 08 00 00 00 CE 2E 31 01 CF 2E 31 01 D0 2E 31 01 D1 2E 31 01 D2 2E 31 01 D3 2E 31 01 D4 2E 31 01 D5 2E 31 01 02 00 00 00 31 00 00 00 0A 00 10 00 12 00 0E 07 E0 3B 8B 0B 60 CE 8A 0B 69 00 6C 00 6C 00 2F 00 35 00 33 00 32 00 30 00 30 00 31 00 31 00 2F 00 73 00 75 00 6D 00 6D 00 6F 00 6E 00 2F 00 61 00 74 00 74 00 61 00 63 00 6B 00 31 00 2F 00 31 00 00 00 00 00 00 00 00 00 02 00 1A 00 04 01 08 07 02 00 00 00 32 00 00 00 05 00 1C 00 06 00 08 07 A0 01 2E 00 58 CD 8A 0B 01 00 00 00 00 00 00 00 6F A2 98 00 01 00 00 00 00 00 00 00 92 A2 98 00 01 00 00 00 00 00 00 00 A8 2A 9A 00 01 00 00 00 00 00 00 00 10 A2 98 00 01 00 00 00 00 00 00 00 43 2A 9A 00 01 00 00 00 01 00 00 00 6F A2 98 00 01 00 00 00 01 00 00 00 92 A2 98 00 01 00 00 00 01 00 00 00 A8 2A 9A 00 01 00 00 00 01 00 00 00 10 A2 98 00 01 00 00 00 01 00 00 00 43 2A 9A 00 02 00 00 00 00 00 00 00 6F A2 98 00 02 00 00 00 00 00 00 00 92 A2 98 00 02 00 00 00 00 00 00 00 A8 2A 9A 00 02 00 00 00 00 00 00 00 10 A2 98 00 02 00 00 00 00 00 00 00 43 2A 9A 00 02 00 00 00 01 00 00 00 6F A2 98 00 02 00 00 00 01 00 00 00 92 A2 98 00 02 00 00 00 01 00 00 00 A8 2A 9A 00 02 00 00 00 01 00 00 00 10 A2 98 00 02 00 00 00 01 00 00 00 43 2A 9A 00 03 00 00 00 00 00 00 00 6F A2 98 00 03 00 00 00 00 00 00 00 92 A2 98 00 03 00 00 00 00 00 00 00 A8 2A 9A 00 03 00 00 00 00 00 00 00 10 A2 98 00 03 00 00 00 00 00 00 00 43 2A 9A 00 03 00 00 00 01 00 00 00 6F A2 98 00 03 00 00 00 01 00 00 00 92 A2 98 00 03 00 00 00 01 00 00 00 A8 2A 9A 00 03 00 00 00 01 00 00 00 10 A2 98 00 03 00 00 00 01 00 00 00 43 2A 9A 00 04 00 00 00 00 00 00 00 6F A2 98 00 04 00 00 00 00 00 00 00 92 A2 98 00 04 00 00 00 00 00 00 00 A8 2A 9A 00 04 00 00 00 00 00 00 00 10 A2 98 00 04 00 00 00 00 00 00 00 43 2A 9A 00 04 00 00 00 01 00 00 00 6F A2 98 00 04 00 00 00 01 00 00 00 92 A2 98 00 04 00 00 00 01 00 00 00 A8 2A 9A 00 04 00 00 00 01 00 00 00 10 A2 98 00 04 00 00 00 01 00 00 00 43 2A 9A 00 05 00 00 00 00 00 00 00 6F A2 98 00 05 00 00 00 00 00 00 00 92 A2 98 00 05 00 00 00 00 00 00 00 A8 2A 9A 00 05 00 00 00 00 00 00 00 10 A2 98 00 05 00 00 00 00 00 00 00 43 2A 9A 00 05 00 00 00 01 00 00 00 6F A2 98 00 05 00 00 00 01 00 00 00 92 A2 98 00 05 00 00 00 01 00 00 00 A8 2A 9A 00 05 00 00 00 01 00 00 00 10 A2 98 00 05 00 00 00 01 00 00 00 43 2A 9A 00 06 00 00 00 00 00 00 00 6F A2 98 00 06 00 00 00 00 00 00 00 92 A2 98 00 06 00 00 00 00 00 00 00 A8 2A 9A 00 06 00 00 00 00 00 00 00 10 A2 98 00 06 00 00 00 00 00 00 00 43 2A 9A 00 06 00 00 00 01 00 00 00 6F A2 98 00 06 00 00 00 01 00 00 00 92 A2 98 00 06 00 00 00 01 00 00 00 A8 2A 9A 00 06 00 00 00 01 00 00 00 10 A2 98 00 06 00 00 00 01 00 00 00 43 2A 9A 00 07 00 00 00 00 00 00 00 6F A2 98 00 07 00 00 00 00 00 00 00 92 A2 98 00 07 00 00 00 00 00 00 00 A8 2A 9A 00 07 00 00 00 00 00 00 00 10 A2 98 00 07 00 00 00 00 00 00 00 43 2A 9A 00 07 00 00 00 01 00 00 00 6F A2 98 00 07 00 00 00 01 00 00 00 92 A2 98 00 07 00 00 00 01 00 00 00 A8 2A 9A 00 07 00 00 00 01 00 00 00 10 A2 98 00 07 00 00 00 01 00 00 00 43 2A 9A 00 08 00 00 00 00 00 00 00 6F A2 98 00 08 00 00 00 00 00 00 00 92 A2 98 00 08 00 00 00 00 00 00 00 A8 2A 9A 00 08 00 00 00 00 00 00 00 10 A2 98 00 08 00 00 00 00 00 00 00 43 2A 9A 00 08 00 00 00 01 00 00 00 6F A2 98 00 08 00 00 00 01 00 00 00 92 A2 98 00 08 00 00 00 01 00 00 00 A8 2A 9A 00 08 00 00 00 01 00 00 00 10 A2 98 00 08 00 00 00 01 00 00 00 43 2A 9A 00".Split(' ');

                for (int i = 0; i < cashShopData.Length; i++)
                {
                    outPacket.WriteByte(byte.Parse(cashShopData[i], System.Globalization.NumberStyles.HexNumber));
                }

                outPacket.WriteShort();
                outPacket.WriteShort();
                outPacket.WriteShort();
                outPacket.WriteByte();
                outPacket.WriteInt();
                outPacket.WriteByte();

                this.Parent.Client.Send(outPacket);
            }

            this.CashInventory.Send();
            #region // TODO: Gifts
            using (Packet outPacket = new Packet(MapleServerOperationCode.CashShopOperation))
            {
                outPacket.WriteByte((byte)(CashShopOperation.Operation + 6));
                outPacket.WriteShort(0);

                this.Parent.Client.Send(outPacket);
            }
            #endregion
            this.WishList.Send(false);
            this.ShowCash();
        }

        public void ShowCash()
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.CashShopUpdate))
            {
                outPacket.WriteInt(this.Parent.CardNX);
                outPacket.WriteInt(this.Parent.MaplePoints);
                outPacket.WriteInt(this.Parent.PaypalNX);

                this.Parent.Client.Send(outPacket);
            }
        }
    }
}
