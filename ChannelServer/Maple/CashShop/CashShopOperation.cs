using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Net;
using Loki.Maple.Characters;
using Loki.Maple.Data;

namespace Loki.Maple.CashShop
{
    public class CashShopOperation
    {
        public static void Handle(Character player, Packet inPacket)
        {
            if (!player.CashShop.Open)
            {
                return;
            }

            CharacterCashShop cs = player.CashShop;
            byte action = inPacket.ReadByte();
            CashItem cashItem = null;
            Item item = null;
            int uniqueID, cash;

            switch (action)
            {
                case 0x03: // Buy item
                case 0x20: // Buy packege
                    inPacket.Skip(1);
                    cash = inPacket.ReadInt();
                    try
                    {
                        cashItem = new CashItem(inPacket.ReadInt());
                    }
                    catch (KeyNotFoundException)
                    {
                        cashItem = null;
                    }

                    if (!CanBuy(cashItem, player, cash)) return;

                    if (action == 0x03) // Item
                    {
                        cashItem.UniqueID = cs.CashInventory.GenerateUniqueID();
                        player.CashShop.CashInventory.Add(cashItem);

                        using (Packet outPacket = new Packet(MapleServerOperationCode.CashShopOperation))
                        {
                            outPacket.WriteByte(0x64);
                            outPacket.WriteBytes(cashItem.ToByteArray(player.AccountID));

                            player.Client.Send(outPacket);
                        }
                    }
                    else // Package
                    {
                        List<CashItem> package;

                        try
                        {
                            package = World.CachedCashItems.Packages[cashItem.MapleID];
                        }
                        catch (KeyNotFoundException)
                        {
                            return;
                        }

                        foreach (CashItem cItem in package)
                        {
                            cItem.UniqueID = cs.CashInventory.GenerateUniqueID();
                            player.CashShop.CashInventory.Add(cItem);
                        }

                        using (Packet outPacket = new Packet(MapleServerOperationCode.CashShopOperation))
                        {
                            outPacket.WriteByte(0x9A);
                            outPacket.WriteByte((byte)package.Count);

                            foreach (CashItem cItem in package)
                            {
                                outPacket.WriteBytes(cItem.ToByteArray(player.AccountID));
                            }

                            outPacket.WriteShort();

                            player.Client.Send(outPacket);
                        }  
                    }

                    player.GainCash((byte)cash, -cashItem.Price);
                    player.CashShop.ShowCash();
                    break;

                case 0x04: // Gift
                    break;

                case 0x05: // Modify wishlist
                    cs.WishList.Clear();

                    for (byte i = 0; i < 10; i++)
                    {
                        int sn = inPacket.ReadInt();
                        cashItem = CashItem.GetCashItem(sn);

                        if (cashItem != null)
                        {
                            cs.WishList.Add(sn);
                        }
                    }

                    cs.WishList.Send(true);
                    break;

                case 0x06: // Buy inventory slots
                    inPacket.Skip(1);
                    cash = inPacket.ReadInt();

                    if (inPacket.ReadByte() == 0)
                    {
                        if (player.GetCash((byte)cash) < 4000)
                        {
                            return;
                        }

                        byte type = inPacket.ReadByte();

                        if (player.GainInventorySlots((ItemType)type, 4, false))
                        {
                            using (Packet outPacket = new Packet(MapleServerOperationCode.CashShopOperation))
                            {
                                outPacket.WriteByte(0x6D);
                                outPacket.WriteByte(type);
                                outPacket.WriteShort(player.Items.MaxSlots[(ItemType)type]);

                                player.Client.Send(outPacket);
                            }

                            player.GainCash((byte)cash, -4000);
                            player.CashShop.ShowCash();
                        }
                    }
                    break;

                case 0x07: // Buy storage slots
                    break;

                case 0x08: // Buy character slot
                    break;

                case 0x0E: // Take from cash inventory
                    uniqueID = inPacket.ReadInt();
                    try
                    {
                        cashItem = cs.CashInventory[uniqueID];
                        cs.CashInventory.Remove(cashItem);
                    }
                    catch (KeyNotFoundException)
                    {
                        return;
                    }
                    break;

                case 0x0F: // Put into cash inventory
                    uniqueID = inPacket.ReadInt();
                    inPacket.Skip(4);
                    item = player.Items[uniqueID, (ItemType)inPacket.ReadByte()];

                    if (item != null)
                    {
                        player.Items.Remove(item, false);
                        cashItem = new CashItem(item.SerialNumber);
                        cashItem.UniqueID = item.UniqueID;
                        cs.CashInventory.Add(cashItem);

                        using (Packet outPacket = new Packet(MapleServerOperationCode.CashShopOperation))
                        {
                            outPacket.WriteByte(0x79);
                            outPacket.WriteBytes(cashItem.ToByteArray(player.AccountID));

                            player.Client.Send(outPacket);
                        }
                    }
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

        static bool CanBuy(CashItem cashItem, Character player, int cash)
        {
            return cashItem != null && player.GetCash((byte)cash) >= cashItem.Price;
        }
    }
}
