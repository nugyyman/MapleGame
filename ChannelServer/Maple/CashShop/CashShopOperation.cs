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
        public static byte Operation = 94;

        public static void Handle(Character costumer, Packet inPacket)
        {
            if (!costumer.CashShop.Open)
            {
                return;
            }

            CharacterCashShop cs = costumer.CashShop;
            byte action = inPacket.ReadByte();
            CashItem cashItem = null;
            Item item = null;
            int uniqueID, cash;

            switch (action)
            {
                case 0x03: // Buy item
                case 0x22: // Buy packege
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

                    if (!CanBuy(cashItem, costumer, cash)) return;

                    if (action == 0x03) // Item
                    {
                        cashItem.UniqueID = cs.CashInventory.GenerateUniqueID();
                        costumer.CashShop.CashInventory.Add(cashItem);

                        using (Packet outPacket = new Packet(MapleServerOperationCode.CashShopOperation))
                        {
                            outPacket.WriteByte((byte)(Operation + 17));
                            outPacket.WriteBytes(cashItem.ToByteArray(costumer.AccountID));

                            costumer.Client.Send(outPacket);
                        }
                    }
                    else // Package
                    {
                        List<CashItem> package;

                        try
                        {
                            package = ChannelData.CachedCashItems.Packages[cashItem.MapleID];
                        }
                        catch (KeyNotFoundException)
                        {
                            return;
                        }

                        foreach (CashItem cItem in package)
                        {
                            cItem.UniqueID = cs.CashInventory.GenerateUniqueID();
                            costumer.CashShop.CashInventory.Add(cItem);
                        }

                        using (Packet outPacket = new Packet(MapleServerOperationCode.CashShopOperation))
                        {
                            outPacket.WriteByte((byte)(Operation + 71));
                            outPacket.WriteByte((byte)package.Count);

                            foreach (CashItem cItem in package)
                            {
                                outPacket.WriteBytes(cItem.ToByteArray(costumer.AccountID));
                            }

                            outPacket.WriteShort();

                            costumer.Client.Send(outPacket);
                        }  
                    }

                    costumer.GainCash((byte)cash, -cashItem.Price);
                    costumer.CashShop.ShowCash();
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
                        if (costumer.GetCash((byte)cash) < 4000)
                        {
                            return;
                        }

                        byte type = inPacket.ReadByte();

                        if (costumer.GainInventorySlots((ItemType)type, 4, false))
                        {
                            using (Packet outPacket = new Packet(MapleServerOperationCode.CashShopOperation))
                            {
                                outPacket.WriteByte((byte)(Operation + 26));
                                outPacket.WriteByte(type);
                                outPacket.WriteShort(costumer.Items.MaxSlots[(ItemType)type]);

                                costumer.Client.Send(outPacket);
                            }

                            costumer.GainCash((byte)cash, -4000);
                            costumer.CashShop.ShowCash();
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
                    item = costumer.Items[uniqueID, (ItemType)inPacket.ReadByte()];

                    if (item != null)
                    {
                        costumer.Items.Remove(item, false);
                        cashItem = new CashItem(item.SerialNumber);
                        cashItem.UniqueID = item.UniqueID;
                        cs.CashInventory.Add(cashItem);

                        using (Packet outPacket = new Packet(MapleServerOperationCode.CashShopOperation))
                        {
                            outPacket.WriteByte((byte)(Operation + 38));
                            outPacket.WriteBytes(cashItem.ToByteArray(costumer.AccountID));

                            costumer.Client.Send(outPacket);
                        }
                    }
                    break;

                case 0x20: // Buy crush ring
                    break;

                case 0x23: // Buy 1 meso item
                    break;

                case 0x26: // Buy friendship ring
                    break;

                case 0x2F: // I have no idea what this is
                    using (Packet outPacket = new Packet(MapleServerOperationCode.CashShopOperation))
                    {
                        outPacket.WriteInt();
                        outPacket.WriteBool(true);

                        costumer.Client.Send(outPacket);
                    }
                    break;

                case 0x2C: // New tab
                    break;

                case 0x61: // Open random box
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
