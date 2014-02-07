using System;
using System.Collections.Generic;
using Loki.Maple.Characters;
using Loki.Maple.Data;
using Loki.Maple.Life;
using Loki.Net;
using Loki.Threading;

namespace Loki.Maple.Maps
{
    public class MapMobs : MapObjects<Mob>
    {
        public MapMobs(Map parent) : base(parent) { }

        protected override void InsertItem(int index, Mob item)
        {
            lock (this)
            {
                base.InsertItem(index, item);

                if (ChannelData.IsInitialized)
                {
                    using (Packet create = item.GetCreatePacket())
                    {
                        item.Map.Broadcast(create);
                    }

                    item.AssignController();
                }
            }
        }

        protected override void RemoveItem(int index) // NOTE: Equivalent of mob death.
        {
            lock (this)
            {
                Mob item = this.GetAtIndex(index);

                int mostDamage = 0;
                Character owner = null;

                foreach (KeyValuePair<Character, uint> attacker in item.Attackers)
                {
                    if (attacker.Key.IsAlive && attacker.Key.Client.IsAlive && attacker.Key.Map == this.Map)
                    {
                        if (attacker.Value > mostDamage)
                        {
                            owner = attacker.Key;
                        }

                        attacker.Key.GainExperience((int)Math.Min(item.Experience, (attacker.Value * item.Experience) / item.MaxHP));
                    }
                }

                item.Attackers.Clear();

                if (item.CanDrop)
                {
                    List<Drop> drops = new List<Drop>();

                    foreach (Loot loopLoot in item.Loots)
                    {
                        if ((Application.Random.Next(1000000) / ChannelServer.DropRate) <= loopLoot.Chance)
                        {
                            drops.Add(new Item(loopLoot.MapleID, (short)Application.Random.Next(loopLoot.MinimumQuantity, loopLoot.MaximumQuantity))
                            {
                                Dropper = item,
                                Owner = owner
                            });
                        }
                    }

                    Point dropPosition = new Point(item.Position.X, item.Position.Y);
                    dropPosition.X -= (short)(12 * drops.Count);

                    foreach (Drop loopDrop in drops)
                    {
                        loopDrop.Position = new Point(dropPosition.X, dropPosition.Y);
                        dropPosition.X += 25;

                        this.Map.Drops.Add(loopDrop);
                    }
                }

                if (owner != null)
                {
                    foreach (KeyValuePair<ushort, Dictionary<int, short>> loopStartedQuest in owner.Quests.Started)
                    {
                        if (loopStartedQuest.Value.ContainsKey(item.MapleID))
                        {
                            if (loopStartedQuest.Value[item.MapleID] < ChannelData.Quests[loopStartedQuest.Key].PostRequiredKills[item.MapleID])
                            {
                                loopStartedQuest.Value[item.MapleID]++;

                                using (Packet outPacket = new Packet(MapleServerOperationCode.ShowLog))
                                {
                                    outPacket.WriteByte(1);
                                    outPacket.WriteUShort(loopStartedQuest.Key);
                                    outPacket.WriteByte(1);

                                    string kills = string.Empty;

                                    foreach (int kill in loopStartedQuest.Value.Values)
                                    {
                                        kills += kill.ToString().PadLeft(3, '0');
                                    }

                                    outPacket.WriteString(kills);
                                    outPacket.WriteInt();
                                    outPacket.WriteInt();

                                    owner.Client.Send(outPacket);
                                }

                                if (owner.Quests.CanComplete(loopStartedQuest.Key, true))
                                {
                                    owner.Quests.NotifyComplete(loopStartedQuest.Key);
                                }
                            }
                        }
                    }
                }

                if (ChannelData.IsInitialized)
                {
                    item.Controller.ControlledMobs.Remove(item);

                    using (Packet destroy = item.GetDestroyPacket())
                    {
                        item.Map.Broadcast(destroy);
                    }
                }

                base.RemoveItem(index);

                if (item.SpawnPoint != null && item.CanRespawn)
                {
                    Delay.Execute(3 * 1000, () => item.SpawnPoint.Spawn(this.Map));
                }

                foreach (int summonId in item.DeathSummons)
                {
                    this.Map.Mobs.Add(new Mob(summonId) { Position = item.Position });
                }
            }
        }
    }
}
