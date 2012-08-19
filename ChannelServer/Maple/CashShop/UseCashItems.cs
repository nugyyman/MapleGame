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
            inPacket.ReadInt();
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

                    case 506:
                        switch (itemId)
                        {
                            case 5062000: // miracle cube
                                Item pEquip = player.Items[(EquipmentSlot)inPacket.ReadShort()];

                                if ((byte)pEquip.Potential < 5)
                                {
                                    return;
                                }

                                switch (pEquip.Potential)
                                {
                                    case Potential.Rare:
                                        pEquip.Potential = Potential.HiddenPotential1;
                                        break;

                                    case Potential.Epic:
                                        pEquip.Potential = Potential.HiddenPotential2;
                                        break;

                                    case Potential.Unique:
                                        pEquip.Potential = Potential.HiddenPotential3;
                                        break;
                                }

                                pEquip.Potential1 = 0;
                                pEquip.Potential2 = 0;
                                pEquip.Potential3 = 0;

                                player.UpdateStatistics();

                                using(Packet outPacket = new Packet(MapleServerOperationCode.ShowMiracleCubeEffect))
                                {
                                    outPacket.WriteInt(player.ID);
                                    outPacket.WriteByte(1);
                                    outPacket.WriteInt(itemId);

                                    player.Client.Send(outPacket);
                                }

                                using (Packet outPacket = new Packet(MapleServerOperationCode.ModifyInventoryItem))
                                {
                                    outPacket.WriteByte(0); // could be from drop
                                    outPacket.WriteByte(2); // always 2
                                    outPacket.WriteByte();
                                    outPacket.WriteByte(3); // quantity > 0 (?)
                                    outPacket.WriteByte(1); // Inventory type
                                    outPacket.WriteShort((short)pEquip.Slot); // item slot
                                    outPacket.WriteByte(0);
                                    outPacket.WriteByte(1);
                                    outPacket.WriteShort((short)pEquip.Slot); // wtf repeat
                                    outPacket.WriteBytes(pEquip.ToByteArray(true));

                                    player.Client.Send(outPacket);
                                }
                                break;

                            case 5062001: // premium miracle cube
                                Item pEquip2 = player.Items[(EquipmentSlot)inPacket.ReadShort()];

                                if ((byte)pEquip2.Potential < 5)
                                {
                                    return;
                                }

                                switch (pEquip2.Potential)
                                {
                                    case Potential.Rare:
                                        pEquip2.Potential = Potential.HiddenPotential1;
                                        break;

                                    case Potential.Epic:
                                        pEquip2.Potential = Potential.HiddenPotential2;
                                        break;

                                    case Potential.Unique:
                                        pEquip2.Potential = Potential.HiddenPotential3;
                                        break;
                                }

                                pEquip2.Potential1 = 0;
                                pEquip2.Potential2 = 0;
                                pEquip2.Potential3 = 0;

                                Random rand = new Random();
                                if (rand.Next(1, 101) <= 30)
                                {
                                    pEquip2.PotentialLines = 3;
                                }

                                player.UpdateStatistics();

                                using(Packet outPacket = new Packet(MapleServerOperationCode.ShowMiracleCubeEffect))
                                {
                                    outPacket.WriteInt(player.ID);
                                    outPacket.WriteByte(1);
                                    outPacket.WriteInt(itemId);

                                    player.Map.Broadcast(outPacket);
                                }

                                using (Packet outPacket = new Packet(MapleServerOperationCode.ModifyInventoryItem))
                                {
                                    outPacket.WriteByte(0); // could be from drop
                                    outPacket.WriteByte(2); // always 2
                                    outPacket.WriteByte(3); // quantity > 0 (?)
                                    outPacket.WriteByte(1); // Inventory type
                                    outPacket.WriteShort((short)pEquip2.Slot); // item slot
                                    outPacket.WriteByte(0);
                                    outPacket.WriteByte(1);
                                    outPacket.WriteShort((short)pEquip2.Slot); // wtf repeat
                                    outPacket.WriteBytes(pEquip2.ToByteArray(true));

                                    player.Client.Send(outPacket);
                                }
                                break;
                        }

                        player.Items.Remove(itemId, 1);
                        break;

                    case 507: // Megaphones
                        bool whisper;
                        switch (itemId / 1000 % 10)
                        {
                            case 1: // Megaphone
                                if (player.Level > 9)
                                {
                                    string message = player.Name + " : " + inPacket.ReadString();
                                    using (Packet outPacket = new Packet(MapleServerOperationCode.ServerMessage))
                                    {
                                        outPacket.WriteByte((byte)2);
                                        outPacket.WriteString(message); ;

                                        player.Map.Broadcast(outPacket);
                                    }
                                }
                                else
                                {
                                    player.Notify("You may not use this until you're level 10.");
                                }
                                player.Items.Remove(itemId, 1);
                                break;

                            case 2: // Super megaphone
                                string message2 = player.Name + " : " + inPacket.ReadString();
                                byte whisper2 = inPacket.ReadByte();
                                using (Packet outPacket = new Packet(MapleServerOperationCode.ServerMessage))
                                {
                                    outPacket.WriteByte((byte)3);
                                    outPacket.WriteString(message2);
                                    outPacket.WriteByte((byte)(ChannelServer.InternalChannelID));
                                    outPacket.WriteByte((byte)(whisper2 != 0 ? 1 : 0));

                                    World.Broadcast(outPacket);
                                }
                                player.Items.Remove(itemId, 1);
                                break;

                            case 6: // Item megaphone
                                string message3 = player.Name + " : " + inPacket.ReadString();
                                whisper = inPacket.ReadByte() == 1;
                                Item item = null;
                                if (inPacket.ReadByte() == 1)
                                {
                                    int type = inPacket.ReadInt();
                                    int slottt = inPacket.ReadInt();
                                    item = player.Items[(ItemType)((byte)type), (sbyte)slottt];
                                    using (Packet outPacket = new Packet(MapleServerOperationCode.ServerMessage))
                                    {
                                        outPacket.WriteByte(8);
                                        outPacket.WriteString(message3);
                                        outPacket.WriteByte(ChannelServer.InternalChannelID);
                                        outPacket.WriteByte((byte)(whisper ? 1 : 0));
                                        if (item == null)
                                        {
                                            outPacket.WriteByte(0);
                                        }
                                        else
                                        {
                                            outPacket.WriteBytes(item.ToByteArray(false));
                                        }

                                        World.Broadcast(outPacket);
                                        
                                    }
                                    player.Items.Remove(itemId, 1);
                                }
                                break;

                            case 7: //Triple megaphone
                                int lines = (int)inPacket.ReadByte();
                                string[] message4 = new string[lines];
                                for (int i = 0; i < lines; i++)
                                {
                                    message4[i] = player.Name + " : " + inPacket.ReadString();
                                }
                                whisper = inPacket.ReadByte() == 1;
                                using (Packet outPacket = new Packet(MapleServerOperationCode.ServerMessage))
                                {
                                    outPacket.WriteByte(0x0A);
                                    if (message4[0] != null)
                                    {
                                        outPacket.WriteString(message4[0]);
                                    }
                                    outPacket.WriteByte((byte)message4.Length);
                                    for (int i = 1; i < message4.Length; i++)
                                    {
                                        if (message4[i] != null)
                                        {
                                            outPacket.WriteString(message4[i]);
                                        }
                                    }
                                    for (int i = 0; i < 10; i++)
                                    {
                                        outPacket.WriteByte(ChannelServer.InternalChannelID);
                                    }
                                    outPacket.WriteByte((byte)(whisper ? 1 : 0));
                                    outPacket.WriteByte(1);

                                    World.Broadcast(outPacket);
                                }
                                player.Items.Remove(itemId, 1);
                                break;
                        }
                        break;

                    case 539: // Avatar Megaphone
                        List<string> lines2 = new List<string>();
                        for (int i = 0; i < 4; i++)
                        {
                            lines2.Add(inPacket.ReadString());
                        }
                        whisper = inPacket.ReadByte() == 1;
                        using (Packet outPacket = new Packet(MapleServerOperationCode.AvatarMegaphone))
                        {
                            outPacket.WriteInt(itemId);
                            outPacket.WriteString(player.Name);
                            foreach (string line2 in lines2)
                            {
                                outPacket.WriteString(line2);
                            }
                            outPacket.WriteInt(ChannelServer.InternalChannelID);
                            outPacket.WriteByte((byte)(whisper ? 1 : 0));
                            outPacket.WriteBytes(player.AppearanceToByteArray());

                            World.Broadcast(outPacket);
                        }
                        player.Items.Remove(itemId, 1);
                        break;

                    case 557: // Vicious Hammer
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
                            outPacket.WriteByte();
                            outPacket.WriteByte(3); // quantity > 0 (?)
                            outPacket.WriteByte(1); // Inventory type
                            outPacket.WriteShort((short)equip.Slot); // item slot
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
