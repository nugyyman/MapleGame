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

        Pirate = 500,
        Brawler = 510,
        Marauder,
        Buccaneer,
        Gunslinger = 520,
        Outlaw,
        Corsair,

        GM = 900,
        SuperGM = 910,

        Noblesse = 1000,

        DawnWarrior1 = 1100,
        DawnWarrior2 = 1110,
        DawnWarrior3 = 1111,

        BlazeWizard1 = 1200,
        BlazeWizard2 = 1210,
        BlazeWizard3 = 1211,

        WindArcher1 = 1300,
        WindArcher2 = 1310,
        WindArcher3 = 1311,

        NightWalker1 = 1400,
        NightWalker2 = 1410,
        NightWalker3 = 1411,

        ThunderBreaker1 = 1500,
        ThunderBreaker2 = 1510,
        ThunderBreaker3 = 1511,

        Legend = 2000,

        Aran1 = 2100,
        Aran2 = 2110,
        Aran3 = 2111,
        Aran4 = 2112
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
        YesNo,
        RequestText,
        RequestNumber,
        Choice,
        RequestStyle = 7,
        AcceptDecline = 0x0C
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
        Start = 1,
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
        public static readonly int[] CharacterLevel = { 1, 15, 34, 57, 92, 135, 372, 560, 840, 1242, 1144, 1573, 2144, 2800, 3640, 4700, 5893, 7360, 9144, 11120, 13477, 16268, 19320, 22880, 27008, 31477, 36600, 42444, 48720, 55813, 63800, 86784, 98208, 110932, 124432, 139372, 155865, 173280, 192400, 213345, 235372, 259392, 285532, 312928, 342624, 374760, 408336, 445544, 483532, 524160, 567772, 598886, 631704, 666321, 702836, 741351, 781976, 824828, 870028, 917625, 967995, 1021041, 1076994, 1136013, 1198266, 1263930, 1333194, 1406252, 1483314, 1564600, 1650340, 1740778, 1836173, 1936794, 2042930, 2154882, 2272970, 2397528, 2528912, 2667496, 2813674, 2967863, 3130502, 3302053, 3483005, 3673873, 3875201, 4087562, 4311559, 4547832, 4797053, 5059931, 5337215, 5629694, 5938202, 6263614, 6606860, 6968915, 7350811, 7753635, 8178534, 8626718, 9099462, 9598112, 10124088, 10678888, 11264090, 11881362, 12532461, 13219239, 13943653, 14707765, 15513750, 16363902, 17260644, 18206527, 19204245, 20256637, 21366700, 22537594, 23772654, 25075395, 26449526, 27898960, 29427822, 31040466, 32741483, 34535716, 36428273, 38424542, 40530206, 42751262, 45094030, 47565183, 50171755, 52921167, 55821246, 58880250, 62106888, 65510344, 69100311, 72887008, 76881216, 81094306, 85594273, 90225770, 95170142, 100385466, 105886589, 111689174, 117809740, 124265714, 131075474, 138258410, 145834970, 153826726, 162256430, 171148082, 180526997, 190419876, 200854885, 211861732, 223471711, 223471711, 248635353, 262260570, 276632449, 291791906, 307782102, 324648562, 342439302, 361204976, 380999008, 401877754, 423900654, 447130410, 471633156, 497478653, 524740482, 553496261, 583827855, 615821622, 649568646, 685165008, 722712050, 762316670, 804091623, 848155844, 894634784, 943660770, 995373379, 1049919840, 1107455447, 1168144006, 1232158297, 1299680571, 1370903066, 1446028554, 1525246918, 1608855764, 1697021059 };
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
            RegularAttack = 0
        }

        public enum Beginner : int
        {
            BlessingOfTheFairy = 12,
            EchoOfHero = 1005,
            FollowTheLead = 8,
            MonsterRider = 1004,
            NimbleFeet = 1002,
            Recovery = 1001
        }

        public enum Swordsman : int
        {
            ImprovedMaxHpIncrease = 1000001,
            IronBody = 1001003
        }

        public enum Fighter : int
        {
            AxeBooster = 1101005,
            AxeMastery = 1100001,
            PowerGuard = 1101007,
            Rage = 1101006,
            SwordBooster = 1101004,
            SwordMastery = 1100000
        }

        public enum Crusader : int
        {
            ArmorCrash = 1111007,
            AxeComa = 1111006,
            AxePanic = 1111004,
            ComboAttack = 1111002,
            Shout = 1111008,
            SwordComa = 1111005,
            SwordPanic = 1111003
        }

        public enum Hero : int
        {
            Achilles = 1120004,
            AdvancedComboAttack = 1120003,
            Enrage = 1121010,
            Guardian = 1120005,
            HerosWill = 1121011,
            MapleWarrior = 1121000,
            MonsterMagnet = 1121001,
            PowerStance = 1121002
        }

        public enum Page : int
        {
            BwBooster = 1201005,
            BwMastery = 1200001,
            PowerGuard = 1201007,
            SwordBooster = 1201004,
            SwordMastery = 1200000,
            Threaten = 1201006
        }

        public enum WhiteKnight : int
        {
            BwFireCharge = 1211004,
            BwIceCharge = 1211006,
            BwLitCharge = 1211008,
            ChargeBlow = 1211002,
            MagicCrash = 1211009,
            SwordFireCharge = 1211003,
            SwordIceCharge = 1211005,
            SwordLitCharge = 1211007
        }

        public enum Paladin : int
        {
            Achilles = 1220005,
            AdvancedCharge = 1220010,
            BwHolyCharge = 1221004,
            Guardian = 1220006,
            HeavensHammer = 1221011,
            HerosWill = 1221012,
            MapleWarrior = 1221000,
            MonsterMagnet = 1221001,
            PowerStance = 1221002,
            SwordHolyCharge = 1221003
        }

        public enum Spearman : int
        {
            HyperBody = 1301007,
            IronWill = 1301006,
            PolearmBooster = 1301005,
            PolearmMastery = 1300001,
            SpearBooster = 1301004,
            SpearMastery = 1300000
        }

        public enum DragonKnight : int
        {
            DragonBlood = 1311008,
            DragonRoar = 1311006,
            ElementalResistance = 1310000,
            PowerCrash = 1311007,
            Sacrifice = 1311005
        }

        public enum DarkKnight : int
        {
            Achilles = 1320005,
            AuraOfBeholder = 1320008,
            Beholder = 1321007,
            Berserk = 1320006,
            HerosWill = 1321010,
            HexOfBeholder = 1320009,
            MapleWarrior = 1321000,
            MonsterMagnet = 1321001,
            PowerStance = 1321002
        }

        public enum Magician : int
        {
            ImprovedMaxMpIncrease = 2000001,
            MagicArmor = 2001003,
            MagicGuard = 2001002
        }

        public enum FirePoisonWizard : int
        {
            Meditation = 2101001,
            MpEater = 2100000,
            PoisonBreath = 2101005,
            Slow = 2101003
        }

        public enum FirePoisonMage : int
        {
            ElementAmplification = 2110001,
            ElementComposition = 2111006,
            PartialResistance = 2110000,
            PoisonMist = 2111003,
            Seal = 2111004,
            SpellBooster = 2111005
        }

        public enum FirePoisonArchMage : int
        {
            BigBang = 2121001,
            Elquines = 2121005,
            FireDemon = 2121003,
            HerosWill = 2121008,
            Infinity = 2121004,
            ManaReflection = 2121002,
            MapleWarrior = 2121000,
            Paralyze = 2121006
        }

        public enum IceLightningWizard : int
        {
            ColdBeam = 2201004,
            Meditation = 2201001,
            MpEater = 2200000,
            Slow = 2201003
        }

        public enum IceLightningMage : int
        {
            ElementAmplification = 2210001,
            ElementComposition = 2211006,
            IceStrike = 2211002,
            PartialResistance = 2210000,
            Seal = 2211004,
            SpellBooster = 2211005
        }

        public enum IceLightningArchMage : int
        {
            BigBang = 2221001,
            Blizzard = 2221007,
            HerosWill = 2221008,
            IceDemon = 2221003,
            Ifrit = 2221005,
            Infinity = 2221004,
            ManaReflection = 2221002,
            MapleWarrior = 2221000
        }

        public enum Cleric : int
        {
            Bless = 2301004,
            Heal = 2301002,
            Invincible = 2301003,
            MpEater = 2300000
        }

        public enum Priest : int
        {
            Dispel = 2311001,
            Doom = 2311005,
            ElementalResistance = 2310000,
            HolySymbol = 2311003,
            MysticDoor = 2311002,
            SummonDragon = 2311006
        }

        public enum Bishop : int
        {
            Bahamut = 2321003,
            BigBang = 2321001,
            HerosWill = 2321009,
            HolyShield = 2321005,
            Infinity = 2321004,
            ManaReflection = 2321002,
            MapleWarrior = 2321000,
            Resurrection = 2321006
        }

        public enum Archer : int
        {
            CriticalShot = 3000001,
            Focus = 3001003
        }

        public enum Hunter : int
        {
            ArrowBomb = 3101005,
            BowBooster = 3101002,
            BowMastery = 3100000,
            SoulArrow = 3101004
        }

        public enum Ranger : int
        {
            MortalBlow = 3110001,
            Puppet = 3111002,
            SilverHawk = 3111005
        }

        public enum Bowmaster : int
        {
            Concentrate = 3121008,
            Hamstring = 3121007,
            HerosWill = 3121009,
            Hurricane = 3121004,
            MapleWarrior = 3121000,
            Phoenix = 3121006,
            SharpEyes = 3121002
        }

        public enum Crossbowman : int
        {
            CrossbowBooster = 3201002,
            CrossbowMastery = 3200000,
            SoulArrow = 3201004
        }

        public enum Sniper : int
        {
            Blizzard = 3211003,
            GoldenEagle = 3211005,
            MortalBlow = 3210001,
            Puppet = 3211002
        }

        public enum Marksman : int
        {
            Blind = 3221006,
            Frostprey = 3221005,
            HerosWill = 3221008,
            MapleWarrior = 3221000,
            PiercingArrow = 3221001,
            SharpEyes = 3221002,
            Snipe = 3221007
        }

        public enum Rogue : int
        {
            DarkSight = 4001003,
            Disorder = 4001002,
            DoubleStab = 4001334,
            LuckySeven = 4001344
        }

        public enum Assassin : int
        {

            ClawBooster = 4101003,
            ClawMastery = 4100000,
            CriticalThrow = 4100001,
            Drain = 4101005,
            Haste = 4101004
        }

        public enum Hermit : int
        {
            Alchemist = 4110000,
            Avenger = 4111005,
            MesoUp = 4111001,
            ShadowMeso = 4111004,
            ShadowPartner = 4111002,
            ShadowWeb = 4111003
        }

        public enum NightLord : int
        {
            HerosWill = 4121009,
            MapleWarrior = 4121000,
            NinjaAmbush = 4121004,
            NinjaStorm = 4121008,
            ShadowShifter = 4120002,
            ShadowStars = 4121006,
            Taunt = 4121003,
            TripleThrow = 4121007,
            VenomousStar = 4120005
        }

        public enum Bandit : int
        {
            DaggerBooster = 4201002,
            DaggerMastery = 4200000,
            Haste = 4201003,
            SavageBlow = 4201005,
            Steal = 4201004
        }

        public enum ChiefBandit : int
        {
            Assaulter = 4211002,
            BandOfThieves = 4211004,
            Chakra = 4211001,
            MesoExplosion = 4211006,
            MesoGuard = 4211005,
            Pickpocket = 4211003
        }

        public enum Shadower : int
        {
            Assassinate = 4221001,
            BoomerangStep = 4221007,
            HerosWill = 4221008,
            MapleWarrior = 4221000,
            NinjaAmbush = 4221004,
            ShadowShifter = 4220002,
            Smokescreen = 4221006,
            Taunt = 4221003,
            VenomousStab = 4220005
        }

        public enum Pirate : int
        {
            Dash = 5001005
        }

        public enum Brawler : int
        {
            BackspinBlow = 5101002,
            CorkscrewBlow = 5101004,
            DoubleUppercut = 5101003,
            ImproveMaxHp = 5100000,
            KnucklerBooster = 5101006,
            KnucklerMastery = 5100001,
            MpRecovery = 5101005,
            OakBarrel = 5101007
        }

        public enum Marauder : int
        {
            EnergyCharge = 5110001,
            EnergyDrain = 5111004,
            StunMastery = 5110000,
            Transformation = 5111005
        }

        public enum Buccaneer : int
        {
            Demolition = 5121004,
            MapleWarrior = 5121000,
            PiratesRage = 5121008,
            Snatch = 5121005,
            SpeedInfusion = 5121009,
            SuperTransformation = 5121003,
            TimeLeap = 5121010
        }

        public enum Gunslinger : int
        {
            BlankShot = 5201004,
            Grenade = 5201002,
            GunBooster = 5201003,
            GunMastery = 5200000
        }

        public enum Outlaw : int
        {
            Flamethrower = 5211004,
            Gaviota = 5211002,
            HomingBeacon = 5211006,
            IceSplitter = 5211005,
            Octopus = 5211001
        }

        public enum Corsair : int
        {
            AerialStrike = 5221003,
            Battleship = 5221006,
            Bullseye = 5220011,
            ElementalBoost = 5220001,
            Hypnotize = 5221009,
            MapleWarrior = 5221000,
            RapidFire = 5221004,
            SpeedInfusion = 5221010,
            WrathOfTheOctopi = 5220002
        }

        public enum GM : int
        {
            Haste = 9001000,
            SuperDragonRoar = 9001001,
            Teleport = 9001007
        }

        public enum SuperGM : int
        {
            Bless = 9101003,
            Haste = 9101001,
            HealPlusDispel = 9101000,
            Hide = 9101004,
            HolySymbol = 9101002,
            HyperBody = 9101008,
            Resurrection = 9101005,
            SuperDragonRoar = 9101006,
            Teleport = 9101007
        }

        public enum Noblesse : int
        {
            BlessingOfTheFairy = 10000012,
            EchoOfHero = 10001005,
            Maker = 10001007,
            MonsterRider = 10001004,
            NimbleFeet = 10001002,
            Recovery = 10001001
        }

        public enum DawnWarrior : int
        {
            AdvancedCombo = 11110005,
            Coma = 11111003,
            ComboAttack = 11111001,
            FinalAttack = 11101002,
            IronBody = 11001001,
            MaxHpEnhancement = 11000000,
            Panic = 11111002,
            Rage = 11101003,
            Soul = 11001004,
            SoulBlade = 11101004,
            SoulCharge = 11111007,
            SwordBooster = 11101001,
            SwordMastery = 11100000
        }

        public enum BlazeWizard : int
        {
            ElementalReset = 12101005,
            ElementAmplification = 12110001,
            FireStrike = 12111006,
            Flame = 12001004,
            FlameGear = 12111005,
            Ifrit = 12111004,
            IncreasingMaxMp = 12000000,
            MagicArmor = 12001002,
            MagicGuard = 12001001,
            Meditation = 12101000,
            Seal = 12111002,
            Slow = 12101001,
            SpellBooster = 12101004
        }

        public enum WindArcher : int
        {
            EagleEye = 13111005,
            BowBooster = 13101001,
            BowMastery = 13100000,
            CriticalShot = 13000000,
            FinalAttack = 13101002,
            Focus = 13001002,
            Hurricane = 13111002,
            Puppet = 13111004,
            SoulArrow = 13101003,
            Storm = 13001004,
            WindPiercing = 13111006,
            WindShot = 13111007,
            WindWalk = 13101006
        }

        public enum NightWalker : int
        {
            Alchemist = 14110003,
            Disorder = 14001002,
            DarkSight = 14001003,
            Darkness = 14001005,
            ClawBooster = 14101002,
            ClawMastery = 14100000,
            CriticalThrow = 14100001,
            Haste = 14101003,
            PoisonBomb = 14111006,
            ShadowPartner = 14111000,
            ShadowWeb = 14111001,
            SuddenAttack = 14100005,
            Vampire = 14101006,
            Venom = 14110004
        }

        public enum ThunderBreaker : int
        {
            CorkscrewBlow = 15101003,
            Dash = 15001003,
            EnergyCharge = 15100004,
            EnergyDrain = 15111001,
            ImproveMaxHp = 15100000,
            KnucklerBooster = 15101002,
            KnucklerMastery = 15100001,
            Lightning = 15001004,
            LightningCharge = 15101006,
            Spark = 15111006,
            SpeedInfusion = 15111005,
            Transformation = 15111002
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
}