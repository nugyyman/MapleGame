using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Net;
using Loki.Maple.Characters;

namespace Loki.Maple.CashShop
{
    public class CashShopOperation
    {
        public static void Handle(Character player, Packet inPacket)
        {
            CharacterCashShop cs = player.CashShop;
            byte action = inPacket.ReadByte();

            switch (action)
            {
                case 0x03: // Buy item
                    break;

                case 0x20: // Buy packege
                    break;

                case 0x04: // Gift
                    break;

                case 0x05: // Modify wishlist
                    cs.WishList.Clear();

                    for (byte i = 0; i < 10; i++)
                    {
                        int sn = inPacket.ReadInt();
                        CashItem cashItem = CashItem.GetCashItem(sn);

                        if (cashItem != null)
                        {
                            cs.WishList.Add(sn);
                        }
                    }

                    cs.WishList.Send(true);
                    break;

                case 0x06: // Buy inventory slots
                    break;

                case 0x07: // Buy storage slots
                    break;

                case 0x08: // Buy character slot
                    break;

                case 0x0E: // Take from cash inventory
                    break;

                case 0x0F: // Put into cash inventory
                    break;

                case 0x1F: // Buy crush ring
                    break;

                case 0x22: // Buy 1 meso item
                    break;

                case 0x25: // Buy friendship ring
                    break;

                case 0x2C: // New tab
                    break;

                default:
                    Log.Warn("Recived an unknown cash operation: " + action);
                    cs.ShowCash();
                    break;
            }
        }
    }
}
