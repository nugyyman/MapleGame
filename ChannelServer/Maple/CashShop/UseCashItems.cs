using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Maple.Characters;
using Loki.Net;
using Loki.Maple.Data;

namespace Loki.Maple.CashShop
{
    class UseCashItem
    {
        public static void Handle(Character player, Packet inPacket)
        {
            inPacket.ReadShort();
            int itemId = inPacket.ReadInt();
            int itemType = itemId / 10000;
            if (!player.Items.Contains(itemId, 1))
            {
                return;
            }
            try
            {
                switch (itemType)
                {
                    case 505: // AP/SP Reset
                        if (itemId > 5050000) // SP
                        {
                            int spTo = inPacket.ReadInt();
                            int spFrom = inPacket.ReadInt();
                            Skill toSkill = player.Skills[spTo];
                            Skill fromSkill = player.Skills[spFrom];
                            if (toSkill.CurrentLevel < new Skill(spTo).MaxLevel && fromSkill.CurrentLevel > 0)
                            {
                                toSkill.CurrentLevel++;
                                fromSkill.CurrentLevel--;
                            }
                        }
                        else // AP
                        {
                            int apTo = inPacket.ReadInt();
                            int apFrom = inPacket.ReadInt();
                            switch (apFrom)
                            {
                                case 64: // str
                                    if (player.Strength < 5)
                                        return;
                                    player.Strength--;
                                    break;
                                case 128: // dex
                                    if (player.Dexterity < 5)
                                        return;
                                    player.Dexterity--;
                                    break;
                                case 256: // int
                                    if (player.Intelligence < 5)
                                        return;
                                    player.Intelligence--;
                                    break;
                                case 512: // luk
                                    if (player.Luck < 5)
                                        return;
                                    player.Luck--;
                                    break;
                                case 2048: // hp
                                    if (player.CurrentHP < 50)
                                        return;
                                    player.CurrentHP--; // TODO: correct HP Reducing
                                    break;
                                case 8192: // mp
                                    if (player.CurrentMP < 5)
                                        return;
                                    player.CurrentMP--; // TODO: correct MP Reducing
                                    break;
                            }
                            player.DistributeAP(apTo);
                        }
                        player.Items.Remove(itemId, 1);
                        break;
                    case 557:
                        inPacket.ReadInt();
                        int slot = inPacket.ReadInt();
                        inPacket.ReadInt();
                        Item equip = player.Items[ItemType.Equipment, (sbyte)slot];
                        if (equip.ViciousHammerApplied == 2 || !player.Items.Contains(5570000, 1))
                            return;
                        equip.ViciousHammerApplied++;
                        equip.UpgradesAvailable++;
                        using (Packet outPacket = new Packet(MapleServerOperationCode.ViciousHammer))
                        {
                            outPacket.WriteByte(0x34);
                            outPacket.WriteInt(0);
                            outPacket.WriteInt(equip.ViciousHammerApplied);
                            player.Client.Send(outPacket);
                        }
                        using (Packet outPacket = new Packet(MapleServerOperationCode.ModifyInventoryItem))
                        {
                            outPacket.WriteByte(0); // could be from drop
                            outPacket.WriteByte(2); // always 2
                            outPacket.WriteByte(3); // quantity > 0 (?)
                            outPacket.WriteByte(1); // Inventory type
                            outPacket.WriteSByte(equip.Slot); // item slot
                            outPacket.WriteShort(0);
                            outPacket.WriteByte(1);
                            outPacket.WriteSByte(equip.Slot); // wtf repeat
                            outPacket.WriteBytes(equip.ToByteArray(true));
                            player.Client.Send(outPacket);
                        }
                        player.Items.Remove(itemId, 1);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
