using System;
using System.Collections.Generic;
using System.Threading;
using Loki.Data;
using Loki.Maple.Commands;
using Loki.Maple.Maps;
using Loki.Net;
using Loki.Maple.Life;

namespace Loki.Maple.Data
{
    public static class ChannelData
    {
        public static bool IsInitialized { get; private set; }

        public static ChannelMaps Maps { get; private set; }
        public static CachedItems CachedItems { get; private set; }
        public static CachedSkills CachedSkills { get; private set; }
        public static CachedMobs CachedMobs { get; private set; }
        public static CachedCashItems CachedCashItems { get; private set; }
        public static AvailableStyles AvailableStyles { get; private set; }
        public static ChannelCharactersHelper Characters { get; private set; }
        public static WorldNpcsHelper Npcs { get; private set; }
        public static QuestData Quests { get; private set; }
        public static CharacterCreationData CharacterCreationData { get; private set; }
        public static ForbiddenNames ForbiddenNames { get; private set; }

        public static void Initialize()
        {
            using (Database.TemporarySchema("mcdb"))
            {
                ChannelData.IsInitialized = false;

                if (ChannelData.AvailableStyles != null)
                {
                    ChannelData.AvailableStyles.Skins.Clear();
                    ChannelData.AvailableStyles.MaleHairs.Clear();
                    ChannelData.AvailableStyles.FemaleHairs.Clear();
                    ChannelData.AvailableStyles.MaleFaces.Clear();
                    ChannelData.AvailableStyles.FemaleFaces.Clear();
                }

                if (ChannelData.CachedItems != null)
                {
                    ChannelData.CachedItems.Clear();
                }

                if (ChannelData.CachedSkills != null)
                {
                    ChannelData.CachedSkills.Clear();
                }

                if (ChannelData.CachedMobs != null)
                {
                    ChannelData.CachedMobs.Clear();
                }

                if (ChannelData.CachedCashItems != null)
                {
                    ChannelData.CachedCashItems.Clear();
                }

                if (ChannelData.Maps != null)
                {
                    ChannelData.Maps.Clear();
                }

                if (ChannelData.Quests != null)
                {
                    ChannelData.Quests.Clear();
                }

                if (MobSkill.Summons != null)
                {
                    MobSkill.Summons.Clear();
                }

                if (Strings.Items != null)
                {
                    Strings.Items.Clear();
                    Strings.Maps.Clear();
                    Strings.Mobs.Clear();
                    Strings.Npcs.Clear();
                    Strings.Quests.Clear();
                }

                if (ChannelData.CharacterCreationData != null)
                {
                    ChannelData.CharacterCreationData.Clear();
                }

                if (ChannelData.ForbiddenNames != null)
                {
                    ChannelData.ForbiddenNames.Clear();
                }

                Database.Test();

                DateTime dti = DateTime.Now;

                Log.Inform("Loading data...");
                Thread.Sleep(100);

                ChannelData.AvailableStyles = new AvailableStyles();
                ChannelData.CachedMobs = new CachedMobs();
                ChannelData.CachedItems = new CachedItems();
                ChannelData.CachedSkills = new CachedSkills();
                ChannelData.CachedCashItems = new CachedCashItems();
                ChannelData.Maps = new ChannelMaps();
                ChannelData.Quests = new QuestData();
                ChannelData.CharacterCreationData = new CharacterCreationData();
                ChannelData.ForbiddenNames = new ForbiddenNames();

                Strings.Load();
                CommandFactory.Initialize();

                //List<SpawnPoint> toSpawn = new List<SpawnPoint>();

                foreach (Map loopMap in ChannelData.Maps)
                {
                    foreach (SpawnPoint loopSpawnPoint in loopMap.SpawnPoints)
                    {
                        //toSpawn.Add(loopSpawnPoint);
                        loopSpawnPoint.Spawn(loopMap);
                    }
                }

                /*foreach (SpawnPoint loopSpawnPoint in toSpawn)
                {
                    loopSpawnPoint.Spawn();
                }*/

                //toSpawn = null;

                Log.Success("Maple data loaded in {0} seconds.", (DateTime.Now - dti).Seconds);

                ChannelData.Characters = new ChannelCharactersHelper();
                ChannelData.Npcs = new WorldNpcsHelper();

                ChannelData.IsInitialized = true;
            }
        }

        public static void Broadcast(Packet inPacket)
        {
            foreach (Map loopMap in ChannelData.Maps)
            {
                loopMap.Broadcast(inPacket);
            }
        }

        public static string Header
        {
            set
            {
                ChannelData.Notify(value, NoticeType.Header);
            }
        }

        public static void Notify(string message, NoticeType type = NoticeType.Notice)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.ServerMessage))
            {
                outPacket.WriteByte((byte)type);
                outPacket.WriteString(message);
                ChannelData.Broadcast(outPacket);
            }
        }

        public static void Tip(string message, string header = "MapleTip")
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.YellowTip))
            {
                outPacket.WriteByte(0xFF);
                outPacket.WriteString("[{0}] {1}", header, message);
                ChannelData.Broadcast(outPacket);
            }
        }

        public static Dictionary<byte, List<int>> GetCharacterStorage(int characterID)
        {
            return ChannelServer.LoginServerConnection.GetCharacterStorage(characterID);
        }
    }
}
