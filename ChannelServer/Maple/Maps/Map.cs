using System;
using Loki.Maple.Characters;
using Loki.Maple.Data;
using Loki.Net;

namespace Loki.Maple.Maps
{
    public class Map : MarshalByRefObject
    {
        public int MapleID { get; private set; }
        public int ReturnMapID { get; private set; }
        public int ForcedReturnMapID { get; private set; }
        public sbyte RegenerationRate { get; private set; }
        public byte DecreaseHP { get; private set; }
        public ushort DamagePerSecond { get; private set; }
        public int ProtectorItemID { get; private set; }
        public bool HasShip { get; private set; }
        public byte RequiredLevel { get; private set; }
        public int TimeLimit { get; private set; }
        public double SpawnRate { get; private set; }
        public bool IsTown { get; private set; }
        public bool HasClock { get; private set; }
        public bool IsEverlasting { get; private set; }
        public bool DisablesTownScroll { get; private set; }
        public bool IsSwim { get; private set; }
        public bool ShufflesReactors { get; private set; }
        public string UniqueShuffledReactor { get; private set; }
        public bool IsShop { get; private set; }
        public bool NoPartyLeaderPass { get; private set; }

        public MapCharacters Characters { get; private set; }
        public MapDrops Drops { get; private set; }
        public MapMobs Mobs { get; private set; }
        public MapNpcs Npcs { get; private set; }
        public MapReactors Reactors { get; private set; }
        public MapPortals Portals { get; private set; }
        public MapSpawnPoints SpawnPoints { get; private set; }
        public MapPlayerShops PlayerShops { get; private set; }
        public MapHiredMerchants HiredMerchants { get; private set; }
        public MapOmoks Omoks { get; private set; }

        public Map ReturnMap
        {
            get
            {
                return ChannelData.Maps[this.ReturnMapID];
            }
        }

        private bool IsObjectIDTaken(int objectId)
        {
            if (this.Characters.Contains(objectId))
            {
                return true;
            }

            if (this.Drops.Contains(objectId))
            {
                return true;
            }

            if (this.Mobs.Contains(objectId))
            {
                return true;
            }

            if (this.Npcs.Contains(objectId))
            {
                return true;
            }

            if (this.Reactors.Contains(objectId))
            {
                return true;
            }

            if (this.Portals.Contains(objectId))
            {
                return true;
            }

            if (this.PlayerShops.Contains(objectId))
            {
                return true;
            }

            if (this.HiredMerchants.Contains(objectId))
            {
                return true;
            }

            if (this.Omoks.Contains(objectId))
            {
                return true;
            }

            return false;
        }

        private int RunningObjectID = 1;

        private object AssignLock = new object();

        public int AssignObjectID()
        {
            lock (this.AssignLock)
            {
                while (this.IsObjectIDTaken(this.RunningObjectID))
                {
                    this.RunningObjectID++;

                    if (this.RunningObjectID > 100000)
                    {
                        this.RunningObjectID = 0;
                    }
                }

                return this.RunningObjectID;
            }
        }

        public Map(dynamic datum)
        {
            this.MapleID = datum.mapid;
            this.ReturnMapID = datum.return_map;
            this.ForcedReturnMapID = datum.forced_return_map;

            this.RegenerationRate = datum.regen_rate;
            this.DecreaseHP = datum.decrease_hp;
            this.DamagePerSecond = datum.damage_per_second;
            this.ProtectorItemID = datum.protect_item;
            this.HasShip = datum.ship_kind >= 0;
            this.SpawnRate = datum.mob_rate;
            this.RequiredLevel = datum.min_level_limit;
            this.TimeLimit = datum.time_limit;

            this.IsTown = datum.flags.Contains("town");
            this.HasClock = datum.flags.Contains("clock");
            this.IsEverlasting = datum.flags.Contains("everlast");
            this.DisablesTownScroll = datum.flags.Contains("scroll_disable");
            this.IsSwim = datum.flags.Contains("swim");
            this.ShufflesReactors = datum.flags.Contains("shuffle_reactors");
            this.UniqueShuffledReactor = datum.shuffle_name;
            this.IsShop = datum.flags.Contains("shop");
            this.NoPartyLeaderPass = datum.flags.Contains("no_party_leader_pass");

            this.Characters = new MapCharacters(this);
            this.Drops = new MapDrops(this);
            this.Mobs = new MapMobs(this);
            this.Npcs = new MapNpcs(this);
            this.Reactors = new MapReactors(this);
            this.Portals = new MapPortals(this);
            this.SpawnPoints = new MapSpawnPoints(this);
            this.PlayerShops = new MapPlayerShops(this);
            this.HiredMerchants = new MapHiredMerchants(this);
            this.Omoks = new MapOmoks(this);
        }

        public void Broadcast(Packet inPacket)
        {
            lock (this.Characters)
            {
                foreach (Character loopCharacter in this.Characters)
                {
                    loopCharacter.Client.Send(inPacket);
                }
            }
        }

        public void Broadcast(Character ignored, Packet inPacket)
        {
            lock (this.Characters)
            {
                foreach (Character loopCharacter in this.Characters)
                {
                    if (loopCharacter != ignored)
                    {
                        loopCharacter.Client.Send(inPacket);
                    }
                }
            }
        }

        public void Notify(string message, NoticeType type = NoticeType.Popup)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.ServerMessage))
            {
                outPacket.WriteByte((byte)type);
                outPacket.WriteString(message);
                this.Broadcast(outPacket);
            }
        }
    }
}
