using System.Collections.Generic;
using Loki.Maple.Characters;
using Loki.Maple.Data;
using Loki.Maple.Life;
using Loki.Net;

namespace Loki.Maple.Maps
{
    public class MapCharacters : MapObjects<Character>
    {
        public MapCharacters(Map parent) : base(parent) { }

        public Character this[string name]
        {
            get
            {
                lock (this)
                {
                    foreach (Character loopCharacter in this)
                    {
                        if (loopCharacter.Name.ToLower() == name.ToLower())
                        {
                            return loopCharacter;
                        }
                    }

                    throw new KeyNotFoundException();
                }
            }
        }

        protected override void InsertItem(int index, Character item)
        {
            lock (this)
            {
                foreach (Character loopCharacter in this.Map.Characters)
                {
                    using (Packet spawn = loopCharacter.GetSpawnPacket())
                    {
                        item.Client.Send(spawn);
                        foreach (Buff loopBuff in loopCharacter.Buffs)
                        {
                            item.Client.Send(loopBuff.GetForeignBuffPacket());
                        }
                    }
                }

                item.Position = this.Map.Portals[item.SpawnPoint].Position;

                base.InsertItem(index, item);
            }

            lock (this.Map.Drops)
            {
                foreach (Drop loopDrop in this.Map.Drops)
                {
                    if (loopDrop.Owner == null)
                    {
                        using (Packet spawn = loopDrop.GetSpawnPacket(item))
                        {
                            item.Client.Send(spawn);
                        }
                    }
                    else
                    {
                        using (Packet spawn = loopDrop.GetSpawnPacket())
                        {
                            item.Client.Send(spawn);
                        }
                    }
                }
            }

            lock (this.Map.Npcs)
            {
                foreach (Npc loopNpc in this.Map.Npcs)
                {
                    using (Packet spawn = loopNpc.GetSpawnPacket())
                    {
                        item.Client.Send(spawn);
                    }
                }
            }

            lock (this.Map.Mobs)
            {
                foreach (Mob loopMob in this.Map.Mobs)
                {
                    using (Packet spawn = loopMob.GetSpawnPacket())
                    {
                        item.Client.Send(spawn);
                    }
                }
            }

            lock (this.Map.Npcs)
            {
                foreach (Npc loopNpc in this.Map.Npcs)
                {
                    loopNpc.AssignController();
                }
            }

            lock (this.Map.Mobs)
            {
                foreach (Mob loopMob in this.Map.Mobs)
                {
                    loopMob.AssignController();
                }
            }

            using (Packet create = item.GetCreatePacket())
            {
                item.Map.Broadcast(item, create);
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (this)
            {
                Character item = this.GetAtIndex(index);

                if (ChannelData.IsInitialized)
                {
                    item.ControlledMobs.Clear();
                    item.ControlledNpcs.Clear();
                }

                base.RemoveItem(index);

                if (ChannelData.IsInitialized)
                {
                    lock (this.Map.Npcs)
                    {
                        foreach (Npc loopNpc in this.Map.Npcs)
                        {
                            loopNpc.AssignController();
                        }
                    }

                    lock (this.Map.Mobs)
                    {
                        foreach (Mob loopMob in this.Map.Mobs)
                        {
                            loopMob.AssignController();
                        }
                    }

                    using (Packet destroy = item.GetDestroyPacket())
                    {
                        this.Map.Broadcast(destroy);
                    }
                }
            }
        }
    }
}
