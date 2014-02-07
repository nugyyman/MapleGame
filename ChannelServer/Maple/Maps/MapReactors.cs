using System;
using System.Collections.Generic;
using Loki.Maple.Life;
using Loki.Maple.Characters;
using Loki.Maple.Data;
using Loki.Net;
using Loki.Threading;

namespace Loki.Maple.Maps
{
    public class MapReactors : MapObjects<Reactor>
    {
        public MapReactors(Map parent) : base(parent) { }

        protected override void InsertItem(int index, Reactor item)
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
                }
            }
        }

        protected override void RemoveItem(int index) // NOTE: Equivalent of reactor death.
        {
            lock (this)
            {
                Reactor item = this.GetAtIndex(index);
                item.Act(item.LastAttacker);

                List<Drop> drops = new List<Drop>();

                foreach (Loot loopLoot in item.Loots)
                {
                    if (item.LastAttacker != null)
                    {
                        if (Application.Random.Next(251) / ChannelServer.DropRate >= loopLoot.Chance) //TODO: is this the right formula?
                        {
                            if (loopLoot.QuestID == -1 || item.LastAttacker.Quests.Started.ContainsKey((ushort)loopLoot.QuestID))
                            {
                                drops.Add(new Item(loopLoot.MapleID)
                                {
                                    Dropper = item,
                                    Owner = item.LastAttacker
                                });
                            }
                        }
                    }
                }

                foreach (Loot loopLoot in item.MesoLoots)
                {
                    if (Application.Random.Next(101) <= loopLoot.Chance)
                    {
                        drops.Add(new Meso(Application.Random.Next(loopLoot.MinimumQuantity, loopLoot.MaximumQuantity + 1) * ChannelServer.MesoRate)
                        {
                            Dropper = item,
                            Owner = item.LastAttacker
                        });
                    }
                }

                item.MesoLoots.Clear();

                Point dropPosition = new Point(item.Position.X, item.Position.Y);
                dropPosition.X -= (short)(12 * drops.Count);

                foreach (Drop loopDrop in drops)
                {
                    loopDrop.Position = new Point(dropPosition.X, dropPosition.Y);
                    loopDrop.Position.Y = loopDrop.Owner.Position.Y; // the position of the reactor is wierd
                    dropPosition.X += 25;

                    this.Map.Drops.Add(loopDrop);
                }

                if(ChannelData.IsInitialized)
                {
                    using (Packet destroy = item.GetDestroyPacket())
                    {
                        item.Map.Broadcast(destroy);
                    }
                }

                base.RemoveItem(index);

                if (item.SpawnPoint != null)
                {
                    Delay.Execute((item.SpawnPoint.RespawnTime <= 0 ? 30 : item.SpawnPoint.RespawnTime) * 100, () => item.SpawnPoint.Spawn(this.Map));
                }
            }
        }
    }
}