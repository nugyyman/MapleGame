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
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
