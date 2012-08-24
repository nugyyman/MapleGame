using System;

namespace Loki.Maple
{
    public enum WorldNames : byte
    {
        Scania,
        Bera,
        Broa,
        Windia,
        Khaini,
        Bellocan,
        Mardia,
        Kradia,
        Yellonde,
        Demethos,
        Elnido,
        Kastia,
        Judis,
        Arkenia,
        Plana,
        Galicia,
        Kalluna,
        Stius,
        Croa,
        Zenith,
        Medere
    }

    public static class WorldNameResolver
    {
        public static byte GetID(string name)
        {
            try
            {
                return (byte)Enum.Parse(typeof(WorldNames), name.ToCamel());
            }
            catch
            {
                throw new ArgumentException("The specified World name is invalid.");
            }
        }

        public static string GetName(byte id)
        {
            try
            {
                return Enum.GetName(typeof(WorldNames), id);
            }
            catch
            {
                throw new ArgumentException("The specified World ID is invalid.");
            }
        }

        public static bool IsValid(byte id)
        {
            return Enum.IsDefined(typeof(WorldNames), id);
        }

        public static bool IsValid(string name)
        {
            try
            {
                WorldNameResolver.GetID(name);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }

    public static class RegistrationResponseResolver
    {
        public static string Explain(ChannelRegistrationResponse outPacket)
        {
            switch (outPacket)
            {
                case ChannelRegistrationResponse.Valid:
                    return "The channel server can be registered.";

                case ChannelRegistrationResponse.InvalidIP:
                    return "The provided external IP is not corresponding.";

                case ChannelRegistrationResponse.InvalidCode:
                    return "The provided security code is not corresponding.";

                case ChannelRegistrationResponse.InvalidWorld:
                    return "The provided World is not valid.";

                case ChannelRegistrationResponse.WorldsFull:
                    return "Cannot register a new World as all the Worlds spots are occupied.";

                case ChannelRegistrationResponse.ChannelsFull:
                    return "Cannot register a new channel as all the channel spots are occupied.";

                default:
                    return null;
            }
        }
    }

    public enum ChangeMapMode : byte
    {
        FromDying = 1,
        FromPortal
    }

    public enum ChannelRegistrationResponse : byte
    {
        Valid,
        InvalidIP,
        InvalidCode,
        InvalidWorld,
        WorldsFull,
        ChannelsFull,
        NotConfigured,
    }

    public enum PinResponse : byte
    {
        Valid,
        Register,
        Invalid,
        Error,
        Request,
        Cancel
    }

    public enum LoginResponse : int
    {
        Valid,
        Banned = 3,
        IncorrectPassword,
        NotRegistered,
        SystemError,
        AlreadyLoggedIn,
        SystemError2,
        SystemError3,
        TooManyConnections,
        AgeLimit,
        NotMasterIP = 13,
        WrongGatewayInformationKorean,
        ProcessKorean,
        VerifyEmail,
        WrongGatewayInformation,
        VerifyEmail2 = 21,
        LicenceAgreement = 23,
        MapleEuropeNotice = 25,
        RequireFullVersion = 27
    }

    public enum CharacterDeletionResponse : byte
    {
        Valid,
        Invalid = 0x14
    }

    public enum ServerFlag : byte
    {
        None,
        Event,
        New,
        Hot
    }

    public enum ServerStatus : short
    {
        Normal,
        HighlyPopulated,
        Full
    }

    public enum Gender : byte
    {
        Male,
        Female
    }

    public enum Job : short
    {
        Beginner,

        Warrior = 100,
        Fighter = 110,
        Crusader,
        Hero,
        Page = 120,
        WhiteKnight,
        Paladin,
        Spearman = 130,
        DragonKnight,
        DarkKnight,

        Magician = 200,
        FirePoisonWizard = 210,
        FirePoisonMage,
        FirePoisonArchMage,
        IceLightningWizard = 220,
        IceLightningMage,
        IceLightningArchMage,
        Cleric = 230,
        Priest,
        Bishop,

        Bowman = 300,
        Hunter = 310,
        Ranger,
        BowMaster,
        CrossbowMan = 320,
        Sniper,
        CrossbowMaster,

        Thief = 400,
        Assassin = 410,
        Hermit,
        NightLord,
        Bandit = 420,
        ChiefBandit,
        Shadower,
        BladeRecruit = 430,
        BladeAcolyte,
        BladeSpecialist,
        BladeLord,
        BladeMaster,

        Pirate = 500,
        PirateCannoneer,
        Brawler = 510,
        Marauder,
        Buccaneer,
        Gunslinger = 520,
        Outlaw,
        Corsair,
        Cannoneer = 530,
        CannonTrooper,
        CannonMaster,

        Manager = 800,
        GM = 900,
        SuperGM = 910,

        Noblesse = 1000,

        DawnWarrior1 = 1100,
        DawnWarrior2 = 1110,
        DawnWarrior3,
        DawnWarrior4,

        BlazeWizard1 = 1200,
        BlazeWizard2 = 1210,
        BlazeWizard3,
        BlazeWizard4,

        WindArcher1 = 1300,
        WindArcher2 = 1310,
        WindArcher3,
        WindArcher4,

        NightWalker1 = 1400,
        NightWalker2 = 1410,
        NightWalker3,
        NightWalker4,

        ThunderBreaker1 = 1500,
        ThunderBreaker2 = 1510,
        ThunderBreaker3,
        ThunderBreaker4,

        Legend = 2000,
        Farmer,
        Mercedes,
        Phantom,

        Aran1 = 2100,
        Aran2 = 2110,
        Aran3,
        Aran4,

        Evan1 = 2200,
        Evan2 = 2210,
        Evan3,
        Evan4,
        Evan5,
        Evan6,
        Evan7,
        Evan8,
        Evan9,
        Evan10,

        Mercedes1 = 2300,
        Mercedes2 = 2310,
        Mercedes3,
        Mercedes4,

        Phantom1 = 2400,
        Phantom2 = 2410,
        Phantom3,
        Phantom4,

        Citizen = 3000,
        DemonSlayer,

        DemonSlayer1 = 3100,
        DemonSlayer2 = 3110,
        DemonSlayer3,
        DemonSlayer4,

        BattleMage1 = 3200,
        BattleMage2 = 3210,
        BattleMage3,
        BattleMage4,

        WildHunter1 = 3300,
        WildHunter2 = 3310,
        WildHunter3,
        WildHunter4,

        Mechanic1 = 3500,
        Mechanic2 = 3510,
        Mechanic3,
        Mechanic4
    }

    public enum ItemType : byte
    {
        Equipment = 1,
        Usable,
        Setup,
        Etcetera,
        Cash
    }

    public enum EquipmentManipulation : byte
    {
        Equip = 0x01,
        Unequip = 0x02
    }

    public enum EquipmentSlot : sbyte
    {
        Hat = -1,
        Face = -2,
        Eye = -3,
        Mantle = -4,
        Top = -5,
        Bottom = -6,
        Shoes = -7,
        Gloves = -8,
        Cape = -9,
        Shield = -10,
        Weapon = -11,
        Ring = -12,
        Necklace = -17,
        Mount = -18,
        CashHat = -101,
        CashFace = -102,
        CashEye = -103,
        CashTop = -104,
        CashOverall = -105,
        CashBottom = -106,
        CashShoes = -107,
        CashGloves = -108,
        CashCape = -109,
        CashShield = -110,
        CashWeapon = -111,
        CashRing = -112,
        CashNecklace = -117,
        CashMount = -118
    }

    public enum NoticeType : byte
    {
        Notice,
        Popup,
        Background,
        Header = 4,
        Pink,
        Blue
    }

    public enum StatisticType : int
    {
        Skin = 0x1,
        Face = 0x2,
        Hair = 0x4,
        Level = 0x10,
        Job = 0x20,
        Strength = 0x40,
        Dexterity = 0x80,
        Intelligence = 0x100,
        Luck = 0x200,
        CurrentHP = 0x400,
        MaxHP = 0x800,
        CurrentMP = 0x1000,
        MaxMP = 0x2000,
        AvailableAP = 0x4000,
        AvailableSP = 0x8000,
        Experience = 0x10000,
        Fame = 0x20000,
        Meso = 0x40000,
        Pet = 0x180008
    }

    [Flags]
    public enum ItemFlags : short
    {
        Sealed = 0x01,
        AddPreventSlipping = 0x02,
        AddPreventColdness = 0x04,
        Untradeable = 0x08,
        Scisored = 0x10
    }

    public enum WeaponType
    {
        NotAWeapon,
        Bow,
        Claw,
        Dagger,
        Crossbow,
        Axe1H,
        Sword1H,
        Blunt1H,
        Axe2H,
        Sword2H,
        Blunt2H,
        PoleArm,
        Spear,
        Staff,
        Wand,
        Knuckle,
        Gun
    }

    public enum NpcMessageType : byte
    {
        Standard,
        YesNo = 2,
        RequestText,
        RequestNumber,
        Choice,
        RequestStyle = 9,
        AcceptDecline = 0x0F
    }

    public enum ForeignEffect : byte
    {
        LevelUp,
        JobAdvance = 8
    }

    public enum ShopAction : byte
    {
        Buy,
        Sell,
        Recharge,
        Leave
    }

    // TODO: Values for this.
    public enum MobElementalModifier
    {
        Normal,
        Immune,
        Strong,
        Weak
    }

    public enum InteractionCode : byte
    {
        Create = 0,
        Invite = 2,
        Decline = 3,
        Visit = 4,
        Chat = 6,
        Exit = 0xA,
        Open = 0xB,
        SetItems = 0xE,
        SetMeso = 0xF,
        Confirm = 0x10,
        AddItem = 0x14,
        Buy = 0x15,
        RemoveItem = 0x19,
        BanPlayer = 0x1A,
        PutItem = 0x1F,
        MerchantBuy = 0x20,
        TakeItemBack = 0x24,
        MaintenanceOff = 0x25,
        MerchantOrganize = 0x26,
        MerchantClose = 0x27,
        RequestTie = 44,
        AnswerTie = 45,
        GiveUp = 46,
        ExitAfterGame = 0x32,
        CancelExit = 0x33,
        Ready = 0x34,
        UnReady = 0x35,
        Start = 0x37,
        Skip = 0x39,
        MoveOmok = 0x3A,
    }

    public enum InteractionType : byte
    {
        Omok = 1,
        Trade = 3,
        PlayerShop,
        HiredMerchant
    }

    public enum AttackType
    {
        CloseRange,
        Ranged,
        Magic
    }

    public enum QuestAction : byte
    {
        RestoreLostItem,
        Start,
        Complete,
        Forfeit,
        ScriptStart,
        ScriptEnd
    }

    public static class PacketConstants
    {
        public static readonly byte[] Item = { 0x80, 0x05 };
        public static readonly byte[] Character = { 0xFF, 0xC9, 0x9A, 0x3B };
        public static readonly byte[] Trade = { 0xB7, 0x50, 0, 0 };
    }

    public static class ExperienceTables
    {
        public static readonly int[] CharacterLevel = { 1, 15, 34, 57, 92, 135, 372, 560, 840, 1242, 1242, 1242, 1242, 1242, 1242, 1490, 1788, 2146, 2575, 3090, 3708, 4450, 5340, 6408, 7690, 9228, 11074, 13289, 15947, 19136, 19136, 19136, 19136, 19136, 19136, 22963, 27556, 33067, 39681, 47616, 51425, 55539, 59582, 64781, 69963, 75560, 81605, 88133, 95184, 102799, 111023, 119905, 129497, 139857, 151046, 163129, 176180, 190274, 205496, 221936, 239691, 258866, 279575, 301941, 326097, 352184, 380359, 410788, 443651, 479143, 479143, 479143, 479143, 479143, 479143, 512683, 548571, 586971, 628059, 672024, 719065, 769400, 823258, 880886, 942548, 1008526, 1079123, 1154662, 1235488, 1321972, 1414511, 1513526, 1619473, 1732836, 1854135, 1983924, 2122799, 2271395, 2430393, 2600520, 2782557, 2977336, 3185749, 3408752, 3647365, 3902680, 4175868, 4468179, 4780951, 5115618, 5473711, 5856871, 6266852, 6705531, 7176919, 7677163, 8214565, 8789584, 9404855, 10063195, 10063195, 10063195, 10063195, 10063195, 10063195, 10767619, 11521352, 12327847, 13190796, 14114152, 15102142, 16159292, 17290443, 18500774, 19795828, 21181536, 22664244, 24250741, 25948292, 27764673, 29708200, 31787774, 34012918, 36393823, 38941390, 41667310, 44584022, 47704904, 51044247, 54617344, 58440558, 62531397, 66908595, 71592197, 76603651, 81965907, 87703520, 93842766, 100411760, 107440583, 113887018, 120720239, 127963453, 135641260, 143779736, 152406520, 161550911, 171243966, 181518604, 192409720, 203954303, 216191561, 229163055, 242912838, 257487608, 272936864, 289313076, 306671861, 325072173, 344576503, 365251093, 387166159, 410396129, 435019897, 461121091, 488788356, 518115657, 549202596, 582154752, 617084037, 654109079, 693355624, 734956961, 779054379, 825797642, 875345501, 927866231, 983538205, 1042550497, 1105103527 };
    }

    public enum SecondaryBuffStat : long
    {
        HomingBeacon = (0x1),
        Morph = (0x2),
        Recovery = (0x4),
        MapleWarrrior = (0x8),
        Stance = (0x10),
        SharpEyes = (0x20),
        ManaReflection = (0x40),
        ShadowClaw = (0x100),
        Infinity = (0x200),
        HolyShield = (0x400),
        Hamstring = (0x800),
        Blind = (0x1000),
        Concentrate = (0x2000),
        EchoOfHero = (0x8000),
        GhostMorph = (0x20000),
        Aura = (0x40000),
        Confuse = (0x80000),
        BerserkFury = (0x8000000),
        DivineBody = (0x10000000),
        FinalAttack = (0x80000000),
        WeaponAttack = (0x100000000L),
        WeaponDefense = (0x200000000L),
        MagicAttack = (0x400000000L),
        MagicDefense = (0x800000000L),
        Accuracy = (0x1000000000L),
        Avoid = (0x2000000000L),
        Hands = (0x4000000000L),
        Speed = (0x8000000000L),
        Jump = (0x10000000000L),
        MagicGuard = (0x20000000000L),
        DarkSight = (0x40000000000L),
        Booster = (0x80000000000L),
        PowerGuard = (0x100000000000L),
        HyperBodyHP = (0x200000000000L),
        HyperBodyMP = (0x400000000000L),
        Invincible = (0x800000000000L),
        SoulArrow = (0x1000000000000L),
        Stun = (0x2000000000000L),
        Poison = (0x4000000000000L),
        Seal = (0x8000000000000L),
        Darkness = (0x10000000000000L),
        Combo = (0x20000000000000L),
        Summon = (0x20000000000000L),
        WKCharge = (0x40000000000000L),
        DragonBlood = (0x80000000000000L),
        HolySymbol = (0x100000000000000L),
        MesoUp = (0x200000000000000L),
        ShadowPartner = (0x400000000000000L),
        PickPocket = (0x800000000000000L),
        Puppet = (0x800000000000000L),
        MesoGuard = (0x1000000000000000L),
        Weaken = (0x4000000000000000L),
    }

    public enum PrimaryBuffStat : long
    {
        EnergyCharge = (0x800000000),
        Dash = (0x1000000000),
        Dash2 = (0x2000000000),
        MonsterRiding = (0x4000000000),
        SpeedInfusion = (0x8000000000),
    }

    public static class SkillNames
    {
        public enum All : int
        {
            RegularAttack = 0,
            PirateBlessing = 80000000,
            FuryUnleashed = 80000001,
            MonsterRiding = 80001000,
            YetiMount = 80001001,
            WitchsBroomstick = 80001002,
            ChargeToyTrojan = 80001003,
            Croco = 80001004,
            BlackScooter = 80001005,
            PinkScooter = 80001006,
            NimbusCloud = 80001007,
            Balrog = 80001008,
            RaceKart = 80001009,
            ZDTiger = 80001010,
            MistBalrog = 80001011,
            Shinjo = 80001012,
            OrangeMushroom = 80001013,
            Nightmare = 80001014,
            Ostrich = 80001015,
            PinkBearHotAirBalloon = 80001016,
            TransformedRobot = 80001017,
            Motorcycle = 80001018,
            PowerSuit = 80001019,
            LeonardotheLion = 80001020,
            BlueScooter = 80001021,
            SantaSled = 80001022,
            Fortune = 80001023,
            YetiMount2 = 80001024,
            Yeti = 80001025,
            WitchsBroomstick2 = 80001026,
            WoodenAirplane = 80001027,
            RedAirplane = 80001028,
            CygnusKnightsChariot = 80001029,
            Chicken = 80001030,
            Owl = 80001031,
            LowRider = 80001032,
            SpiegelmannsHotAirBalloon = 80001033,
            VirtuesBlessing = 80001034,
            VirtuesBlessing2 = 80001035,
            VirtuesBlessing3 = 80001036,
            Bjorn = 80001037,
            SpeedyCygnusKnightsChariot = 80001038,
            UnicornMount = 80001039,
            ElvenBlessing = 80001040,
            GreatFortitude = 80001041,
            GreatFortitude2 = 80001042,
            GreatFortitude3 = 80001043,
            BunnyMount = 80001044,
            Lion = 80001045,
            Unicorn = 80001046,
            LowRider2 = 80001047,
            RedTruck = 80001048,
            Gargoyle = 80001049,
            Chicken2 = 80001050,
            Owl2 = 80001051,
            Mothership = 80001052,
            OS3AMachine = 80001053,
            GiantRabbit = 80001054,
            SmallRabbit = 80001055,
            RabbitRickshaw = 80001056,
            LawOfficer = 80001057,
            NinasPentacle = 80001058,
            Frog = 80001059,
            Turtle = 80001060,
            SpiritViking = 80001061,
            NapoleanMount = 80001062,
            RedDraco = 80001063,
            Shinjo2 = 80001064,
            HotAirBalloon = 80001065,
            NadeshikoFlyHigh = 80001066,
            Pegasus = 80001067,
            Dragon = 80001068,
            MagicBroom = 80001069,
            Cloud = 80001070,
            KnightsChariot = 80001071,
            Nightmare2 = 80001072,
            Balrog2 = 80001073,
            InvisibleBalrog = 80001074,
            DragonLevel3 = 80001075,
            Owl3 = 80001076,
            Helicopter = 80001077,
            HighwayPatrolCar = 80001078,
            MonsterCarnivalATTUP = 80001079,
            MonsterCarnivalDEFUP = 80001080,
            MonsterCarnivalEXPUP = 80001081,
            Buffalo = 80001082,
            RabbitRickshaw2 = 80001083,
            GiantRabbit2 = 80001084,
            MiwokHogMount = 80001090,
            Spaceship = 80001085,
            SpaceDash = 80001086,
            SpaceBeam = 80001087,
            OS4Shuttle = 80001088,
            Soaring = 80001089,
            SebeksEye = 80001091,
            NefersEye = 80001092,
            PtahsEye = 80001093,
            NutsEye = 80001094,
            GebsEye = 80001095,
            SethsEye = 80001096,
            MaatsEye = 80001097,
            ImhotepsEye = 80001098,
            ApissEye = 80001099,
            HorussEye = 80001100,
            IsissEye = 80001101,
            AmunsEye = 80001102,
            RasEye = 80001103,
            AnubissEye = 80001104,
            HorussEye2 = 80001105,
            IsissEye2 = 80001106,
            OsirissEye = 80001107,
            RasEye2 = 80001108,
            PandaMount = 80001112,
            BunnyBuddyBuggy = 80001114,
            CrimsonNightmare = 80001121,
            할로윈호박 = 80001122,
            MSSAwesome = 80001124,
            CretaceousMount = 80001127,
            InfernalMutt = 80001131,
            UnluckyBuddyMount = 80001153,
            MaximusMount = 80001195,
            InstantDeath = 90000000,
            KnockDown = 90001001,
            Slow = 90001002,
            Poison = 90001003,
            Darkness = 90001004,
            Seal = 90001005,
            Freeze = 90001006,
            BannerofNimbleness = 91000000,
            BannerofExperience = 91000001,
            BannerofFortitude = 91000002,
            BannerofFerocity = 91000003,
            BannerofMobility = 91000004,
            BannerofLethality = 91000005,
            BannerofPlenty = 91000006,
            Herbalism = 92000000,
            Mining = 92010000,
            Smithing = 92020000,
            AccessoryCrafting = 92030000,
            Alchemy = 92040000,
            NebuliteFusion = 80001152,
        }

        public enum Beginner : int
        {
            FollowtheLead = 0000008,
            BlessingoftheFairy = 0000012,
            EmpresssBlessing = 0000073,
            EmpresssMight = 0000074,
            EmpresssMight2 = 0000080,
            ArchangelicBlessing = 0000086,
            DarkAngelicBlessing = 0000088,
            ArchangelicBlessing2 = 0000091,
            HiddenPotentialExplorer = 0000093,
            FreezingAxe = 0000097,
            IceSmash = 0000099,
            IceTempest = 0000100,
            IceChop = 0000103,
            IceCurse = 0000104,
            MasterofSwimming = 0000109,
            PirateBlessing = 0000110,
            MasterofOrganization = 0000111,
            MasterofOrganization2 = 0000112,
            WhiteAngelicBlessing = 0000180,
            WilloftheAlliance = 0000190,
            MihilesSoul = 0001196,
            OzsFlame = 0001197,
            IrenasWind = 0001198,
            EckhartsDarkness = 0001199,
            HawkeyesLightning = 0001200,
            ThreeSnails = 0001000,
            Recovery = 0001001,
            NimbleFeet = 0001002,
            LegendarySpirit = 0001003,
            MonsterRider = 0001004,
            EchoofHero = 0001005,
            Test = 0001006,
            Maker = 0001007,
            BambooRain = 0001009,
            Invincibility = 0001010,
            PowerExplosion = 0001011,
            Spaceship = 0001013,
            SpaceDash = 0001014,
            SpaceBeam = 0001015,
            YetiRider = 0001017,
            YetiMount = 0001018,
            WitchsBroomstick = 0001019,
            RageofPharaoh = 0001020,
            MagicalWoodenHorse = 0001025,
            Soaring = 0001026,
            Croking = 0001027,
            RetroScooter = 0001028,
            LovelyScooter = 0001029,
            NimbusCloud = 0001030,
            Balrog = 0001031,
            F1Machine = 0001033,
            Tiger = 0001034,
            MistBalrog = 0001035,
            Lion = 0001036,
            Unicorn = 0001037,
            LowRider = 0001038,
            RedTruck = 0001039,
            Gargoyle = 0001040,
            Shinjo = 0001042,
            OrangeMushroom = 0001044,
            Helicopter = 0001045,
            Spaceship2 = 0001046,
            SpaceDash2 = 0001047,
            SpaceBeam2 = 0001048,
            Nightmare = 0001049,
            Yeti = 0001050,
            Ostrich = 0001051,
            PinkBearHotAirBalloon = 0001052,
            Transformer = 0001053,
            Chicken = 0001054,
            KurenaiRunAway = 0001063,
            PowerSuit = 0001064,
            OS4Shuttle = 0001065,
            VisitorMeleeAttack = 0001066,
            VisitorRangeAttack = 0001067,
            Owl = 0001069,
            Mothership = 0001070,
            OS3AMachine = 0001071,
            LeonardotheLion = 0001072,
            MihilesSoulDriver = 0001075,
            OzsFlameGear = 0001076,
            IrenasWindPiercing = 0001077,
            EckhartsVampire = 0001078,
            HawkeyesSharkWave = 0001079,
            WitchsBroomstick2 = 0001081,
            BlueScooter = 0001084,
            Archangel = 0001085,
            DarkAngel = 0001087,
            SantaSled = 0001089,
            Archangel2 = 0001090,
            GiantRabbit = 0001096,
            IceDoubleJump = 0001098,
            SmallRabbit = 0001101,
            RabbitRickshaw = 0001102,
            IceKnight = 0001105,
            Fortune = 0001106,
            PartyTonight6th = 0001113,
            PartyTonight6th2 = 0001114,
            LawOfficer = 0001115,
            NinasPentacle = 0001118,
            Frog = 0001121,
            Turtle = 0001122,
            Buffalo = 0001123,
            JrTank = 0001124,
            SpiritViking = 0001129,
            PachinkoRobo = 0001130,
            RexsHyena = 0001136,
            LowRider2 = 0001138,
            NapoleanMount = 0001139,
            SoaringMount = 0001142,
            RedDraco = 0001143,
            Shinjo2 = 0001144,
            HotAirBalloon = 0001145,
            NadeshikoFlyHigh = 0001146,
            Pegasus = 0001147,
            Dragon = 0001148,
            MagicBroom = 0001149,
            Cloud = 0001150,
            KnightsChariot = 0001151,
            Nightmare2 = 0001152,
            Balrog2 = 0001153,
            InvisibleBalrog = 0001154,
            DragonLevel3 = 0001155,
            Owl2 = 0001156,
            Helicopter2 = 0001157,
            HighwayPatrolCar = 0001158,
            WhiteAngel = 0001179,
            DecentHaste = 0008000,
            DecentMysticDoor = 0008001,
            DecentSharpEyes = 0008002,
            DecentHyperBody = 0008003,
            DecentCombatOrders = 0008004,
            DecentAdvancedBlessing = 0008005,
            DecentSpeedInfusion = 0008006,
            PigsWeakness = 0009000,
            StumpsWeakness = 0009001,
            SlimesWeakness = 0009002,
        }

        public enum Warrior : int
        {
            HPBoost = 1000006,
            GuardianArmor = 1000007,
            IronBody = 1001003,
            PowerStrike = 1001004,
            SlashBlast = 1001005,
        }

        public enum Fighter : int
        {
            WeaponMastery = 1100000,
            FinalAttack = 1100002,
            PhysicalTraining = 1100009,
            Slipstream = 1101010,
            WeaponBooster = 1101004,
            Rage = 1101006,
            PowerReflection = 1101007,
            GroundSmash = 1101008,
        }

        public enum Crusader : int
        {
            SelfRecovery = 1110000,
            ChanceAttack = 1110009,
            ComboAttack = 1111002,
            Panic = 1111003,
            Coma = 1111005,
            MagicCrash = 1111007,
            Shout = 1111008,
            Brandish = 1111010,
        }

        public enum Hero : int
        {
            AdvancedComboAttack = 1120003,
            CombatMastery = 1120012,
            AdvancedFinalAttack = 1120013,
            MapleWarrior = 1121000,
            MonsterMagnet = 1121001,
            PowerStance = 1121002,
            Rush = 1121006,
            IntrepidSlash = 1121008,
            Enrage = 1121010,
            HerosWill = 1121011,
        }

        public enum Page : int
        {
            WeaponMastery = 1200000,
            FinalAttack = 1200002,
            PhysicalTraining = 1200009,
            Slipstream = 1201010,
            WeaponBooster = 1201004,
            Threaten = 1201006,
            PowerGuard = 1201007,
            GroundSmash = 1201008,
        }

        public enum WhiteKnight : int
        {
            ShieldMastery = 1210001,
            ChargedBlow = 1211002,
            FlameCharge = 1211004,
            BlizzardCharge = 1211006,
            LightningCharge = 1211008,
            MagicCrash = 1211009,
            HPRecovery = 1211010,
            CombatOrders = 1211011,
        }

        public enum Paladin : int
        {
            Achilles = 1220005,
            Guardian = 1220006,
            AdvancedCharge = 1220010,
            DivineShield = 1220013,
            MapleWarrior = 1221000,
            PowerStance = 1221002,
            HolyCharge = 1221004,
            Rush = 1221007,
            Blast = 1221009,
            HeavensHammer = 1221011,
            HerosWill = 1221012,
        }

        public enum Spearman : int
        {
            WeaponMastery = 1300000,
            FinalAttack = 1300002,
            PhysicalTraining = 1300009,
            Slipstream = 1301010,
            WeaponBooster = 1301004,
            IronWill = 1301006,
            HyperBody = 1301007,
            GroundSmash = 1301008,
        }

        public enum DragonKnight : int
        {
            ElementalResistance = 1310000,
            DragonWisdom = 1310009,
            DragonBuster = 1311001,
            DragonFury = 1311003,
            Sacrifice = 1311005,
            DragonRoar = 1311006,
            MagicCrash = 1311007,
            DragonStrength = 1311008,
        }

        public enum DarkKnight : int
        {
            Berserk = 1320006,
            AuraoftheBeholden = 1320008,
            HexoftheBeholden = 1320009,
            RevengeoftheBeholden = 1320011,
            MapleWarrior = 1321000,
            MonsterMagnet = 1321001,
            PowerStance = 1321002,
            Rush = 1321003,
            Beholden = 1321007,
            HerosWill = 1321010,
            DarkImpale = 1321012,
        }

        public enum Magician : int
        {
            MPBoost = 2000006,
            ElementalWeakness = 2000007,
            MagicGuard = 2001002,
            MagicArmor = 2001003,
            EnergyBolt = 2001004,
            MagicClaw = 2001005,
        }

        public enum FirePoisonWizard : int
        {
            MPEater = 2100000,
            SpellMastery = 2100006,
            HighWisdom = 2100007,
            Meditation = 2101001,
            Teleport = 2101002,
            Slow = 2101003,
            BlazingArrow = 2101004,
            PoisonBreath = 2101005,
        }

        public enum FirePoisonMage : int
        {
            BurningMagic = 2110000,
            ElementAmplification = 2110001,
            Explosion = 2111002,
            PoisonMist = 2111003,
            Seal = 2111004,
            SpellBooster = 2111005,
            FireDemon = 2111006,
            TeleportMastery = 2111007,
            ElementalDecrease = 2111008,
            ArcaneOverdrive = 2110009,
        }

        public enum FirePoisonArchMage : int
        {
            ArcaneAim = 2120010,
            MapleWarrior = 2121000,
            BigBang = 2121001,
            MistEruption = 2121003,
            Infinity = 2121004,
            Ifrit = 2121005,
            Paralyze = 2121006,
            MeteorShower = 2121007,
            HerosWill = 2121008,
            BuffMastery = 2121009,
        }

        public enum IceLightningWizard : int
        {
            MPEater = 2200000,
            SpellMastery = 2200006,
            HighWisdom = 2200007,
            Meditation = 2201001,
            Teleport = 2201002,
            Slow = 2201003,
            ColdBeam = 2201004,
            ThunderBolt = 2201005,
        }

        public enum IceLightningMage : int
        {
            StormMagic = 2210000,
            ElementAmplification = 2210001,
            IceStrike = 2211002,
            ThunderSpear = 2211003,
            Seal = 2211004,
            SpellBooster = 2211005,
            IceDemon = 2211006,
            TeleportMastery = 2211007,
            ElementalDecrease = 2211008,
            ArcaneOverdrive = 2210009,
        }

        public enum IceLightningArchMage : int
        {
            ArcaneAim = 2220010,
            MapleWarrior = 2221000,
            BigBang = 2221001,
            GlacierChain = 2221003,
            Infinity = 2221004,
            Elquines = 2221005,
            ChainLightning = 2221006,
            Blizzard = 2221007,
            HerosWill = 2221008,
            BuffMastery = 2221009,
        }

        public enum Cleric : int
        {
            MPEater = 2300000,
            SpellMastery = 2300006,
            HighWisdom = 2300007,
            Teleport = 2301001,
            Heal = 2301002,
            Invincible = 2301003,
            Bless = 2301004,
            HolyArrow = 2301005,
        }

        public enum Priest : int
        {
            HolyFocus = 2310008,
            Dispel = 2311001,
            MysticDoor = 2311002,
            HolySymbol = 2311003,
            ShiningRay = 2311004,
            Doom = 2311005,
            MagicBooster = 2311006,
            TeleportMastery = 2311007,
            HolyMagicShell = 2311009,
            ArcaneOverdrive = 2310010,
        }

        public enum Bishop : int
        {
            ArcaneAim = 2320011,
            MapleWarrior = 2321000,
            BigBang = 2321001,
            ManaReflection = 2321002,
            Bahamut = 2321003,
            Infinity = 2321004,
            AdvancedBlessing = 2321005,
            Resurrection = 2321006,
            AngelRay = 2321007,
            Genesis = 2321008,
            HerosWill = 2321009,
            BuffMastery = 2321010,
        }

        public enum Bowman : int
        {
            CriticalShot = 3000001,
            ArcheryMastery = 3000002,
            NaturesBalance = 3000006,
            ArrowBlow = 3001004,
            DoubleShot = 3001005,
        }

        public enum Hunter : int
        {
            BowMastery = 3100000,
            FinalAttackBow = 3100001,
            PhysicalTraining = 3100006,
            BowBooster = 3101002,
            DoubleJump = 3101003,
            SoulArrowBow = 3101004,
            ArrowBombBow = 3101005,
            SilverHawk = 3101007,
        }

        public enum Ranger : int
        {
            MortalBlow = 3110001,
            EvasionBoost = 3110007,
            Concentrate = 3111000,
            Puppet = 3111002,
            RoastingShot = 3111003,
            ArrowRain = 3111004,
            Phoenix = 3111005,
            Strafe = 3111006,
            DrainArrow = 3111008,
        }

        public enum BowMaster : int
        {
            BowExpert = 3120005,
            SpiritLinkPhoenix = 3120006,
            AdvancedFinalAttack = 3120008,
            BroilerShot = 3120010,
            Marksmanship = 3120011,
            ElitePuppet = 3120012,
            MapleWarrior = 3121000,
            SharpEyes = 3121002,
            Hurricane = 3121004,
            IllusionStep = 3121007,
            HerosWill = 3121009,
        }

        public enum CrossbowMan : int
        {
            CrossbowMastery = 3200000,
            FinalAttackCrossbow = 3200001,
            PhysicalTraining = 3200006,
            CrossbowBooster = 3201002,
            DoubleJump = 3201003,
            SoulArrowCrossbow = 3201004,
            IronArrowCrossbow = 3201005,
            GoldenEagle = 3201007,
        }

        public enum Sniper : int
        {
            MortalBlow = 3210001,
            EvasionBoost = 3210007,
            Concentrate = 3211000,
            Puppet = 3211002,
            SnapfreezeShot = 3211003,
            ArrowEruption = 3211004,
            Frostprey = 3211005,
            Strafe = 3211006,
            DragonsBreath = 3211008,
        }

        public enum CrossbowMaster : int
        {
            MarksmanBoost = 3220004,
            SpiritLinkFrostprey = 3220005,
            Marksmanship = 3220009,
            UltimateStrafe = 3220010,
            ElitePuppet = 3220012,
            MapleWarrior = 3221000,
            PiercingArrow = 3221001,
            SharpEyes = 3221002,
            IllusionStep = 3221006,
            Snipe = 3221007,
            HerosWill = 3221008,
        }

        public enum Thief : int
        {
            NimbleBody = 4000000,
            KeenEyes = 4000001,
            MagicTheft = 4000010,
            Disorder = 4001002,
            DarkSight = 4001003,
            DoubleStab = 4001334,
            LuckySeven = 4001344,
        }

        public enum Assassin : int
        {
            ClawMastery = 4100000,
            CriticalThrow = 4100001,
            ShadowResistance = 4100006,
            ClawBooster = 4101003,
            Haste = 4101004,
            Drain = 4101005,
        }

        public enum Hermit : int
        {
            Alchemist = 4110000,
            MesoUp = 4111001,
            ShadowPartner = 4111002,
            ShadowWeb = 4111003,
            ShadowMeso = 4111004,
            Avenger = 4111005,
            FlashJump = 4111006,
            DarkFlare = 4111007,
        }

        public enum NightLord : int
        {
            ShadowShifter = 4120002,
            VenomousStar = 4120005,
            ExpertThrowingStarHandling = 4120010,
            MapleWarrior = 4121000,
            Taunt = 4121003,
            NinjaAmbush = 4121004,
            ShadowStars = 4121006,
            TripleThrow = 4121007,
            NinjaStorm = 4121008,
            HerosWill = 4121009,
        }

        public enum Bandit : int
        {
            DaggerMastery = 4200000,
            ShadowResistance = 4200006,
            DaggerBooster = 4201002,
            Haste = 4201003,
            Steal = 4201004,
            SavageBlow = 4201005,
        }

        public enum ChiefBandit : int
        {
            ShieldMastery = 4210000,
            Chakra = 4211001,
            Assaulter = 4211002,
            Pickpocket = 4211003,
            BandofThieves = 4211004,
            MesoGuard = 4211005,
            MesoExplosion = 4211006,
            DarkFlare = 4211007,
            ShadowPartner = 4211008,
            FlashJump = 4211009,
        }

        public enum Shadower : int
        {
            ShadowShifter = 4220002,
            VenomousStab = 4220005,
            MesoMastery = 4220009,
            MapleWarrior = 4221000,
            Assassinate = 4221001,
            Taunt = 4221003,
            NinjaAmbush = 4221004,
            Smokescreen = 4221006,
            BoomerangStep = 4221007,
            HerosWill = 4221008,
        }

        public enum BladeRecruit : int
        {
            KataraMastery = 4300000,
            TripleStab = 4301001,
            KataraBooster = 4301002,
        }

        public enum BladeAcolyte : int
        {
            ShadowResistance = 4310004,
            SelfHaste = 4311001,
            FatalBlow = 4311002,
            SlashStorm = 4311003,
        }

        public enum BladeSpecialist : int
        {
            TornadoSpin = 4321000,
            TornadoSpinAttack = 4321001,
            Flashbang = 4321002,
            FlashJump = 4321003,
            UpperStab = 4321004,
        }

        public enum BladeLord : int
        {
            AdvancedDarkSight = 4330001,
            LifeDrain = 4330007,
            BloodyStorm = 4331000,
            MirrorImage = 4331002,
            OwlSpirit = 4331003,
            UpperStab = 4331004,
            FlyingAssaulter = 4331005,
            ChainsofHell = 4331006,
        }

        public enum BladeMaster : int
        {
            Venom = 4340001,
            Sharpness = 4340010,
            MapleWarrior = 4341000,
            FinalCut = 4341002,
            MonsterBomb = 4341003,
            BladeFury = 4341004,
            ChainsofHell = 4341005,
            MirroredTarget = 4341006,
            Thorns = 4341007,
            HerosWill = 4341008,
            PhantomBlow = 4341009,
        }

        public enum Pirate : int
        {
            BulletTime = 5000000,
            FortunesFavor = 5000006,
            FlashFist = 5001001,
            SommersaultKick = 5001002,
            DoubleShot = 5001003,
            Dash = 5001005,
        }

        public enum PirateCannoneer : int
        {
            CannonBoost = 5010003,
            FortunesFavor = 5010004,
            CannonBlaster = 5011000,
            CannonStrike = 5011001,
            BlastBack = 5011002,
        }

        public enum Brawler : int
        {
            KnuckleMastery = 5100001,
            CriticalPunch = 5100008,
            HPBoost = 5100009,
            BackspinBlow = 5101002,
            DoubleUppercut = 5101003,
            CorkscrewBlow = 5101004,
            MPRecovery = 5101005,
            KnuckleBooster = 5101006,
            OakBarrel = 5101007,
        }

        public enum Marauder : int
        {
            StunMastery = 5110000,
            EnergyCharge = 5110001,
            BrawlingMastery = 5110008,
            EnergyBlast = 5111002,
            EnergyDrain = 5111004,
            Transformation = 5111005,
            Shockwave = 5111006,
            RolloftheDice = 5111007,
        }

        public enum Buccaneer : int
        {
            PiratesRevenge = 5120011,
            MapleWarrior = 5121000,
            DragonStrike = 5121001,
            EnergyOrb = 5121002,
            SuperTransformation = 5121003,
            Demolition = 5121004,
            Snatch = 5121005,
            Barrage = 5121007,
            PiratesRage = 5121008,
            SpeedInfusion = 5121009,
            TimeLeap = 5121010,
        }

        public enum Gunslinger : int
        {
            GunMastery = 5200000,
            CriticalShot = 5200007,
            InvisibleShot = 5201001,
            Grenade = 5201002,
            GunBooster = 5201003,
            BlankShot = 5201004,
            Wings = 5201005,
            RecoilShot = 5201006,
        }

        public enum Outlaw : int
        {
            BurstFire = 5210000,
            Octopus = 5211001,
            Gaviota = 5211002,
            Flamethrower = 5211004,
            IceSplitter = 5211005,
            HomingBeacon = 5211006,
            RolloftheDice = 5211007,
        }

        public enum Corsair : int
        {
            ElementalBoost = 5220001,
            WrathoftheOctopi = 5220002,
            Bullseye = 5220011,
            PiratesRevenge = 5220012,
            MapleWarrior = 5221000,
            AirStrike = 5221003,
            RapidFire = 5221004,
            Battleship = 5221006,
            BattleshipCannon = 5221007,
            BattleshipTorpedo = 5221008,
            Hypnotize = 5221009,
            HerosWill = 5221010,
        }

        public enum Cannoneer : int
        {
            CriticalFire = 5300004,
            CannonMastery = 5300005,
            PirateTraining = 5300008,
            ScatterShot = 5301000,
            BarrelBomb = 5301001,
            CannonBooster = 5301002,
            MonkeyMagic = 5301003,
        }

        public enum CannonTrooper : int
        {
            ReinforcedCannon = 5310006,
            PirateRush = 5310007,
            MonkeyWave = 5310008,
            CounterCrush = 5310009,
            CannonSpike = 5311000,
            MonkeyMadness = 5311001,
            MonkeyWave2 = 5311002,
            CannonJump = 5311003,
            BarrelRoulette = 5311004,
            LuckoftheDie = 5311005,
        }

        public enum CannonMaster : int
        {
            DoubleDown = 5320007,
            MegaMonkeyMagic = 5320008,
            CannonOverload = 5320009,
            CannonBazooka = 5321000,
            NautilusStrike = 5321001,
            AnchorsAweigh = 5321003,
            MonkeyMilitia = 5321004,
            MapleWarrior = 5321005,
            HerosWill = 5321006,
            PiratesSpirit = 5321010,
            CannonBarrage = 5321012,
        }

        public enum Manager : int
        {
            MacroTest = 8001000,
            Teleport = 8001001,
        }

        public enum GM : int
        {
            HasteNormal = 9001000,
            SuperDragonRoar = 9001001,
            Teleport = 9001002,
            Bless = 9001003,
            Hide = 9001004,
            Resurrection = 9001005,
            SuperDragonRoar2 = 9001006,
            Teleport2 = 9001007,
            HyperBody = 9001008,
            ADMINANTIMACRO = 9001009,
        }

        public enum SuperGM : int
        {
            HealDispel = 9101000,
            HasteSuper = 9101001,
            HolySymbol = 9101002,
            Bless = 9101003,
            Hide = 9101004,
            Resurrection = 9101005,
            SuperDragonRoar = 9101006,
            Teleport = 9101007,
            HyperBody = 9101008,
        }

        public enum Noblesse : int
        {
            BlessingoftheFairy = 10000012,
            Helper = 10000013,
            FollowtheLead = 10000018,
            EmpresssBlessing = 10000073,
            EmpresssShout = 10000074,
            ArchangelicBlessing = 10000086,
            DarkAngelicBlessing = 10000088,
            ArchangelicBlessing2 = 10000091,
            HiddenPotentialCygnusKnight = 10000093,
            FreezingAxe = 10000097,
            IceSmash = 10000099,
            IceTempest = 10000100,
            IceChop = 10000103,
            IceCurse = 10000104,
            WhiteAngelicBlessing = 10000180,
            WilloftheAlliance = 10000190,
            BenedictionoftheFairy = 10000201,
            NobleMind = 10000202,
            ThreeSnails = 10001000,
            Recovery = 10001001,
            NimbleFeet = 10001002,
            LegendarySpirit = 10001003,
            MonsterRider = 10001004,
            EchoofHero = 10001005,
            JumpDown = 10001006,
            Maker = 10001007,
            BambooThrust = 10001009,
            InvincibleBarrier = 10001010,
            MeteoShower = 10001011,
            Spaceship = 10001014,
            SpaceDash = 10001015,
            SpaceBeam = 10001016,
            YetiRider = 10001019,
            RageofPharaoh = 10001020,
            YetiMount = 10001022,
            WitchsBroomstick = 10001023,
            MagicalWoodenHorse = 10001025,
            Soaring = 10001026,
            Croking = 10001027,
            RetroScooter = 10001028,
            LovelyScooter = 10001029,
            NimbusCloud = 10001030,
            Balrog = 10001031,
            F1Machine = 10001033,
            Tiger = 10001034,
            MistBalrog = 10001035,
            Lion = 10001036,
            Unicorn = 10001037,
            LowRider = 10001038,
            RedTruck = 10001039,
            Gargoyle = 10001040,
            Shinjo = 10001042,
            OrangeMushroom = 10001044,
            Helicopter = 10001045,
            Spaceship2 = 10001046,
            SpaceDash2 = 10001047,
            SpaceBeam2 = 10001048,
            Nightmare = 10001049,
            Yeti = 10001050,
            Ostrich = 10001051,
            PinkBearHotAirBalloon = 10001052,
            Transformer = 10001053,
            Chicken = 10001054,
            KurenaiRunAway = 10001063,
            PowerSuit = 10001064,
            OS4Shuttle = 10001065,
            VisitorMeleeAttack = 10001066,
            VisitorRangeAttack = 10001067,
            Owl = 10001069,
            Mothership = 10001070,
            OS3AMachine = 10001071,
            LeonardotheLion = 10001072,
            EmpresssPrayer = 10001075,
            WitchsBroomstick2 = 10001081,
            BlueScooter = 10001084,
            Archangel = 10001085,
            DarkAngel = 10001087,
            SantaSled = 10001089,
            Archangel2 = 10001090,
            GiantRabbit = 10001096,
            IceDoubleJump = 10001098,
            SmallRabbit = 10001101,
            RabbitRickshaw = 10001102,
            IceKnight = 10001105,
            Fortune = 10001106,
            PartyTonight6th = 10001113,
            PartyTonight6th2 = 10001114,
            LawOfficer = 10001115,
            NinasPentacle = 10001118,
            Frog = 10001121,
            Turtle = 10001122,
            Buffalo = 10001123,
            JrTank = 10001124,
            SpiritViking = 10001129,
            PachinkoRobo = 10001130,
            RexsHyena = 10001136,
            LowRider2 = 10001138,
            NapoleanMount = 10001139,
            SoaringMount = 10001142,
            RedDraco = 10001143,
            Shinjo2 = 10001144,
            HotAirBalloon = 10001145,
            NadeshikoFlyHigh = 10001146,
            Pegasus = 10001147,
            Dragon = 10001148,
            MagicBroom = 10001149,
            Cloud = 10001150,
            KnightsChariot = 10001151,
            Nightmare2 = 10001152,
            Balrog2 = 10001153,
            InvisibleBalrog = 10001154,
            DragonLevel3 = 10001155,
            Owl2 = 10001156,
            Helicopter2 = 10001157,
            HighwayPatrolCar = 10001158,
            BalrogsHellfire = 10001162,
            HorntailsFlameBreath = 10001163,
            RexsCharge = 10001164,
            AnisJudgment = 10001165,
            DragonRidersEnergyBreath = 10001171,
            VonLeonsLionSlash = 10001172,
            ZakumsToweringInferno = 10001173,
            SpiritofRocksDoomStrike = 10001174,
            MuGongsAbsoluteDestruction = 10001175,
            PinkBeansZoneofIncrediblePain = 10001176,
            WhiteAngel = 10001179,
            DecentHaste = 10008000,
            DecentMysticDoor = 10008001,
            DecentSharpEyes = 10008002,
            DecentHyperBody = 10008003,
            DecentCombatOrders = 10008004,
            DecentAdvancedBlessing = 10008005,
            DecentSpeedInfusion = 10008006,
            PigsWeakness = 10009000,
            StumpsWeakness = 10009001,
            SlimesWeakness = 10009002,
        }

        public enum DawnWarrior1 : int
        {
            HPBoost = 11000005,
            GuardianArmor = 11000006,
            IronBody = 11001001,
            PowerStrike = 11001002,
            SlashBlast = 11001003,
            Soul = 11001004,
        }

        public enum DawnWarrior2 : int
        {
            SwordMastery = 11100000,
            SwordBooster = 11101001,
            FinalAttack = 11101002,
            Rage = 11101003,
            SoulBlade = 11101004,
            SoulRush = 11101005,
            PowerReflection = 11101006,
            PhysicalTraining = 11100007,
        }

        public enum DawnWarrior3 : int
        {
            SelfRecovery = 11110000,
            Advancedcombo = 11110005,
            ComboAttack = 11111001,
            Panic = 11111002,
            Coma = 11111003,
            Brandish = 11111004,
            SoulDriver = 11111006,
            SoulCharge = 11111007,
            MagicCrash = 11111008,
        }

        public enum DawnWarrior4 : int
        {
        }

        public enum BlazeWizard1 : int
        {
            MPBoost = 12000005,
            ElementalWeakness = 12000006,
            MagicGuard = 12001001,
            MagicArmor = 12001002,
            MagicClaw = 12001003,
            Flame = 12001004,
        }

        public enum BlazeWizard2 : int
        {
            SpellMastery = 12100007,
            HighWisdom = 12100008,
            Meditation = 12101000,
            Slow = 12101001,
            FireArrow = 12101002,
            Teleport = 12101003,
            SpellBooster = 12101004,
            ElementalReset = 12101005,
            FirePillar = 12101006,
        }

        public enum BlazeWizard3 : int
        {
            MagicCritical = 12110000,
            ElementAmplification = 12110001,
            Seal = 12111002,
            MeteorShower = 12111003,
            Ifrit = 12111004,
            FlameGear = 12111005,
            FireStrike = 12111006,
            TeleportMastery = 12111007,
        }

        public enum BlazeWizard4 : int
        {
        }

        public enum WindArcher1 : int
        {
            CriticalShot = 13000000,
            ArcheryMastery = 13000001,
            NaturesBalance = 13000005,
            DoubleShot = 13001003,
            Storm = 13001004,
        }

        public enum WindArcher2 : int
        {
            BowMastery = 13100000,
            BowBooster = 13101001,
            FinalAttack = 13101002,
            SoulArrow = 13101003,
            DoubleJump = 13101004,
            StormSpike = 13101005,
            WindWalk = 13101006,
            Strafe = 13101007,
            PhysicalTraining = 13100008,
        }

        public enum WindArcher3 : int
        {
            BowExpert = 13110003,
            EvasionBoost = 13110008,
            MortalBlow = 13110009,
            ArrowRain = 13111000,
            Concentrate = 13111001,
            Hurricane = 13111002,
            Puppet = 13111004,
            EagleEye = 13111005,
            WindPiercing = 13111006,
            WindShot = 13111007,
        }

        public enum WindArcher4 : int
        {
        }

        public enum NightWalker1 : int
        {
            NimbleBody = 14000000,
            KeenEyes = 14000001,
            MagicTheft = 14000006,
            Disorder = 14001002,
            DarkSight = 14001003,
            LuckySeven = 14001004,
            Darkness = 14001005,
        }

        public enum NightWalker2 : int
        {
            ClawMastery = 14100000,
            CriticalThrow = 14100001,
            Vanish = 14100005,
            ClawBooster = 14101002,
            Haste = 14101003,
            FlashJump = 14101004,
            Vampire = 14101006,
            PhysicalTraining = 14100010,
        }

        public enum NightWalker3 : int
        {
            Alchemist = 14110003,
            Venom = 14110004,
            ShadowPartner = 14111000,
            ShadowWeb = 14111001,
            Avenger = 14111002,
            Triplethrow = 14111005,
            PoisonBomb = 14111006,
            DarkFlare = 14111010,
        }

        public enum NightWalker4 : int
        {
        }

        public enum ThunderBreaker1 : int
        {
            QuickMotion = 15000000,
            FortunesFavor = 15000005,
            Straight = 15001001,
            SomersaultKick = 15001002,
            Dash = 15001003,
            Lightning = 15001004,
        }

        public enum ThunderBreaker2 : int
        {
            KnuckleMastery = 15100001,
            EnergyCharge = 15100004,
            HPBoost = 15100007,
            PirateTraining = 15100009,
            KnuckleBooster = 15101002,
            CorkscrewBlow = 15101003,
            EnergyBlast = 15101005,
            LightningCharge = 15101006,
        }

        public enum ThunderBreaker3 : int
        {
            CriticalPunch = 15110000,
            EnergyDrain = 15111001,
            Transformation = 15111002,
            Shockwave = 15111003,
            Barrage = 15111004,
            SpeedInfusion = 15111005,
            Spark = 15111006,
            SharkWave = 15111007,
            RolloftheDice = 15111011,
        }

        public enum ThunderBreaker4 : int
        {
        }

        public enum Legend : int
        {
            BlessingoftheFairy = 20000012,
            TutorialSkill = 20000014,
            TutorialSkill2 = 20000015,
            TutorialSkill3 = 20000016,
            TutorialSkill4 = 20000017,
            TutorialSkill5 = 20000018,
            FollowtheLead = 20000024,
            EmpresssBlessing = 20000073,
            ArchangelicBlessing = 20000086,
            DarkAngelicBlessing = 20000088,
            ArchangelicBlessing2 = 20000091,
            HiddenPotentialHero = 20000093,
            FreezingAxe = 20000097,
            IceSmash = 20000099,
            IceTempest = 20000100,
            IceChop = 20000103,
            IceCurse = 20000104,
            WhiteAngelicBlessing = 20000180,
            WilloftheAlliance = 20000190,
            RegainedMemory = 20000194,
            ThreeSnails = 20001000,
            Recovery = 20001001,
            AgileBody = 20001002,
            LegendarySpirit = 20001003,
            MonsterRider = 20001004,
            EchoofHero = 20001005,
            JumpDown = 20001006,
            Maker = 20001007,
            BambooThrust = 20001009,
            InvincibleBarrier = 20001010,
            MeteoShower = 20001011,
            Helper = 20001013,
            YetiRider = 20001019,
            RageofPharaoh = 20001020,
            YetiMount = 20001022,
            WitchsBroomstick = 20001023,
            MagicalWoodenHorse = 20001025,
            Soaring = 20001026,
            Croking = 20001027,
            RetroScooter = 20001028,
            LovelyScooter = 20001029,
            NimbusCloud = 20001030,
            Balrog = 20001031,
            F1Machine = 20001033,
            Tiger = 20001034,
            MistBalrog = 20001035,
            Lion = 20001036,
            Unicorn = 20001037,
            LowRider = 20001038,
            RedTruck = 20001039,
            Gargoyle = 20001040,
            Shinjo = 20001042,
            OrangeMushroom = 20001044,
            Helicopter = 20001045,
            Spaceship = 20001046,
            SpaceDash = 20001047,
            SpaceBeam = 20001048,
            Nightmare = 20001049,
            Yeti = 20001050,
            Ostrich = 20001051,
            PinkBearHotAirBalloon = 20001052,
            Transformer = 20001053,
            Chicken = 20001054,
            KurenaiRunAway = 20001063,
            PowerSuit = 20001064,
            OS4Shuttle = 20001065,
            VisitorMeleeAttack = 20001066,
            VisitorRangeAttack = 20001067,
            Owl = 20001069,
            Mothership = 20001070,
            OS3AMachine = 20001071,
            LeonardotheLion = 20001072,
            WitchsBroomstick2 = 20001081,
            BlueScooter = 20001084,
            Archangel = 20001085,
            DarkAngel = 20001087,
            SantaSled = 20001089,
            Archangel2 = 20001090,
            GiantRabbit = 20001096,
            IceDoubleJump = 20001098,
            SmallRabbit = 20001101,
            RabbitRickshaw = 20001102,
            IceKnight = 20001105,
            Fortune = 20001106,
            PartyTonight6th = 20001113,
            PartyTonight6th2 = 20001114,
            LawOfficer = 20001115,
            NinasPentacle = 20001118,
            Frog = 20001121,
            Turtle = 20001122,
            Buffalo = 20001123,
            JrTank = 20001124,
            SpiritViking = 20001129,
            PachinkoRobo = 20001130,
            RexsHyena = 20001136,
            LowRider2 = 20001138,
            NapoleanMount = 20001139,
            SoaringMount = 20001142,
            RedDraco = 20001143,
            Shinjo2 = 20001144,
            HotAirBalloon = 20001145,
            NadeshikoFlyHigh = 20001146,
            Pegasus = 20001147,
            Dragon = 20001148,
            MagicBroom = 20001149,
            Cloud = 20001150,
            KnightsChariot = 20001151,
            Nightmare2 = 20001152,
            Balrog2 = 20001153,
            InvisibleBalrog = 20001154,
            DragonLevel3 = 20001155,
            Owl2 = 20001156,
            Helicopter2 = 20001157,
            HighwayPatrolCar = 20001158,
            BalrogsHellfire = 20001162,
            HorntailsFlameBreath = 20001163,
            RexsCharge = 20001164,
            AnisJudgment = 20001165,
            DragonRidersEnergyBreath = 20001171,
            VonLeonsLionSlash = 20001172,
            ZakumsToweringInferno = 20001173,
            SpiritofRocksDoomStrike = 20001174,
            MuGongsAbsoluteDestruction = 20001175,
            PinkBeansZoneofIncrediblePain = 20001176,
            WhiteAngel = 20001179,
            DecentHaste = 20008000,
            DecentMysticDoor = 20008001,
            DecentSharpEyes = 20008002,
            DecentHyperBody = 20008003,
            DecentCombatOrders = 20008004,
            DecentAdvancedBlessing = 20008005,
            DecentSpeedInfusion = 20008006,
            PigsWeakness = 20009000,
            StumpsWeakness = 20009001,
            SlimesWeakness = 20009002,
        }

        public enum Farmer : int
        {
            BlessingoftheFairy = 20010012,
            DragonFlight = 20010022,
            EmpresssBlessing = 20010073,
            ArchangelicBlessing = 20010086,
            DarkAngelicBlessing = 20010088,
            ArchangelicBlessing2 = 20010091,
            HiddenPotentialHero = 20010093,
            FreezingAxe = 20010097,
            IceSmash = 20010099,
            IceTempest = 20010100,
            IceChop = 20010103,
            IceCurse = 20010104,
            WhiteAngelicBlessing = 20010180,
            WilloftheAlliance = 20010190,
            InheritedWill = 20010194,
            ThreeSnails = 20011000,
            Recover = 20011001,
            NimbleFeet = 20011002,
            LegendarySpirit = 20011003,
            MonsterRider = 20011004,
            HerosEcho = 20011005,
            JumpDown = 20011006,
            Maker = 20011007,
            BambooThrust = 20011009,
            InvincibleBarrier = 20011010,
            MeteoShower = 20011011,
            YetiRider = 20011018,
            WitchsBroomstick = 20011019,
            RageofPharaoh = 20011020,
            FollowtheLead = 20011024,
            MagicalWoodenHorse = 20011025,
            Soaring = 20011026,
            Croking = 20011027,
            RetroScooter = 20011028,
            LovelyScooter = 20011029,
            NimbusCloud = 20011030,
            Balrog = 20011031,
            F1Machine = 20011033,
            Tiger = 20011034,
            MistBalrog = 20011035,
            Lion = 20011036,
            Unicorn = 20011037,
            LowRider = 20011038,
            RedTruck = 20011039,
            Gargoyle = 20011040,
            Shinjo = 20011042,
            OrangeMushroom = 20011044,
            Helicopter = 20011045,
            Spaceship = 20011046,
            SpaceDash = 20011047,
            SpaceBeam = 20011048,
            Nightmare = 20011049,
            Yeti = 20011050,
            Ostrich = 20011051,
            PinkBearHotAirBalloon = 20011052,
            Transformer = 20011053,
            Chicken = 20011054,
            KurenaiRunAway = 20011063,
            PowerSuit = 20011064,
            OS4Shuttle = 20011065,
            VisitorMeleeAttack = 20011066,
            VisitorRangeAttack = 20011067,
            Owl = 20011069,
            Mothership = 20011070,
            OS3AMachine = 20011071,
            LeonardotheLion = 20011072,
            WitchsBroomstick2 = 20011081,
            BlueScooter = 20011084,
            Archangel = 20011085,
            DarkAngel = 20011087,
            SantaSled = 20011089,
            Archangel2 = 20011090,
            GiantRabbit = 20011096,
            IceDoubleJump = 20011098,
            SmallRabbit = 20011101,
            RabbitRickshaw = 20011102,
            IceKnight = 20011105,
            Fortune = 20011106,
            PartyTonight6th = 20011113,
            PartyTonight6th2 = 20011114,
            LawOfficer = 20011115,
            NinasPentacle = 20011118,
            Frog = 20011121,
            Turtle = 20011122,
            Buffalo = 20011123,
            JrTank = 20011124,
            SpiritViking = 20011129,
            PachinkoRobo = 20011130,
            RexsHyena = 20011136,
            LowRider2 = 20011138,
            NapoleanMount = 20011139,
            SoaringMount = 20011142,
            RedDraco = 20011143,
            Shinjo2 = 20011144,
            HotAirBalloon = 20011145,
            NadeshikoFlyHigh = 20011146,
            Pegasus = 20011147,
            Dragon = 20011148,
            MagicBroom = 20011149,
            Cloud = 20011150,
            KnightsChariot = 20011151,
            Nightmare2 = 20011152,
            Balrog2 = 20011153,
            InvisibleBalrog = 20011154,
            DragonLevel3 = 20011155,
            Owl2 = 20011156,
            Helicopter2 = 20011157,
            HighwayPatrolCar = 20011158,
            BalrogsHellfire = 20011162,
            HorntailsFlameBreath = 20011163,
            RexsCharge = 20011164,
            AnisJudgment = 20011165,
            DragonRidersEnergyBreath = 20011171,
            VonLeonsLionSlash = 20011172,
            ZakumsToweringInferno = 20011173,
            SpiritofRocksDoomStrike = 20011174,
            MuGongsAbsoluteDestruction = 20011175,
            PinkBeansZoneofIncrediblePain = 20011176,
            WhiteAngel = 20011179,
            DecentHaste = 20018000,
            DecentMysticDoor = 20018001,
            DecentSharpEyes = 20018002,
            DecentHyperBody = 20018003,
            DecentCombatOrders = 20018004,
            DecentAdvancedBlessing = 20018005,
            DecentSpeedInfusion = 20018006,
            PigsWeakness = 20019000,
            StumpsWeakness = 20019001,
            SlimesWeakness = 20019002,
        }

        public enum Mercedes : int
        {
            PotionMastery = 20020002,
            BlessingoftheFairy = 20020012,
            DeadlyCrits = 20020022,
            EmpresssBlessing = 20020073,
            ArchangelicBlessing = 20020086,
            DarkAngelicBlessing = 20020088,
            ArchangelicBlessing2 = 20020091,
            HiddenPotentialHero = 20020093,
            FreezingAxe = 20020097,
            IceSmash = 20020099,
            IceTempest = 20020100,
            IceChop = 20020103,
            IceCurse = 20020104,
            ElvenHealing = 20020109,
            Updraft = 20020111,
            ElvenGrace = 20020112,
            WhiteAngelicBlessing = 20020180,
            WilloftheAlliance = 20020190,
            CrystalThrow = 20021000,
            Infiltrate = 20021001,
            LegendarySpirit = 20021003,
            MonsterRiding = 20021004,
            HerosEcho = 20021005,
            Test = 20021006,
            Maker = 20021007,
            BambooRain = 20021009,
            Invincibility = 20021010,
            PowerExplosion = 20021011,
            Spaceship = 20021013,
            SpaceDash = 20021014,
            SpaceBeam = 20021015,
            YetiMount = 20021017,
            YetiMount2 = 20021018,
            WitchsBroomstick = 20021019,
            RageofPharaoh = 20021020,
            FollowtheLead = 20021024,
            ChargeToyTrojan = 20021025,
            Soaring = 20021026,
            Croco = 20021027,
            BlackScooter = 20021028,
            PinkScooter = 20021029,
            NimbusCloud = 20021030,
            Balrog = 20021031,
            RaceKart = 20021033,
            ZDTiger = 20021034,
            MistBalrog = 20021035,
            Shinjo = 20021042,
            OrangeMushroom = 20021044,
            Nightmare = 20021049,
            Yeti = 20021050,
            Ostrich = 20021051,
            PinkBearHotAirBalloon = 20021052,
            TransformedRobot = 20021053,
            Capture = 20021061,
            CalloftheHunter = 20021062,
            Motorcycle = 20021063,
            PowerSuit = 20021064,
            MechanicDash = 20021068,
            LeonardotheLion = 20021072,
            WitchsBroomstick2 = 20021081,
            BlueScooter = 20021084,
            Archangel = 20021085,
            DarkAngel = 20021087,
            SantaSled = 20021089,
            Archangel2 = 20021090,
            IceDoubleJump = 20021098,
            IceKnight = 20021105,
            Fortune = 20021106,
            ElvenBlessing = 20021110,
            Sylvidia = 20021160,
            Sylvidia2 = 20021161,
            WhiteAngel = 20021179,
            DecentHaste = 20028000,
            DecentMysticDoor = 20028001,
            DecentSharpEyes = 20028002,
            DecentHyperBody = 20028003,
            DecentCombatOrders = 20028004,
            DecentAdvancedBlessing = 20028005,
            DecentSpeedInfusion = 20028006,
        }

        public enum Aran1 : int
        {
            ComboAbility = 21000000,
            DoubleSwing = 21000002,
            ComboSmash = 21000004,
            GuardianArmor = 21000005,
            CombatStep = 21001001,
            PolearmBooster = 21001003,
        }

        public enum Aran2 : int
        {
            PolearmMastery = 21100000,
            TripleSwing = 21100001,
            FinalCharge = 21100002,
            ComboSmash = 21100004,
            ComboDrain = 21100005,
            ComboFenrir = 21100007,
            PhysicalTraining = 21100008,
            SelfRecovery = 21100009,
            FinalAttack = 21100010,
            BodyPressure = 21101003,
            SnowCharge = 21101006,
        }

        public enum Aran3 : int
        {
            ComboCritical = 21110000,
            FullSwing = 21110002,
            FinalToss = 21110003,
            ComboFenrir = 21110004,
            RollingSpin = 21110006,
            hiddenFullSwingDoubleSwing = 21110007,
            hiddenFullSwingTripleSwing = 21110008,
            CleavingBlows = 21110010,
            ComboJudgment = 21110011,
            Might = 21111001,
            SnowCharge = 21111005,
            ComboRecharge = 21111009,
            MahaBlessing = 21111012,
        }

        public enum Aran4 : int
        {
            HighMastery = 21120001,
            OverSwing = 21120002,
            HighDefense = 21120004,
            FinalBlow = 21120005,
            ComboTempest = 21120006,
            ComboBarrier = 21120007,
            hiddenOverSwingDoubleSwing = 21120009,
            hiddenOverSwingTripleSwing = 21120010,
            SuddenStrike = 21120011,
            AdvancedFinalAttack = 21120012,
            MapleWarrior = 21121000,
            FreezeStanding = 21121003,
            HerosWill = 21121008,
        }

        public enum Evan1 : int
        {
            DragonSoul = 22000000,
            ElementalWeakness = 22000002,
            MagicMissile = 22001001,
        }

        public enum Evan2 : int
        {
            FireCircle = 22101000,
            Teleport = 22101001,
        }

        public enum Evan3 : int
        {
            LightningBolt = 22111000,
            MagicGuard = 22111001,
        }

        public enum Evan4 : int
        {
            HighWisdom = 22120001,
            SpellMastery = 22120002,
            IceBreath = 22121000,
        }

        public enum Evan5 : int
        {
            MagicFlare = 22131000,
            MagicShield = 22131001,
            ElementalDecrease = 22131002,
        }

        public enum Evan6 : int
        {
            CriticalMagic = 22140000,
            DragonThrust = 22141001,
            MagicBooster = 22141002,
            Slow = 22141003,
            DragonBlink = 22141004,
        }

        public enum Evan7 : int
        {
            MagicAmplification = 22150000,
            DragonSpark = 22150004,
            FireBreath = 22151001,
            KillerWings = 22151002,
            MagicResistance = 22151003,
        }

        public enum Evan8 : int
        {
            DragonFury = 22160000,
            Earthquake = 22161001,
            PhantomImprint = 22161002,
            RecoveryAura = 22161003,
            OnyxShroud = 22161004,
            TeleportMastery = 22161005,
        }

        public enum Evan9 : int
        {
            MagicMastery = 22170001,
            MapleWarrior = 22171000,
            Illusion = 22171002,
            FlameWheel = 22171003,
            HerosWill = 22171004,
        }

        public enum Evan10 : int
        {
            BlessingoftheOnyx = 22181000,
            Blaze = 22181001,
            DarkFog = 22181002,
            SoulStone = 22181003,
            OnyxWill = 22181004,
        }

        public enum Mercedes1 : int
        {
            PotentialPower = 23000001,
            SharpAim = 23000003,
            NaturesBalance = 23000004,
            SwiftDualShot = 23001000,
            GlideBlast = 23001002,
        }

        public enum Mercedes2 : int
        {
            PartingShot = 23100004,
            DualBowgunsMastery = 23100005,
            FinalAttackDualBowguns = 23100006,
            PhysicalTraining = 23100008,
            PiercingStorm = 23101000,
            RisingRush = 23101001,
            DualBowgunsBoost = 23101002,
            SpiritSurge = 23101003,
        }

        public enum Mercedes3 : int
        {
            AerialBarrage = 23110006,
            StunningStrikes = 23111000,
            LeapTornado = 23111001,
            UnicornSpike = 23111002,
            GustDive = 23111003,
            IgnisRoar = 23111004,
            WaterShield = 23111005,
            ElementalKnights = 23111008,
            ElementalKnights2 = 23111009,
            ElementalKnights3 = 23111010,
        }

        public enum Mercedes4 : int
        {
            DualBowgunsExpert = 23120009,
            DefenseBreak = 23120010,
            RollingMoonsault = 23120011,
            AdvancedFinalAttack = 23120012,
            IshtarsRing = 23121000,
            SpikesRoyale = 23121002,
            LightningEdge = 23121003,
            AncientWarding = 23121004,
            MapleWarrior = 23121005,
            HerosWill = 23121008,
        }

        public enum Citizen : int
        {
            PotionMastery = 30000002,
            BlessingoftheFairy = 30000012,
            DeadlyCrits = 30000022,
            EmpresssBlessing = 30000073,
            ArchangelicBlessing = 30000086,
            DarkAngelicBlessing = 30000088,
            ArchangelicBlessing2 = 30000091,
            HiddenPotentialResistance = 30000093,
            FreezingAxe = 30000097,
            IceSmash = 30000099,
            IceTempest = 30000100,
            IceChop = 30000103,
            IceCurse = 30000104,
            WhiteAngelicBlessing = 30000180,
            WilloftheAlliance = 30000190,
            CrystalThrow = 30001000,
            Infiltrate = 30001001,
            LegendarySpirit = 30001003,
            MonsterRiding = 30001004,
            HerosEcho = 30001005,
            Test = 30001006,
            Maker = 30001007,
            BambooRain = 30001009,
            Invincibility = 30001010,
            PowerExplosion = 30001011,
            Spaceship = 30001013,
            SpaceDash = 30001014,
            SpaceBeam = 30001015,
            YetiMount = 30001017,
            YetiMount2 = 30001018,
            WitchsBroomstick = 30001019,
            RageofPharaoh = 30001020,
            FollowtheLead = 30001024,
            MagicalWoodenHorse = 30001025,
            Soaring = 30001026,
            Croking = 30001027,
            RetroScooter = 30001028,
            LovelyScooter = 30001029,
            NimbusCloud = 30001030,
            Balrog = 30001031,
            F1Machine = 30001033,
            Tiger = 30001034,
            MistBalrog = 30001035,
            Lion = 30001036,
            Unicorn = 30001037,
            LowRider = 30001038,
            RedTruck = 30001039,
            Gargoyle = 30001040,
            Shinjo = 30001042,
            OrangeMushroom = 30001044,
            Nightmare = 30001049,
            Yeti = 30001050,
            Ostrich = 30001051,
            PinkBearHotAirBalloon = 30001052,
            Transformer = 30001053,
            Chicken = 30001054,
            Capture = 30001061,
            CalloftheHunter = 30001062,
            KurenaiRunAway = 30001063,
            PowerSuit = 30001064,
            OS4Shuttle = 30001065,
            VisitorMeleeAttack = 30001066,
            VisitorRangeAttack = 30001067,
            MechanicDash = 30001068,
            Owl = 30001069,
            Mothership = 30001070,
            OS3AMachine = 30001071,
            LeonardotheLion = 30001072,
            WitchsBroomstick2 = 30001081,
            BlueScooter = 30001084,
            Archangel = 30001085,
            DarkAngel = 30001087,
            SantaSled = 30001089,
            Archangel2 = 30001090,
            GiantRabbit = 30001096,
            IceDoubleJump = 30001098,
            SmallRabbit = 30001101,
            RabbitRickshaw = 30001102,
            IceKnight = 30001105,
            Fortune = 30001106,
            PartyTonight6th = 30001113,
            PartyTonight6th2 = 30001114,
            LawOfficer = 30001115,
            NinasPentacle = 30001118,
            Frog = 30001121,
            Turtle = 30001122,
            Buffalo = 30001123,
            JrTank = 30001124,
            SpiritViking = 30001129,
            PachinkoRobo = 30001130,
            RexsHyena = 30001136,
            LowRider2 = 30001138,
            NapoleanMount = 30001139,
            SoaringMount = 30001142,
            RedDraco = 30001143,
            Shinjo2 = 30001144,
            HotAirBalloon = 30001145,
            NadeshikoFlyHigh = 30001146,
            Pegasus = 30001147,
            Dragon = 30001148,
            MagicBroom = 30001149,
            Cloud = 30001150,
            KnightsChariot = 30001151,
            Nightmare2 = 30001152,
            Balrog2 = 30001153,
            InvisibleBalrog = 30001154,
            DragonLevel3 = 30001155,
            Owl2 = 30001156,
            Helicopter = 30001157,
            HighwayPatrolCar = 30001158,
            BalrogsHellfire = 30001162,
            HorntailsFlameBreath = 30001163,
            RexsCharge = 30001164,
            AnisJudgment = 30001165,
            DragonRidersEnergyBreath = 30001171,
            VonLeonsLionSlash = 30001172,
            ZakumsToweringInferno = 30001173,
            SpiritofRocksDoomStrike = 30001174,
            MuGongsAbsoluteDestruction = 30001175,
            PinkBeansZoneofIncrediblePain = 30001176,
            WhiteAngel = 30001179,
            DecentHaste = 30008000,
            DecentMysticDoor = 30008001,
            DecentSharpEyes = 30008002,
            DecentHyperBody = 30008003,
            DecentCombatOrders = 30008004,
            DecentAdvancedBlessing = 30008005,
            DecentSpeedInfusion = 30008006,
        }

        public enum DemonSlayer : int
        {
            PotionMastery = 30010002,
            BlessingoftheFairy = 30010012,
            DeadlyCrits = 30010022,
            EmpresssBlessing = 30010073,
            ArchangelicBlessing = 30010086,
            DarkAngelicBlessing = 30010088,
            ArchangelicBlessing2 = 30010091,
            HiddenPotentialResistance = 30010093,
            FreezingAxe = 30010097,
            IceSmash = 30010099,
            IceTempest = 30010100,
            IceChop = 30010103,
            IceCurse = 30010104,
            DarkWinds = 30010110,
            CurseofFury = 30010111,
            FuryUnleashed = 30010112,
            WhiteAngelicBlessing = 30010180,
            DemonicBlood = 30010185,
            WilloftheAlliance = 30010190,
            CrystalThrow = 30011000,
            Infiltrate = 30011001,
            LegendarySpirit = 30011003,
            MonsterRiding = 30011004,
            HerosEcho = 30011005,
            Test = 30011006,
            Maker = 30011007,
            BambooRain = 30011009,
            Invincibility = 30011010,
            PowerExplosion = 30011011,
            Spaceship = 30011013,
            SpaceDash = 30011014,
            SpaceBeam = 30011015,
            YetiMount = 30011017,
            YetiMount2 = 30011018,
            WitchsBroomstick = 30011019,
            RageofPharaoh = 30011020,
            FollowtheLead = 30011024,
            ChargeToyTrojan = 30011025,
            Soaring = 30011026,
            Croco = 30011027,
            BlackScooter = 30011028,
            PinkScooter = 30011029,
            NimbusCloud = 30011030,
            Balrog = 30011031,
            RaceKart = 30011033,
            ZDTiger = 30011034,
            MistBalrog = 30011035,
            Shinjo = 30011042,
            OrangeMushroom = 30011044,
            Nightmare = 30011049,
            Yeti = 30011050,
            Ostrich = 30011051,
            PinkBearHotAirBalloon = 30011052,
            TransformedRobot = 30011053,
            Capture = 30011061,
            CalloftheHunter = 30011062,
            Motorcycle = 30011063,
            PowerSuit = 30011064,
            MechanicDash = 30011068,
            LeonardotheLion = 30011072,
            WitchsBroomstick2 = 30011081,
            BlueScooter = 30011084,
            Archangel = 30011085,
            DarkAngel = 30011087,
            SantaSled = 30011089,
            Archangel2 = 30011090,
            IceDoubleJump = 30011098,
            IceKnight = 30011105,
            Fortune = 30011106,
            DemonWings = 30011109,
            DemonWings2 = 30011159,
            WhiteAngel = 30011179,
            DecentHaste = 30018000,
            DecentMysticDoor = 30018001,
            DecentSharpEyes = 30018002,
            DecentHyperBody = 30018003,
            DecentCombatOrders = 30018004,
            DecentAdvancedBlessing = 30018005,
            DecentSpeedInfusion = 30018006,
        }

        public enum DemonSlayer1 : int
        {
            ShadowSwiftness = 31000002,
            HPBoost = 31000003,
            DemonLash = 31000004,
            GuardianArmor = 31000005,
            GrimScythe = 31001000,
            BattlePact = 31001001,
        }

        public enum DemonSlayer2 : int
        {
            WeaponMastery = 31100004,
            PhysicalTraining = 31100005,
            Outrage = 31100006,
            BarbedLash = 31100007,
            SoulEater = 31101000,
            DarkThrust = 31101001,
            ChaosLock = 31101002,
            Vengeance = 31101003,
        }

        public enum DemonSlayer3 : int
        {
            InsulttoInjury = 31110006,
            FocusedFury = 31110007,
            PossessedAegis = 31110008,
            MaxFury = 31110009,
            DemonLashArch = 31110010,
            Judgment = 31111000,
            VortexofDoom = 31111001,
            RavenStorm = 31111003,
            BlackHeartedStrength = 31111004,
            CarrionBreath = 31111005,
        }

        public enum DemonSlayer4 : int
        {
            BarricadeMastery = 31120008,
            ObsidianSkin = 31120009,
            DemonThrash = 31120011,
            InfernalConcussion = 31121000,
            DemonImpact = 31121001,
            LeechAura = 31121002,
            DemonCry = 31121003,
            MapleWarrior = 31121004,
            DarkMetamorphosis = 31121005,
            BindingDarkness = 31121006,
            BoundlessRage = 31121007,
        }

        public enum BattleMage1 : int
        {
            ElementalWeakness = 32000012,
            TripleBlow = 32001000,
            TheFinisher = 32001001,
            Teleport = 32001002,
            DarkAura = 32001003,
            TheFinisher2 = 32001008,
            TheFinisher3 = 32001009,
            TheFinisher4 = 32001010,
            TheFinisher5 = 32001011,
        }

        public enum BattleMage2 : int
        {
            StaffMastery = 32100006,
            HighWisdom = 32100007,
            QuadBlow = 32101000,
            HyperDarkChain = 32101001,
            BlueAura = 32101002,
            YellowAura = 32101003,
            BloodDrain = 32101004,
            StaffBoost = 32101005,
        }

        public enum BattleMage3 : int
        {
            AdvancedBlueAura = 32110000,
            BattleMastery = 32110001,
            QuintupleBlow = 32111002,
            DarkShock = 32111003,
            Conversion = 32111004,
            BodyBoost = 32111005,
            SummonReaperBuff = 32111006,
            TeleportMastery = 32111010,
            AdvancedDarkChain = 32111011,
            BlueAura = 32111012,
            Stance = 32111014,
        }

        public enum BattleMage4 : int
        {
            AdvancedDarkAura = 32120000,
            AdvancedYellowAura = 32120001,
            Energize = 32120009,
            FinishingBlow = 32121002,
            TwisterSpin = 32121003,
            DarkGenesis = 32121004,
            PowerStance = 32121005,
            PartyShield = 32121006,
            MapleWarrior = 32121007,
            HerosWill = 32121008,
        }

        public enum WildHunter1 : int
        {
            NaturesBalance = 33000004,
            TripleShot = 33001000,
            JaguarRider = 33001001,
            JagJump = 33001002,
            CrossbowBooster = 33001003,
        }

        public enum WildHunter2 : int
        {
            CrossbowMastery = 33100000,
            FinalAttack = 33100009,
            PhysicalTraining = 33100010,
            Ricochet = 33101001,
            JaguarRawr = 33101002,
            SoulArrowCrossbow = 33101003,
            ItsRainingMines = 33101004,
            Jaguaroshi2 = 33101005,
            Jaguaroshi3 = 33101006,
            Jaguaroshi4 = 33101007,
            ItsRainingMineshiddenselfdestruct = 33101008,
        }

        public enum WildHunter3 : int
        {
            JaguarBoost = 33110000,
            EnduringFire = 33111001,
            DashnSlash = 33111002,
            WildTrap = 33111003,
            Blind = 33111004,
            SilverHawk = 33111005,
            Swipe = 33111006,
            FelineBerserk = 33111007,
        }

        public enum WildHunter4 : int
        {
            CrossbowExpert = 33120000,
            WildInstinct = 33120010,
            AdvancedFinalAttack = 33120011,
            ExplodingArrows = 33121001,
            SonicRoar = 33121002,
            SharpEyes = 33121004,
            StinkBombShot = 33121005,
            FelineBerserk = 33121006,
            MapleWarrior = 33121007,
            HerosWill = 33121008,
            WildArrowBlast = 33121009,
        }

        public enum Mechanic1 : int
        {
            FortunesFavor = 35000005,
            FlameLauncher = 35001001,
            MechPrototype = 35001002,
            ME07Drillhands = 35001003,
            GatlingGun = 35001004,
        }

        public enum Mechanic2 : int
        {
            MechanicMastery = 35100000,
            HeavyWeaponMastery = 35100008,
            PhysicalTraining = 35100011,
            AtomicHammer = 35101003,
            RocketBooster = 35101004,
            OpenPortalGX9 = 35101005,
            MechanicRage = 35101006,
            PerfectArmor = 35101007,
            EnhancedFlameLauncher = 35101009,
            EnhancedGatlingGun = 35101010,
        }

        public enum Mechanic3 : int
        {
            MetalFistMastery = 35110014,
            Satellite = 35111001,
            RocknShock = 35111002,
            MechSiegeMode = 35111004,
            AccelerationBotEX7 = 35111005,
            Satellite2 = 35111009,
            Satellite3 = 35111010,
            HealingRobotHLX = 35111011,
            RolloftheDice = 35111013,
            PunchLauncher = 35111015,
        }

        public enum Mechanic4 : int
        {
            ExtremeMech = 35120000,
            RobotMastery = 35120001,
            GiantRobotSG88 = 35121003,
            MechMissileTank = 35121005,
            SatelliteSafety = 35121006,
            MapleWarrior = 35121007,
            HerosWill = 35121008,
            BotsnTots = 35121009,
            AmplifierRobotAF11 = 35121010,
            LaserBlast = 35121012,
            MechSiegeMode = 35121013,
        }
    }

    [Flags]
    public enum CharacterStatus : int
    {
        Curse = 0x01,
        Weakness = 0x02,
        Darkness = 0x04,
        Seal = 0x08,
        Poison = 0x10,
        Stun = 0x20,
        Slow = 0x40,
        Seduce = 0x80,
        Zombify = 0x100,
        CrazySkull = 0x200
    }

    [Flags]
    public enum MobStatus : int
    {
        Null,

        WeaponAttackIcon = 0x01,
        WeaponDefenceIcon = 0x02,
        MagicAttackIcon = 0x04,
        MagicDefenceIcon = 0x08,
        AccuracyIcon = 0x10,
        AvoidabilityIcon = 0x20,
        SpeedIcon = 0x40,

        Stunned = 0x80,
        Frozen = 0x100,
        Poisoned = 0x200,
        Sealed = 0x400,

        Unknown1 = 0x800,

        WeaponAttackUp = 0x1000,
        WeaponDefenseUp = 0x2000,
        MagicAttackUp = 0x4000,
        MagicDefenseUp = 0x8000,

        Doom = 0x10000,
        ShadowWeb = 0x20000,

        WeaponImmunity = 0x40000,
        MagicImmunity = 0x80000,

        Unknown2 = 0x100000,
        Unknown3 = 0x200000,
        NinjaAmbush = 0x400000,
        Unknown4 = 0x800000,
        VenomousWeapon = 0x1000000,
        Unknown5 = 0x2000000,
        Unknown6 = 0x4000000,
        Empty = 0x8000000,
        Hypnotized = 0x10000000,
        WeaponDamageReflect = 0x20000000,
        MagicDamageReflect = 0x40000000
    }

    public enum CharacterDisease : long
    {
        Null,
        Slow = 0x1,
        Seduce = 0x80,
        Fishable = 0x100,
        Confuse = 0x80000,
        Stun = 0x2000000000000,
        Poison = 0x4000000000000,
        Sealed = 0x8000000000000,
        Darkness = 0x10000000000000,
        Weaken = 0x4000000000000000
    }

    public enum MobSkillName : int
    {
        WeaponAttackUp = 100,
        MagicAttackUp = 101,
        WeaponDefenseUp = 102,
        MagicDefenseUp = 103,

        WeaponAttackUpAreaOfEffect = 110,
        MagicAttackUpAreaOfEffect = 111,
        WeaponDefenseUpAreaOfEffect = 112,
        MagicDefenseUpAreaOfEffect = 113,
        HealAreaOfEffect = 114,
        SpeedUpAreaOfEffect = 115,

        Seal = 120,
        Darkness = 121,
        Weakness = 122,
        Stun = 123,
        Curse = 124,
        Poison = 125,
        Slow = 126,
        Dispel = 127,
        Seduce = 128,
        SendToTown = 129,
        PoisonMist = 131,
        Confuse = 132,
        Zombify = 133,

        WeaponImmunity = 140,
        MagicImmunity = 141,
        ArmorSkill = 142,

        WeaponDamageReflect = 143,
        MagicDamageReflect = 144,
        AnyDamageReflect = 145,

        WeaponAttackUpMonsterCarnival = 150,
        MagicAttackUpMonsterCarnival = 151,
        WeaponDefenseUpMonsterCarnival = 152,
        MagicDefenseUpMonsterCarnival = 153,
        AccuracyUpMonsterCarnival = 154,
        AvoidabilityUpMonsterCarnival = 155,
        SpeedUpMonsterCarnival = 156,
        SealMonsterCarnival = 157,

        Summon = 200
    }

    public enum ScrollResult : byte
    {
        Success = 1,
        Fail = 2,
        Curse = 3
    }

    public enum BuddyAddResults
    {
        BuddyListFull,
        AlreadyOnList,
        Success
    }

    public enum Potential : byte
    {
        Regular = 0,
        HiddenPotential1,
        HiddenPotential2,
        HiddenPotential3,
        Rare = 5,
        Epic,
        Unique,
    }

    public enum ExtendedSPType
    {
        Regular,
        Evan,
        Resistance,
        Mercedes,
        Phantom
    }

    public enum SpecialJob : byte
    {
        Regular,
        DualBlade,
        Cannoneer
    }

    public enum ExpirationTime : long
    {
        DefaultTime = 150842304000000000L,
        ZeroTime = 94354848000000000L,
        Permanent = 150841440000000000L
    }
}