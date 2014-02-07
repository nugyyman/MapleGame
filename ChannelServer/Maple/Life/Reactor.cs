using System;
using System.Collections.Generic;
using Loki.Maple.Maps;
using Loki.Data;
using Loki.Maple.Data;
using Loki.Net;
using Loki.Maple.Characters;

namespace Loki.Maple.Life
{
    public class Reactor : MapObject, ISpawnable
    {
        public int MapleID { get; private set; }
        public SpawnPoint SpawnPoint { get; private set; }
        public List<Loot> Loots { get; set; }
        public List<Loot> MesoLoots { get; set; }
        public Character LastAttacker { get; private set; }

        public sbyte State { get; set; }
        public int Link { get; set; }
        public ReactorState[] States { get; set; }

        public Reactor CachedReference
        {
            get
            {
                return ChannelData.CachedReactors[this.MapleID];
            }
        }

        public Reactor(dynamic datum)
            : base()
        {
            this.MapleID = datum.reactorid;
            this.State = 0;
            this.Link = datum.link;
            this.States = new ReactorState[datum.max_states];
            this.Loots = new List<Loot>();
            this.MesoLoots = new List<Loot>();
        }

        public Reactor(int mapleId)
        {
            this.MapleID = mapleId;

            this.State = this.CachedReference.State;
            this.Link = this.CachedReference.Link;
            this.States = this.CachedReference.States;
            this.Loots = this.CachedReference.Loots;
            this.MesoLoots = new List<Loot>();
        }

        public Reactor(SpawnPoint spawnPoint)
            : this(spawnPoint.MapleID)
        {
            this.SpawnPoint = spawnPoint;

            this.Position = this.SpawnPoint.Position;
            this.Position.Y -= 1;
        }

        public void Damage(Character attacker, Packet inPacket)
        {
            this.LastAttacker = attacker;
            int characterPosition = inPacket.ReadInt();
            short stance = inPacket.ReadShort();

            switch (this.States[this.State].Type)
            {
                case "plain_advance_state":
                    if (this.State == this.States.Length - 2) // Destroy (is this correct?)
                    {
                        this.State = this.States[State].NextState;
                        this.Map.Reactors.Remove(this);
                    }
                    else // Hit
                    {
                        this.State = this.States[State].NextState;

                        using (Packet outPacket = new Packet(MapleServerOperationCode.HitReactor))
                        {
                            outPacket.WriteInt(this.ObjectID);
                            outPacket.WriteSByte(this.State);
                            outPacket.WriteShort(this.Position.X);
                            outPacket.WriteShort(this.Position.Y);
                            outPacket.WriteInt(stance);

                            attacker.Client.Send(outPacket);
                        }
                    }

                    break;
            }
        }

        public Packet GetCreatePacket()
        {
            Packet spawn = new Packet(MapleServerOperationCode.SpawnReactor);

            spawn.WriteInt(this.ObjectID);
            spawn.WriteInt(this.MapleID);
            spawn.WriteSByte(this.State);
            spawn.WriteShort(this.Position.X);
            spawn.WriteShort(this.Position.Y);
            spawn.WriteByte(0);
            spawn.WriteString("");

            return spawn;
        }

        public Packet GetSpawnPacket()
        {
            return this.GetCreatePacket();
        }

        public Packet GetDestroyPacket()
        {
            Packet destroy = new Packet(MapleServerOperationCode.DestroyReactor);

            destroy.WriteInt(this.ObjectID);
            destroy.WriteSByte(this.State);
            destroy.WriteShort(this.Position.X);
            destroy.WriteShort(this.Position.Y);

            return destroy;
        }

        public virtual void Act(Character attacker)
        {
            Log.Warn("Uncoded reactor has been used: {0}", this.MapleID);
        }

        public void DropMesos(int minMeso, int maxMeso, int chance)
        {

            this.MesoLoots.Add(new Loot(minMeso, maxMeso, chance));
        }
    }

    public class ReactorState
    {
        public string Type { get; private set; }
        public int TimeOut { get; private set; }
        public int ItemId { get; private set; }
        public short ItemQuantity { get; private set; }
        public Point Left { get; private set; }
        public Point Right { get; private set; }
        public sbyte NextState { get; private set; }

        public ReactorState(dynamic datum)
        {
            this.Type = datum.event_type;
            this.TimeOut = datum.timeout;
            this.ItemId = datum.itemid;
            this.ItemQuantity = datum.quantity;
            this.Left = new Point(datum.ltx, datum.lty);
            this.Right = new Point(datum.rbx, datum.rby);
            this.NextState = datum.next_state;
        }
    }
}