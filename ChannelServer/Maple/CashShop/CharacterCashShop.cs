using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Maple.Characters;
using Loki.Net;

namespace Loki.Maple.CashShop
{
    public class CharacterCashShop
    {
        public Character Parent { get; private set; }
        public CashInventory CashInventory { get; private set; }
        public WishList WishList { get; private set; }

        public CharacterCashShop(Character parent)
        {
            this.Parent = parent;
            this.CashInventory = new CashInventory(this.Parent);
            this.WishList = new WishList(this.Parent);
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
                outPacket.WriteString(this.Parent.Name); // TODO: Account name!
                string[] cs = "00 00 00 00 22 00 4A 26 9A 00 00 04 00 00 00 81 A0 98 00 00 04 00 00 00 4B 26 9A 00 00 04 00 00 00 48 26 9A 00 00 04 00 00 00 49 26 9A 00 00 04 00 00 00 AF 29 9A 00 00 04 00 00 00 AE 29 9A 00 00 04 00 00 00 47 26 9A 00 00 04 00 00 00 45 FE FD 02 00 04 00 00 00 46 FE FD 02 00 04 00 00 00 47 FE FD 02 00 04 00 00 00 48 27 9A 00 00 04 00 00 00 10 9F 98 00 00 04 00 00 00 11 9F 98 00 00 04 00 00 00 12 9F 98 00 00 04 00 00 00 B3 29 9A 00 00 04 00 00 00 B0 29 9A 00 00 04 00 00 00 55 FE FD 02 00 04 00 00 00 98 FE FD 02 00 04 00 00 00 54 FE FD 02 00 04 00 00 00 99 FE FD 02 00 04 00 00 00 B4 29 9A 00 00 04 00 00 00 47 27 9A 00 00 04 00 00 00 7A 87 93 03 00 04 00 00 00 78 87 93 03 00 04 00 00 00 61 9F 98 00 00 04 00 00 00 79 87 93 03 00 04 00 00 00 60 9F 98 00 00 04 00 00 00 77 87 93 03 00 04 00 00 00 A1 A1 98 00 00 04 00 00 00 A0 A1 98 00 00 04 00 00 00 A3 A1 98 00 00 04 00 00 00 A2 A1 98 00 00 04 00 00 00 A4 A1 98 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 93 FE FD 02 01 00 00 00 00 00 00 00 C4 FD FD 02 01 00 00 00 00 00 00 00 F9 96 98 00 01 00 00 00 00 00 00 00 E8 9E 98 00 01 00 00 00 00 00 00 00 1A 97 98 00 01 00 00 00 01 00 00 00 93 FE FD 02 01 00 00 00 01 00 00 00 C4 FD FD 02 01 00 00 00 01 00 00 00 F9 96 98 00 01 00 00 00 01 00 00 00 E8 9E 98 00 01 00 00 00 01 00 00 00 1A 97 98 00 02 00 00 00 00 00 00 00 93 FE FD 02 02 00 00 00 00 00 00 00 C4 FD FD 02 02 00 00 00 00 00 00 00 F9 96 98 00 02 00 00 00 00 00 00 00 E8 9E 98 00 02 00 00 00 00 00 00 00 1A 97 98 00 02 00 00 00 01 00 00 00 93 FE FD 02 02 00 00 00 01 00 00 00 C4 FD FD 02 02 00 00 00 01 00 00 00 F9 96 98 00 02 00 00 00 01 00 00 00 E8 9E 98 00 02 00 00 00 01 00 00 00 1A 97 98 00 03 00 00 00 00 00 00 00 93 FE FD 02 03 00 00 00 00 00 00 00 C4 FD FD 02 03 00 00 00 00 00 00 00 F9 96 98 00 03 00 00 00 00 00 00 00 E8 9E 98 00 03 00 00 00 00 00 00 00 1A 97 98 00 03 00 00 00 01 00 00 00 93 FE FD 02 03 00 00 00 01 00 00 00 C4 FD FD 02 03 00 00 00 01 00 00 00 F9 96 98 00 03 00 00 00 01 00 00 00 E8 9E 98 00 03 00 00 00 01 00 00 00 1A 97 98 00 04 00 00 00 00 00 00 00 93 FE FD 02 04 00 00 00 00 00 00 00 C4 FD FD 02 04 00 00 00 00 00 00 00 F9 96 98 00 04 00 00 00 00 00 00 00 E8 9E 98 00 04 00 00 00 00 00 00 00 1A 97 98 00 04 00 00 00 01 00 00 00 93 FE FD 02 04 00 00 00 01 00 00 00 C4 FD FD 02 04 00 00 00 01 00 00 00 F9 96 98 00 04 00 00 00 01 00 00 00 E8 9E 98 00 04 00 00 00 01 00 00 00 1A 97 98 00 05 00 00 00 00 00 00 00 93 FE FD 02 05 00 00 00 00 00 00 00 C4 FD FD 02 05 00 00 00 00 00 00 00 F9 96 98 00 05 00 00 00 00 00 00 00 E8 9E 98 00 05 00 00 00 00 00 00 00 1A 97 98 00 05 00 00 00 01 00 00 00 93 FE FD 02 05 00 00 00 01 00 00 00 C4 FD FD 02 05 00 00 00 01 00 00 00 F9 96 98 00 05 00 00 00 01 00 00 00 E8 9E 98 00 05 00 00 00 01 00 00 00 1A 97 98 00 06 00 00 00 00 00 00 00 93 FE FD 02 06 00 00 00 00 00 00 00 C4 FD FD 02 06 00 00 00 00 00 00 00 F9 96 98 00 06 00 00 00 00 00 00 00 E8 9E 98 00 06 00 00 00 00 00 00 00 1A 97 98 00 06 00 00 00 01 00 00 00 93 FE FD 02 06 00 00 00 01 00 00 00 C4 FD FD 02 06 00 00 00 01 00 00 00 F9 96 98 00 06 00 00 00 01 00 00 00 E8 9E 98 00 06 00 00 00 01 00 00 00 1A 97 98 00 07 00 00 00 00 00 00 00 93 FE FD 02 07 00 00 00 00 00 00 00 C4 FD FD 02 07 00 00 00 00 00 00 00 F9 96 98 00 07 00 00 00 00 00 00 00 E8 9E 98 00 07 00 00 00 00 00 00 00 1A 97 98 00 07 00 00 00 01 00 00 00 93 FE FD 02 07 00 00 00 01 00 00 00 C4 FD FD 02 07 00 00 00 01 00 00 00 F9 96 98 00 07 00 00 00 01 00 00 00 E8 9E 98 00 07 00 00 00 01 00 00 00 1A 97 98 00 08 00 00 00 00 00 00 00 93 FE FD 02 08 00 00 00 00 00 00 00 C4 FD FD 02 08 00 00 00 00 00 00 00 F9 96 98 00 08 00 00 00 00 00 00 00 E8 9E 98 00 08 00 00 00 00 00 00 00 1A 97 98 00 08 00 00 00 01 00 00 00 93 FE FD 02 08 00 00 00 01 00 00 00 C4 FD FD 02 08 00 00 00 01 00 00 00 F9 96 98 00 08 00 00 00 01 00 00 00 E8 9E 98 00 08 00 00 00 01 00 00 00 1A 97 98 00 00 00 00 00 00 00 00 A2 00 00 00".Split(' ');
                for (int i = 0; i < cs.Length; i++)
                {
                    outPacket.WriteByte(byte.Parse(cs[i], System.Globalization.NumberStyles.HexNumber));
                }

                this.Parent.Client.Send(outPacket);
            }

            this.CashInventory.Send();
            #region // TODO: Gifts
            using (Packet outPacket = new Packet(MapleServerOperationCode.CashShopOperation))
            {
                outPacket.WriteByte(0x5A);
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
