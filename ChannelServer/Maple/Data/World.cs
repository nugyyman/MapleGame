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
    public static class World
    {
        public static bool IsInitialized { get; private set; }

        public static WorldMaps Maps { get; private set; }
        public static CachedItems CachedItems { get; private set; }
        public static CachedSkills CachedSkills { get; private set; }
        public static CachedMobs CachedMobs { get; private set; }
        public static AvailableStyles AvailableStyles { get; private set; }
        public static WorldCharactersHelper Characters { get; private set; }
        public static WorldNpcsHelper Npcs { get; private set; }
        public static QuestData Quests { get; private set; }
        public static CharacterCreationData CharacterCreationData { get; private set; }
        public static ForbiddenNames ForbiddenNames { get; private set; }

        public static void Initialize()
        {
            using (Database.TemporarySchema("mcdb83"))
            {
                World.IsInitialized = false;

                if (World.AvailableStyles != null)
                {
                    World.AvailableStyles.Skins.Clear();
                    World.AvailableStyles.MaleHairs.Clear();
                    World.AvailableStyles.FemaleHairs.Clear();
                    World.AvailableStyles.MaleFaces.Clear();
                    World.AvailableStyles.FemaleFaces.Clear();
                }

                if (World.CachedItems != null)
                {
                    World.CachedItems.Clear();
                }

                if (World.CachedSkills != null)
                {
                    World.CachedSkills.Clear();
                }

                if (World.CachedMobs != null)
                {
                    World.CachedMobs.Clear();
                }

                if (World.Maps != null)
                {
                    World.Maps.Clear();
                }

                if (World.Quests != null)
                {
                    World.Quests.Clear();
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

                if (World.CharacterCreationData != null)
                {
                    World.CharacterCreationData.ExplorerMaleFace.Clear();
                    World.CharacterCreationData.ExplorerMaleHair.Clear();
                    World.CharacterCreationData.ExplorerMaleHairColor.Clear();
                    World.CharacterCreationData.ExplorerMaleSkin.Clear();
                    World.CharacterCreationData.ExplorerMaleTop.Clear();
                    World.CharacterCreationData.ExplorerMaleBottom.Clear();
                    World.CharacterCreationData.ExplorerMaleShoes.Clear();
                    World.CharacterCreationData.ExplorerMaleWeapon.Clear();

                    World.CharacterCreationData.ExplorerFemaleFace.Clear();
                    World.CharacterCreationData.ExplorerFemaleHair.Clear();
                    World.CharacterCreationData.ExplorerFemaleHairColor.Clear();
                    World.CharacterCreationData.ExplorerFemaleSkin.Clear();
                    World.CharacterCreationData.ExplorerFemaleTop.Clear();
                    World.CharacterCreationData.ExplorerFemaleBottom.Clear();
                    World.CharacterCreationData.ExplorerFemaleShoes.Clear();
                    World.CharacterCreationData.ExplorerFemaleWeapon.Clear();

                    World.CharacterCreationData.CygnusMaleFace.Clear();
                    World.CharacterCreationData.CygnusMaleHair.Clear();
                    World.CharacterCreationData.CygnusMaleHairColor.Clear();
                    World.CharacterCreationData.CygnusMaleSkin.Clear();
                    World.CharacterCreationData.CygnusMaleTop.Clear();
                    World.CharacterCreationData.CygnusMaleBottom.Clear();
                    World.CharacterCreationData.CygnusMaleShoes.Clear();
                    World.CharacterCreationData.CygnusMaleWeapon.Clear();

                    World.CharacterCreationData.CygnusFemaleFace.Clear();
                    World.CharacterCreationData.CygnusFemaleHair.Clear();
                    World.CharacterCreationData.CygnusFemaleHairColor.Clear();
                    World.CharacterCreationData.CygnusFemaleSkin.Clear();
                    World.CharacterCreationData.CygnusFemaleTop.Clear();
                    World.CharacterCreationData.CygnusFemaleBottom.Clear();
                    World.CharacterCreationData.CygnusFemaleShoes.Clear();
                    World.CharacterCreationData.CygnusFemaleWeapon.Clear();

                    World.CharacterCreationData.AranMaleFace.Clear();
                    World.CharacterCreationData.AranMaleHair.Clear();
                    World.CharacterCreationData.AranMaleHairColor.Clear();
                    World.CharacterCreationData.AranMaleSkin.Clear();
                    World.CharacterCreationData.AranMaleTop.Clear();
                    World.CharacterCreationData.AranMaleBottom.Clear();
                    World.CharacterCreationData.AranMaleShoes.Clear();
                    World.CharacterCreationData.AranMaleWeapon.Clear();

                    World.CharacterCreationData.AranFemaleFace.Clear();
                    World.CharacterCreationData.AranFemaleHair.Clear();
                    World.CharacterCreationData.AranFemaleHairColor.Clear();
                    World.CharacterCreationData.AranFemaleSkin.Clear();
                    World.CharacterCreationData.AranFemaleTop.Clear();
                    World.CharacterCreationData.AranFemaleBottom.Clear();
                    World.CharacterCreationData.AranFemaleShoes.Clear();
                    World.CharacterCreationData.AranFemaleWeapon.Clear();
                }

                if (World.ForbiddenNames != null)
                {
                    World.ForbiddenNames.Clear();
                }

                Database.Test();

                DateTime dti = DateTime.Now;

                Log.Inform("Loading data...");
                Thread.Sleep(100);

                World.AvailableStyles = new AvailableStyles();
                World.CachedMobs = new CachedMobs();
                World.CachedItems = new CachedItems();
                World.CachedSkills = new CachedSkills();
                World.Maps = new WorldMaps();
                World.Quests = new QuestData();
                World.CharacterCreationData = new CharacterCreationData();
                World.ForbiddenNames = new ForbiddenNames();

                Strings.Load();
                CommandFactory.Initialize();

                List<SpawnPoint> toSpawn = new List<SpawnPoint>();

                foreach (Map loopMap in World.Maps)
                {
                    foreach (SpawnPoint loopSpawnPoint in loopMap.SpawnPoints)
                    {
                        toSpawn.Add(loopSpawnPoint);
                    }
                }

                foreach (SpawnPoint loopSpawnPoint in toSpawn)
                {
                    loopSpawnPoint.Spawn();
                }

                toSpawn = null;

                Log.Success("Maple data loaded in {0} seconds.", (DateTime.Now - dti).Seconds);

                World.Characters = new WorldCharactersHelper();
                World.Npcs = new WorldNpcsHelper();

                World.IsInitialized = true;
            }
        }

        public static void Broadcast(Packet inPacket)
        {
            foreach (Map loopMap in World.Maps)
            {
                loopMap.Broadcast(inPacket);
            }
        }

        public static string Header
        {
            set
            {
                World.Notify(value, NoticeType.Header);
            }
        }

        public static void Notify(string message, NoticeType type = NoticeType.Notice)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.ServerMessage))
            {
                outPacket.WriteByte((byte)type);
                outPacket.WriteString(message);
                World.Broadcast(outPacket);
            }
        }

        public static void Tip(string message, string header = "MapleTip")
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.YellowTip))
            {
                outPacket.WriteByte(0xFF);
                outPacket.WriteString("[{0}] {1}", header, message);
                World.Broadcast(outPacket);
            }
        }
    }
}
