using System;
using Loki.Data;
using Loki.IO;
using Loki.Maple.Characters;
using Loki.Maple.Data;
using Loki.Maple.Maps;
using Loki.Net;
using Loki.Threading;
using System.Collections.Generic;

namespace Loki.Maple
{
    public class Item : Drop
    {
        public static ItemType GetType(int mapleId)
        {
            int firstDigit = mapleId;

            while (firstDigit >= 10)
            {
                firstDigit /= 10;
            }

            return (ItemType)firstDigit;
        }

        public CharacterItems Parent { get; set; }

        public int ID { get; private set; }
        public int MapleID { get; private set; }
        private short maxPerStack;
        private short quantity;
        public sbyte Slot { get; set; }
        public string Creator { get; private set; }
        public bool IsStored { get; private set; }

        public bool IsCash { get; private set; }
        public bool OnlyOne { get; private set; }
        public bool PreventsSlipping { get; set; }
        public bool PreventsColdness { get; set; }
        public bool IsTradeBlocked { get; private set; }
        public bool IsScisored { get; private set; }
        public bool IsQuestItem { get; private set; }
        public int SalePrice { get; private set; }

        public byte UpgradesAvailable { get; set; }
        public byte UpgradesApplied { get; set; }
        public byte MaxUpgradesAvailable { get; set; }
        public short Strength { get; set; }
        public short Dexterity { get; set; }
        public short Intelligence { get; set; }
        public short Luck { get; set; }
        public short HP { get; set; }
        public short MP { get; set; }
        public short WeaponAttack { get; set; }
        public short MagicAttack { get; set; }
        public short WeaponDefense { get; set; }
        public short MagicDefense { get; set; }
        public short Accuracy { get; set; }
        public short Avoidability { get; set; }
        public short Agility { get; set; }
        public short Speed { get; set; }
        public short Jump { get; set; }
        public short ViciousHammerApplied { get; set; }
        public Potential Potential { get; set; }
        public byte Stars { get; set; }
        public short Potential1 { get; set; }
        public short Potential2 { get; set; }
        public short Potential3 { get; set; }
        public byte PotentialLines { get; set; }

        public sbyte AttackSpeed { get; private set; }
        public short RecoveryRate { get; private set; }
        public short KnockBackChance { get; private set; }

        public short RequiredLevel { get; private set; }
        public short RequiredStrength { get; private set; }
        public short RequiredDexterity { get; private set; }
        public short RequiredIntelligence { get; private set; }
        public short RequiredLuck { get; private set; }
        public short RequiredFame { get; private set; }
        public Job RequiredJob { get; private set; }

        // Scrolls data
        public int Success { get; private set; }
        public int BreakItem { get; private set; }
        public string Flag { get; private set; }
        public short IStrength { get; private set; }
        public short IDexterity { get; private set; }
        public short IIntelligence { get; private set; }
        public short ILuck { get; private set; }
        public short IHP { get; private set; }
        public short IMP { get; private set; }
        public short IWeaponAttack { get; private set; }
        public short IMagicAttack { get; private set; }
        public short IWeaponDefense { get; private set; }
        public short IMagicDefense { get; private set; }
        public short IAccuracy { get; private set; }
        public short IAvoidability { get; private set; }
        public short IJump { get; private set; }
        public short ISpeed { get; private set; }

        // SkillBooks and MasteryBooks data
        public List<int> SkillId = new List<int>();
        public short RequestSkillLevel { get; private set; }
        public short MasterLevel { get; private set; }
        public short Chance { get; private set; }

        // Item cosumes data
        public int CItemId { get; private set; }
        public string CFlags { get; private set; }
        public string CCureAilments { get; private set; }
        public short CEffect { get; private set; }
        public short CHP { get; private set; }
        public short CMP { get; private set; }
        public short CHPPercentage { get; private set; }
        public short CMPPercentage { get; private set; }
        public int CMoveTo { get; private set; }
        public short CProb { get; private set; }
        public int CBuffTime { get; private set; }
        public short CWeaponAttack { get; private set; }
        public short CMagicAttack { get; private set; }
        public short CWeaponDefense { get; private set; }
        public short CMagicDefense { get; private set; }
        public short CAccuracy { get; private set; }
        public short CAvoid { get; private set; }
        public short CSpeed { get; private set; }
        public short CJump { get; private set; }
        public short CMorph { get; private set; }

        // Cash items data
        public int SerialNumber { get; set; }
        public int UniqueID { get; set; }

        public Character Character
        {
            get
            {
                try
                {
                    return this.Parent.Parent;
                }
                catch (NullReferenceException)
                {
                    return null;
                }
            }
        }

        public Item CachedReference
        {
            get
            {
                return ChannelData.CachedItems[this.MapleID];
            }
        }

        public ItemType Type
        {
            get
            {
                return Item.GetType(this.MapleID);
            }
        }

        public WeaponType WeaponType
        {
            get
            {
                switch (this.MapleID / 10000 % 100)
                {
                    case 30:
                        return WeaponType.Sword1H;

                    case 31:
                        return WeaponType.Axe1H;

                    case 32:
                        return WeaponType.Blunt1H;

                    case 33:
                        return WeaponType.Dagger;

                    case 37:
                        return WeaponType.Wand;

                    case 38:
                        return WeaponType.Staff;

                    case 40:
                        return WeaponType.Sword2H;

                    case 41:
                        return WeaponType.Axe2H;

                    case 42:
                        return WeaponType.Blunt2H;

                    case 43:
                        return WeaponType.Spear;

                    case 44:
                        return WeaponType.PoleArm;

                    case 45:
                        return WeaponType.Bow;

                    case 46:
                        return WeaponType.Crossbow;

                    case 47:
                        return WeaponType.Claw;

                    case 48:
                        return WeaponType.Knuckle;

                    case 49:
                        return WeaponType.Gun;

                    default:
                        return WeaponType.NotAWeapon;
                }
            }
        }

        public short MaxPerStack
        {
            get
            {
                if (this.IsRechargeable && this.Parent != null)
                {
                    return maxPerStack; // + TODO: { += 4100000 or 5200000 * 10 }
                }
                else
                {
                    return maxPerStack;
                }
            }
            set
            {
                maxPerStack = value;
            }
        }

        public short Quantity
        {
            get
            {
                return quantity;
            }
            set
            {
                if (value > this.MaxPerStack)
                {
                    throw new ArgumentException("Quantity too high.");
                }
                else
                {
                    quantity = value;
                }
            }
        }

        public byte Flags
        {
            get
            {
                byte flags = 0;

                if (this.IsSealed) flags |= (byte)ItemFlags.Sealed;
                if (this.PreventsSlipping) flags |= (byte)ItemFlags.AddPreventSlipping;
                if (this.PreventsColdness) flags |= (byte)ItemFlags.AddPreventColdness;
                if (this.IsScisored) flags |= (byte)ItemFlags.Scisored;
                if (this.IsTradeBlocked) flags |= (byte)ItemFlags.Untradeable;

                return flags;
            }
        }

        public bool IsSealed
        {
            get
            {
                return ChannelData.CachedItems.WizetItemIDs.Contains(this.MapleID);
            }
        }

        public bool IsEquipped
        {
            get
            {
                return (this.Slot < 0);
            }
        }

        public bool IsEquippedCash
        {
            get
            {
                return (this.Slot < -100);
            }
        }

        public bool IsRechargeable
        {
            get
            {
                return this.IsThrowingStar || this.IsBullet;
            }
        }

        public bool IsThrowingStar
        {
            get
            {
                return this.MapleID / 10000 == 207;
            }
        }

        public bool IsBullet
        {
            get
            {
                return this.MapleID / 10000 == 233;
            }
        }

        public bool IsArrow
        {
            get
            {
                return this.IsArrowForBow || this.IsArrowForCrossbow;
            }
        }

        public bool IsArrowForBow
        {
            get
            {
                return this.MapleID >= 2060000 && this.MapleID < 2061000;
            }
        }

        public bool IsArrowForCrossbow
        {
            get
            {
                return this.MapleID >= 2061000 && this.MapleID < 2062000;
            }
        }

        public bool IsOverall
        {
            get
            {
                return this.MapleID / 10000 == 105;
            }
        }

        public bool IsWeapon
        {
            get
            {
                return this.WeaponType != WeaponType.NotAWeapon;
            }
        }

        public bool IsShield
        {
            get
            {
                return this.MapleID / 10000 % 100 == 9;
            }
        }

        public bool IsPet
        {
            get
            {
                return this.MapleID >= 5000000 && this.MapleID <= 5000100;
            }
        }

        public bool IsTownScroll
        {
            get
            {
                return this.MapleID >= 2030000 && this.MapleID < 2030020;
            }
        }

        public bool IsTwoHanded
        {
            get
            {
                switch (this.WeaponType)
                {
                    case WeaponType.Sword2H:
                    case WeaponType.Axe2H:
                    case WeaponType.Blunt2H:
                    case WeaponType.Spear:
                    case WeaponType.PoleArm:
                    case WeaponType.Bow:
                    case WeaponType.Crossbow:
                    case WeaponType.Claw:
                    case WeaponType.Knuckle:
                    case WeaponType.Gun:
                        return true;

                    default:
                        return false;
                }
            }
        }

        public bool IsBlocked
        {
            get
            {
                return this.IsCash || this.IsSealed || (this.IsTradeBlocked && !this.IsScisored);
            }
        }

        public bool IsScroll
        {
            get
            {
                return this.MapleID / 10000 == 204;
            }
        }

        public bool IsSkillBook
        {
            get
            {
                return this.MapleID / 10000 == 228 || this.MapleID / 10000 == 229;
            }
        }

        public bool IsItemConsume
        {
            get
            {
                return this.MapleID / 10000 >= 200 && this.MapleID / 10000 < 204;
            }
        }

        public byte AbsoluteSlot
        {
            get
            {
                if (this.IsEquipped)
                {
                    return (byte)(this.Slot * -1);
                }
                else
                {
                    throw new InvalidOperationException("Attempting to retrieve absolute slot for non-equipped item.");
                }
            }
        }

        public byte ComputedSlot
        {
            get
            {
                if (this.IsEquippedCash)
                {
                    return ((byte)(this.AbsoluteSlot - 100));
                }
                else if (this.IsEquipped)
                {
                    return this.AbsoluteSlot;
                }
                else
                {
                    return (byte)this.Slot;
                }
            }
        }

        public bool Assigned { get; set; }

        public Item(int mapleId, short quantity = 1, bool equipped = false)
        {
            this.MapleID = mapleId;
            this.MaxPerStack = this.CachedReference.MaxPerStack;
            this.Quantity = (this.Type == ItemType.Equipment) ? (short)1 : quantity;
            if (equipped) this.Slot = (sbyte)this.GetEquippedSlot();
            this.IsStored = false;
            this.Creator = string.Empty;

            this.IsCash = this.CachedReference.IsCash;
            this.OnlyOne = this.CachedReference.OnlyOne;
            this.IsTradeBlocked = this.CachedReference.IsTradeBlocked;
            this.IsScisored = this.CachedReference.IsScisored;
            this.SalePrice = this.CachedReference.SalePrice;
            this.RequiredLevel = this.CachedReference.RequiredLevel;

            this.SerialNumber = this.CachedReference.SerialNumber;
            this.UniqueID = this.CachedReference.UniqueID;

            if (this.Type == ItemType.Equipment)
            {
                this.PreventsSlipping = this.CachedReference.PreventsSlipping;
                this.PreventsColdness = this.CachedReference.PreventsColdness;

                this.AttackSpeed = this.CachedReference.AttackSpeed;
                this.RecoveryRate = this.CachedReference.RecoveryRate;
                this.KnockBackChance = this.CachedReference.KnockBackChance;

                this.RequiredStrength = this.CachedReference.RequiredStrength;
                this.RequiredDexterity = this.CachedReference.RequiredDexterity;
                this.RequiredIntelligence = this.CachedReference.RequiredIntelligence;
                this.RequiredLuck = this.CachedReference.RequiredLuck;
                this.RequiredFame = this.CachedReference.RequiredFame;
                this.RequiredJob = this.CachedReference.RequiredJob;

                this.UpgradesAvailable = this.CachedReference.UpgradesAvailable;
                this.UpgradesApplied = this.CachedReference.UpgradesApplied;
                this.MaxUpgradesAvailable = this.CachedReference.MaxUpgradesAvailable;
                this.Strength = this.CachedReference.Strength;
                this.Dexterity = this.CachedReference.Dexterity;
                this.Intelligence = this.CachedReference.Intelligence;
                this.Luck = this.CachedReference.Luck;
                this.HP = this.CachedReference.HP;
                this.MP = this.CachedReference.MP;
                this.WeaponAttack = this.CachedReference.WeaponAttack;
                this.MagicAttack = this.CachedReference.MagicAttack;
                this.WeaponDefense = this.CachedReference.WeaponDefense;
                this.MagicDefense = this.CachedReference.MagicDefense;
                this.Accuracy = this.CachedReference.Accuracy;
                this.Avoidability = this.CachedReference.Avoidability;
                this.Agility = this.CachedReference.Agility;
                this.Speed = this.CachedReference.Speed;
                this.Jump = this.CachedReference.Jump;
                this.ViciousHammerApplied = this.CachedReference.ViciousHammerApplied;
                this.Potential = (Maple.Potential)this.CachedReference.Potential;
                this.Stars = this.CachedReference.Stars;
                this.Potential1 = this.CachedReference.Potential1;
                this.Potential2 = this.CachedReference.Potential2;
                this.Potential3 = this.CachedReference.Potential3;
                this.PotentialLines = this.CachedReference.PotentialLines;
            }
            else if (this.IsScroll)
            {
                this.Success = this.CachedReference.Success;
                this.BreakItem = this.CachedReference.BreakItem;
                this.Flag = this.CachedReference.Flag;
                this.IStrength = this.CachedReference.IStrength;
                this.IDexterity = this.CachedReference.IDexterity;
                this.IIntelligence = this.CachedReference.IIntelligence;
                this.ILuck = this.CachedReference.ILuck;
                this.IHP = this.CachedReference.IHP;
                this.IMP = this.CachedReference.IMP;
                this.IWeaponAttack = this.CachedReference.IWeaponAttack;
                this.IMagicAttack = this.CachedReference.IMagicAttack;
                this.IWeaponDefense = this.CachedReference.IWeaponDefense;
                this.IMagicDefense = this.CachedReference.IMagicDefense;
                this.IAccuracy = this.CachedReference.IAccuracy;
                this.IAvoidability = this.CachedReference.IAvoidability;
                this.IJump = this.CachedReference.IJump;
                this.ISpeed = this.CachedReference.ISpeed;
            }
            else if (this.IsSkillBook)
            {
                this.SkillId = this.CachedReference.SkillId;
                this.RequestSkillLevel = this.CachedReference.RequestSkillLevel;
                this.MasterLevel = this.CachedReference.MasterLevel;
                this.Chance = this.CachedReference.Chance;
            }
            else if (this.IsItemConsume)
            {
                this.CFlags = this.CachedReference.CFlags;
                this.CCureAilments = this.CachedReference.CCureAilments;
                this.CEffect = this.CachedReference.CEffect;
                this.CHP = this.CachedReference.CHP;
                this.CMP = this.CachedReference.CMP;
                this.CHPPercentage = this.CachedReference.CHPPercentage;
                this.CMPPercentage = this.CachedReference.CMPPercentage;
                this.CMoveTo = this.CachedReference.CMoveTo;
                this.CProb = this.CachedReference.CProb;
                this.CBuffTime = this.CachedReference.CBuffTime;
                this.CWeaponAttack = this.CachedReference.CWeaponAttack;
                this.CMagicAttack = this.CachedReference.CMagicAttack;
                this.CWeaponDefense = this.CachedReference.CWeaponDefense;
                this.CMagicDefense = this.CachedReference.CMagicDefense;
                this.CAccuracy = this.CachedReference.CAccuracy;
                this.CAvoid = this.CachedReference.CAvoid;
                this.CSpeed = this.CachedReference.CSpeed;
                this.CJump = this.CachedReference.CJump;
                this.CMorph = this.CachedReference.CMorph;
            }
        }

        public void LoadEquipmentData(dynamic equipDatum)
        {
            this.PreventsSlipping = equipDatum.traction > 0;
            this.PreventsColdness = this.MapleID == 1472063;

            this.AttackSpeed = equipDatum.attack_speed;
            this.RecoveryRate = equipDatum.recovery > 0 ? equipDatum.recovery : (short)1;
            this.KnockBackChance = equipDatum.knockback;

            this.RequiredStrength = equipDatum.req_str;
            this.RequiredDexterity = equipDatum.req_dex;
            this.RequiredIntelligence = equipDatum.req_int;
            this.RequiredLuck = equipDatum.req_luk;
            this.RequiredFame = equipDatum.req_fame;
            if (equipDatum.req_job != "beginner,warrior,magician,bowman,thief,pirate") this.RequiredJob = (Job)Enum.Parse(typeof(Job), equipDatum.req_job, true);

            this.UpgradesAvailable = (byte)equipDatum.scroll_slots;
            this.MaxUpgradesAvailable = (byte)equipDatum.scroll_slots;
            this.UpgradesApplied = 0;
            this.HP = equipDatum.hp;
            this.MP = equipDatum.hp;
            this.Strength = equipDatum.strength;
            this.Dexterity = equipDatum.dexterity;
            this.Intelligence = equipDatum.intelligence;
            this.Luck = equipDatum.luck;
            this.Agility = equipDatum.hands;
            this.WeaponAttack = equipDatum.weapon_attack;
            this.WeaponDefense = equipDatum.weapon_defense;
            this.MagicAttack = equipDatum.magic_attack;
            this.MagicDefense = equipDatum.magic_defense;
            this.Accuracy = equipDatum.accuracy;
            this.Avoidability = equipDatum.avoid;
            this.Jump = equipDatum.jump;
            this.Speed = equipDatum.speed;
            this.ViciousHammerApplied = 0;
            this.Potential = Maple.Potential.Regular;
            this.Stars = 0;
            this.Potential1 = 0;
            this.Potential2 = 0;
            this.Potential3 = 0;
            this.PotentialLines = 0;
        }

        public void LoadScrollData(dynamic scrollDatum)
        {
            this.Success = scrollDatum.success;
            this.BreakItem = scrollDatum.break_item;
            this.Flag = scrollDatum.flags;
            this.IStrength = scrollDatum.istr;
            this.IDexterity = scrollDatum.idex;
            this.IIntelligence = scrollDatum.iint;
            this.ILuck = scrollDatum.iluk;
            this.IHP = scrollDatum.ihp;
            this.IMP = scrollDatum.imp;
            this.IWeaponAttack = scrollDatum.iwatk;
            this.IMagicAttack = scrollDatum.imatk;
            this.IWeaponDefense = scrollDatum.iwdef;
            this.IMagicDefense = scrollDatum.imdef;
            this.IAccuracy = scrollDatum.iacc;
            this.IAvoidability = scrollDatum.iavo;
            this.IJump = scrollDatum.ijump;
            this.ISpeed = scrollDatum.ispeed;
        }

        public void LoadSkillBookData(dynamic skillBookDatum)
        {
            this.SkillId.Add(skillBookDatum.skillid);
            this.RequestSkillLevel = skillBookDatum.skill_level;
            this.MasterLevel = skillBookDatum.master_level;
            this.Chance = skillBookDatum.chance;
        }

        public void LoadItemConsumeData(dynamic ItemConsumeDatum)
        {
            this.CFlags = ItemConsumeDatum.flags;
            this.CCureAilments = ItemConsumeDatum.cure_ailments;
            this.CEffect = ItemConsumeDatum.effect;
            this.CHP = ItemConsumeDatum.hp;
            this.CMP = ItemConsumeDatum.mp;
            this.CHPPercentage = ItemConsumeDatum.hp_percentage;
            this.CMPPercentage = ItemConsumeDatum.mp_percentage;
            this.CMoveTo = ItemConsumeDatum.move_to;
            this.CProb = ItemConsumeDatum.prob;
            this.CBuffTime = ItemConsumeDatum.buff_time;
            this.CWeaponAttack = ItemConsumeDatum.weapon_attack;
            this.CMagicAttack = ItemConsumeDatum.magic_attack;
            this.CWeaponDefense = ItemConsumeDatum.weapon_defense;
            this.CMagicDefense = ItemConsumeDatum.magic_defense;
            this.CAccuracy = ItemConsumeDatum.accuracy;
            this.CAvoid = ItemConsumeDatum.avoid;
            this.CSpeed = ItemConsumeDatum.speed;
            this.CJump = ItemConsumeDatum.jump;
            this.CMorph = ItemConsumeDatum.morph;
        }

        public Item(dynamic itemDatum)
        {
            if (!ChannelData.IsInitialized)
            {
                this.MapleID = itemDatum.itemid;
                this.MaxPerStack = itemDatum.max_slot_quantity;

                this.IsCash = itemDatum.flags.Contains("cash_item");
                this.OnlyOne = itemDatum.max_possession_count > 0;
                this.IsTradeBlocked = itemDatum.flags.Contains("no_trade");
                this.IsQuestItem = itemDatum.flags.Contains("quest");
                this.IsScisored = false;
                this.SalePrice = itemDatum.price;
                this.RequiredLevel = itemDatum.min_level;
                this.SerialNumber = 0;
                this.UniqueID = 0;
            }
            else
            {
                this.ID = itemDatum.ID;
                this.Assigned = true;

                this.MapleID = itemDatum.MapleID;
                this.MaxPerStack = this.CachedReference.MaxPerStack;
                this.Quantity = itemDatum.Quantity;
                this.Slot = itemDatum.Slot;
                this.IsStored = itemDatum.IsStored;
                this.Creator = itemDatum.Creator;

                this.IsCash = this.CachedReference.IsCash;
                this.OnlyOne = this.CachedReference.OnlyOne;
                this.IsTradeBlocked = this.CachedReference.IsTradeBlocked;
                this.IsScisored = itemDatum.IsScisored;
                this.SalePrice = this.CachedReference.SalePrice;
                this.RequiredLevel = this.CachedReference.RequiredLevel;

                this.SerialNumber = itemDatum.SerialNumber;
                this.UniqueID = itemDatum.UniqueID;

                if (this.Type == ItemType.Equipment)
                {
                    this.PreventsSlipping = itemDatum.PreventsSlipping;
                    this.PreventsColdness = itemDatum.PreventsColdness;

                    this.AttackSpeed = this.CachedReference.AttackSpeed;
                    this.RecoveryRate = this.CachedReference.RecoveryRate;
                    this.KnockBackChance = this.CachedReference.KnockBackChance;

                    this.RequiredStrength = this.CachedReference.RequiredStrength;
                    this.RequiredDexterity = this.CachedReference.RequiredDexterity;
                    this.RequiredIntelligence = this.CachedReference.RequiredIntelligence;
                    this.RequiredLuck = this.CachedReference.RequiredLuck;
                    this.RequiredFame = this.CachedReference.RequiredFame;
                    this.RequiredJob = this.CachedReference.RequiredJob;

                    this.UpgradesAvailable = itemDatum.UpgradesAvailable;
                    this.UpgradesApplied = itemDatum.UpgradesApplied;
                    this.MaxUpgradesAvailable = this.CachedReference.MaxUpgradesAvailable;
                    this.HP = itemDatum.HP;
                    this.MP = itemDatum.MP;
                    this.Strength = itemDatum.Strength;
                    this.Dexterity = itemDatum.Dexterity;
                    this.Intelligence = itemDatum.Intelligence;
                    this.Luck = itemDatum.Luck;
                    this.Agility = itemDatum.Agility;
                    this.WeaponAttack = itemDatum.WeaponAttack;
                    this.WeaponDefense = itemDatum.WeaponDefense;
                    this.MagicAttack = itemDatum.MagicAttack;
                    this.MagicDefense = itemDatum.MagicDefense;
                    this.Accuracy = itemDatum.Accuracy;
                    this.Avoidability = itemDatum.Avoidability;
                    this.Jump = itemDatum.Jump;
                    this.Speed = itemDatum.Speed;
                    this.ViciousHammerApplied = itemDatum.ViciousHammerApplied;
                    this.Potential = (Maple.Potential)itemDatum.Potential;
                    this.Stars = itemDatum.Stars;
                    this.Potential1 = itemDatum.Potential1;
                    this.Potential2 = itemDatum.Potential2;
                    this.Potential3 = itemDatum.Potential3;
                    this.PotentialLines = itemDatum.PotentialLines;
                }
                else if (this.IsScroll)
                {
                    this.Success = this.CachedReference.Success;
                    this.BreakItem = this.CachedReference.BreakItem;
                    this.Flag = this.CachedReference.Flag;
                    this.IStrength = this.CachedReference.IStrength;
                    this.IDexterity = this.CachedReference.IDexterity;
                    this.IIntelligence = this.CachedReference.IIntelligence;
                    this.ILuck = this.CachedReference.ILuck;
                    this.IHP = this.CachedReference.IHP;
                    this.IMP = this.CachedReference.IMP;
                    this.IWeaponAttack = this.CachedReference.IWeaponAttack;
                    this.IMagicAttack = this.CachedReference.IMagicAttack;
                    this.IWeaponDefense = this.CachedReference.IWeaponDefense;
                    this.IMagicDefense = this.CachedReference.IMagicDefense;
                    this.IAccuracy = this.CachedReference.IAccuracy;
                    this.IAvoidability = this.CachedReference.IAvoidability;
                    this.IJump = this.CachedReference.IJump;
                    this.ISpeed = this.CachedReference.ISpeed;
                }
                else if (this.IsSkillBook)
                {
                    this.SkillId = this.CachedReference.SkillId;
                    this.RequestSkillLevel = this.CachedReference.RequestSkillLevel;
                    this.MasterLevel = this.CachedReference.MasterLevel;
                    this.Chance = this.CachedReference.Chance;
                }
                else if (this.IsItemConsume)
                {
                    this.CFlags = this.CachedReference.CFlags;
                    this.CCureAilments = this.CachedReference.CCureAilments;
                    this.CEffect = this.CachedReference.CEffect;
                    this.CHP = this.CachedReference.CHP;
                    this.CMP = this.CachedReference.CMP;
                    this.CHPPercentage = this.CachedReference.CHPPercentage;
                    this.CMPPercentage = this.CachedReference.CMPPercentage;
                    this.CMoveTo = this.CachedReference.CMoveTo;
                    this.CProb = this.CachedReference.CProb;
                    this.CBuffTime = this.CachedReference.CBuffTime;
                    this.CWeaponAttack = this.CachedReference.CWeaponAttack;
                    this.CMagicAttack = this.CachedReference.CMagicAttack;
                    this.CWeaponDefense = this.CachedReference.CWeaponDefense;
                    this.CMagicDefense = this.CachedReference.CMagicDefense;
                    this.CAccuracy = this.CachedReference.CAccuracy;
                    this.CAvoid = this.CachedReference.CAvoid;
                    this.CSpeed = this.CachedReference.CSpeed;
                    this.CJump = this.CachedReference.CJump;
                    this.CMorph = this.CachedReference.CMorph;
                }
            }
        }

        public void Save()
        {
            dynamic datum = new Datum("items");

            datum.CharacterID = this.Character.ID;
            datum.MapleID = this.MapleID;
            datum.Quantity = this.Quantity;
            datum.Slot = this.Slot;
            datum.IsStored = this.IsStored;
            datum.Creator = this.Creator;
            datum.IsScisored = this.IsScisored;
            datum.PreventsSlipping = this.PreventsSlipping;
            datum.PreventsColdness = this.PreventsColdness;
            datum.UpgradesAvailable = this.UpgradesAvailable;
            datum.UpgradesApplied = this.UpgradesApplied;
            datum.HP = this.HP;
            datum.MP = this.MP;
            datum.Strength = this.Strength;
            datum.Dexterity = this.Dexterity;
            datum.Intelligence = this.Intelligence;
            datum.Luck = this.Luck;
            datum.Agility = this.Agility;
            datum.WeaponAttack = this.WeaponAttack;
            datum.WeaponDefense = this.WeaponDefense;
            datum.MagicAttack = this.MagicAttack;
            datum.MagicDefense = this.MagicDefense;
            datum.Accuracy = this.Accuracy;
            datum.Avoidability = this.Avoidability;
            datum.Jump = this.Jump;
            datum.Speed = this.Speed;
            datum.ViciousHammerApplied = this.ViciousHammerApplied;
            datum.Potential = (byte)this.Potential;
            datum.Stars = this.Stars;
            datum.Potential1 = this.Potential1;
            datum.Potential2 = this.Potential2;
            datum.Potential3 = this.Potential3;
            datum.PotentialLines = this.PotentialLines;
            datum.SerialNumber = this.SerialNumber;
            datum.UniqueID = this.UniqueID;

            if (this.Assigned)
            {
                datum.Update("ID = '{0}'", this.ID);
            }
            else
            {
                datum.Insert();

                this.ID = Database.Fetch("items", "ID", "CharacterID = '{0}' && MapleID = '{1}' && Slot = '{2}'", this.Character.ID, this.MapleID, this.Slot);

                this.Assigned = true;
            }
        }

        public void SaveToStorage(Character parent)
        {
            dynamic datum = new Datum("storage_items");

            datum.AccountID = parent.AccountID;
            datum.MapleID = this.MapleID;
            datum.Quantity = this.Quantity;
            datum.Slot = this.Slot;
            datum.IsStored = this.IsStored;
            datum.Creator = this.Creator;
            datum.IsScisored = this.IsScisored;
            datum.PreventsSlipping = this.PreventsSlipping;
            datum.PreventsColdness = this.PreventsColdness;
            datum.UpgradesAvailable = this.UpgradesAvailable;
            datum.UpgradesApplied = this.UpgradesApplied;
            datum.HP = this.HP;
            datum.MP = this.MP;
            datum.Strength = this.Strength;
            datum.Dexterity = this.Dexterity;
            datum.Intelligence = this.Intelligence;
            datum.Luck = this.Luck;
            datum.Agility = this.Agility;
            datum.WeaponAttack = this.WeaponAttack;
            datum.WeaponDefense = this.WeaponDefense;
            datum.MagicAttack = this.MagicAttack;
            datum.MagicDefense = this.MagicDefense;
            datum.Accuracy = this.Accuracy;
            datum.Avoidability = this.Avoidability;
            datum.Jump = this.Jump;
            datum.Speed = this.Speed;
            datum.ViciousHammerApplied = this.ViciousHammerApplied;
            datum.Potential = (byte)this.Potential;
            datum.Stars = this.Stars;
            datum.Potential1 = this.Potential1;
            datum.Potential2 = this.Potential2;
            datum.Potential3 = this.Potential3;
            datum.PotentialLines = this.PotentialLines;
            datum.SerialNumber = this.SerialNumber;
            datum.UniqueID = this.UniqueID;
            datum.Insert();
        }

        public void Delete()
        {
            Database.Delete("items", "ID = '{0}'", this.ID);
            this.Assigned = false;
        }

        public void DeleteFromStorage()
        {
            Database.Delete("storage_items", "ID = '{0}'", this.ID);
            this.Assigned = false;
        }

        public void Equip()
        {
            if (this.Type != ItemType.Equipment)
            {
                throw new InvalidOperationException("Can only equip equipment items.");
            }
            else if ((this.Character.Strength < this.RequiredStrength ||
                this.Character.Dexterity < this.RequiredDexterity ||
                this.Character.Intelligence < this.RequiredIntelligence ||
                this.Character.Luck < this.RequiredLuck) &&
                !(this.Character.Job == Job.GM || this.Character.Job == Job.SuperGM)) // TODO: Job requirement.
            {
                throw new HackException("Equipping item without requirements.");
            }
            else
            {
                sbyte sourceSlot = this.Slot;
                EquipmentSlot destinationSlot = this.GetEquippedSlot();

                Item top = this.Parent[EquipmentSlot.Top];
                Item bottom = this.Parent[EquipmentSlot.Bottom];
                Item weapon = this.Parent[EquipmentSlot.Weapon];
                Item shield = this.Parent[EquipmentSlot.Shield];

                Item destination = this.Parent[destinationSlot];

                if (destination != null)
                {
                    destination.Slot = sourceSlot;
                }

                this.Slot = (sbyte)destinationSlot;

                using (Packet outPacket = new Packet(MapleServerOperationCode.ModifyInventoryItem))
                {
                    outPacket.WriteBytes(1, 1, 0, 2);
                    outPacket.WriteByte((byte)this.Type);
                    outPacket.WriteShort((short)sourceSlot);
                    outPacket.WriteShort((short)destinationSlot);
                    outPacket.WriteByte((byte)EquipmentManipulation.Equip);

                    this.Character.Client.Send(outPacket);
                }

                switch (destinationSlot)
                {
                    case EquipmentSlot.Bottom:
                        if (top != null && top.IsOverall)
                        {
                            top.Unequip();
                        }

                        break;

                    case EquipmentSlot.Top:
                        if (this.IsOverall && bottom != null)
                        {
                            bottom.Unequip();
                        }

                        break;

                    case EquipmentSlot.Shield:
                        if (weapon != null && weapon.IsTwoHanded)
                        {
                            weapon.Unequip();
                        }

                        break;

                    case EquipmentSlot.Weapon:
                        if (this.IsTwoHanded && shield != null)
                        {
                            shield.Unequip();
                        }

                        // TODO: Cancel booster buffs.

                        break;
                }

                this.Character.UpdateLook();
            }
        }

        public void Unequip(sbyte destination = 0)
        {
            if (this.Type != ItemType.Equipment)
            {
                throw new InvalidOperationException("Can only unequip equipment items.");
            }
            else
            {
                sbyte source = this.Slot;

                if (destination == 0)
                {
                    destination = this.Parent.GetNextFreeSlot(ItemType.Equipment);
                }

                this.Slot = destination;

                using (Packet outPacket = new Packet(MapleServerOperationCode.ModifyInventoryItem))
                {
                    outPacket.WriteBytes(1, 1, 0, 2);
                    outPacket.WriteByte((byte)this.Type);
                    outPacket.WriteShort((short)source);
                    outPacket.WriteShort((short)destination);
                    outPacket.WriteByte((byte)EquipmentManipulation.Equip);

                    this.Character.Client.Send(outPacket);
                }

                this.Character.UpdateLook();
            }
        }

        public void Move(sbyte destinationSlot)
        {
            sbyte sourceSlot = this.Slot;

            Item destination = this.Parent[this.Type, destinationSlot];

            if (destination != null &&
                this.Type != ItemType.Equipment &&
                this.MapleID == destination.MapleID &&
                !this.IsRechargeable &&
                destination.Quantity < destination.MaxPerStack)
            {
                if (this.Quantity + destination.Quantity > destination.MaxPerStack)
                {
                    this.Quantity -= (short)(destination.MaxPerStack - destination.Quantity);
                    destination.Quantity = destination.MaxPerStack;

                    using (Packet outPacket = new Packet(MapleServerOperationCode.ModifyInventoryItem))
                    {
                        outPacket.WriteBytes(1, 2, 0, 1);
                        outPacket.WriteByte((byte)this.Type);
                        outPacket.WriteShort((short)sourceSlot);
                        outPacket.WriteShort((short)this.Quantity);
                        outPacket.WriteByte(1);
                        outPacket.WriteByte((byte)destination.Type);
                        outPacket.WriteShort((short)destinationSlot);
                        outPacket.WriteShort(destination.Quantity);

                        this.Character.Client.Send(outPacket);
                    }
                }
                else
                {
                    destination.Quantity += this.Quantity;

                    using (Packet outPacket = new Packet(MapleServerOperationCode.ModifyInventoryItem))
                    {
                        outPacket.WriteBytes(1, 2, 0, 3);
                        outPacket.WriteByte((byte)this.Type);
                        outPacket.WriteShort((short)sourceSlot);
                        outPacket.WriteByte(1); // UNK
                        outPacket.WriteByte((byte)destination.Type);
                        outPacket.WriteShort((short)destinationSlot);
                        outPacket.WriteShort(destination.Quantity);

                        this.Character.Client.Send(outPacket);
                    }

                    this.Parent.Remove(this, false);
                }
            }
            else
            {
                if (destination != null)
                {
                    destination.Slot = sourceSlot;
                }

                this.Slot = destinationSlot;

                using (Packet outPacket = new Packet(MapleServerOperationCode.ModifyInventoryItem))
                {
                    outPacket.WriteBytes(1, 1, 0, 2);
                    outPacket.WriteByte((byte)this.Type);
                    outPacket.WriteShort((short)sourceSlot);
                    outPacket.WriteShort((short)destinationSlot);

                    this.Character.Client.Send(outPacket);
                }
            }
        }

        public void Drop(short quantity)
        {
            if (this.IsRechargeable)
            {
                quantity = this.Quantity;
            }

            if (this.IsBlocked)
            {
                throw new HackException("Dropping blocked item.");
            }
            else if (quantity == this.Quantity)
            {
                using (Packet outPacket = new Packet(MapleServerOperationCode.ModifyInventoryItem))
                {
                    outPacket.WriteBytes(1, 1, 0, 3);
                    outPacket.WriteByte((byte)this.Type);
                    outPacket.WriteShort((short)this.Slot);

                    if (this.IsEquipped)
                    {
                        outPacket.WriteBool(true);
                    }

                    this.Character.Client.Send(outPacket);
                }

                this.Dropper = this.Character;
                this.Owner = null;

                this.Character.Map.Drops.Add(this);
                this.Parent.Remove(this, false);
            }
            else if (quantity < this.Quantity)
            {
                this.Quantity -= quantity;

                using (Packet outPacket = new Packet(MapleServerOperationCode.ModifyInventoryItem))
                {
                    outPacket.WriteBytes(1, 1, 0, 1);
                    outPacket.WriteBytes(1, 1, 0, 1);
                    outPacket.WriteByte((byte)this.Type);
                    outPacket.WriteShort((short)this.Slot);
                    outPacket.WriteShort(this.Quantity);

                    this.Character.Client.Send(outPacket);
                }

                Item dropped = new Item(this.MapleID, quantity)
                {
                    Dropper = this.Character,
                    Owner = null
                };

                this.Character.Map.Drops.Add(dropped);
            }
            else if (quantity > this.Quantity)
            {
                throw new HackException("Dropping more than available.");
            }
        }

        public void Update()
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.ModifyInventoryItem))
            {
                outPacket.WriteBool(true); // TODO: From drop.
                outPacket.WriteBytes(1, 0, 1);
                outPacket.WriteByte((byte)this.Type);
                outPacket.WriteShort((short)this.Slot);
                outPacket.WriteShort(this.Quantity);

                this.Character.Client.Send(outPacket);
            }
        }

        public byte[] ToByteArray(bool zeroPosition = false, bool leaveOut = false)
        {
            using (ByteBuffer buffer = new ByteBuffer())
            {
                if (!zeroPosition && !leaveOut)
                {
                    byte pos = this.ComputedSlot;
                    if (pos < 0)
                        pos = (byte)(pos * -1);
                    if (pos > 100)
                        pos -= 100;
                    if (this.Type == ItemType.Equipment)
                        buffer.WriteShort(pos);
                    else
                        buffer.WriteByte(pos);
                }

                buffer.WriteByte((byte)(this.Type == ItemType.Equipment ? 1 : 2));
                buffer.WriteInt(this.MapleID);
                buffer.WriteBool(this.IsCash);

                if (this.IsCash)
                {
                    buffer.WriteLong(this.UniqueID);
                }

                buffer.WriteLong((long)ExpirationTime.DefaultTime);
                buffer.WriteInt(-1);

                if (this.Type == ItemType.Equipment)
                {
                    buffer.WriteByte(this.UpgradesAvailable);
                    buffer.WriteByte(this.UpgradesApplied);
                    buffer.WriteShort(this.Strength);
                    buffer.WriteShort(this.Dexterity);
                    buffer.WriteShort(this.Intelligence);
                    buffer.WriteShort(this.Luck);
                    buffer.WriteShort(this.HP);
                    buffer.WriteShort(this.MP);
                    buffer.WriteShort(this.WeaponAttack);
                    buffer.WriteShort(this.MagicAttack);
                    buffer.WriteShort(this.WeaponDefense);
                    buffer.WriteShort(this.MagicDefense);
                    buffer.WriteShort(this.Accuracy);
                    buffer.WriteShort(this.Avoidability);
                    buffer.WriteShort(this.Agility);
                    buffer.WriteShort(this.Speed);
                    buffer.WriteShort(this.Jump);
                    buffer.WriteString(this.Creator);
                    buffer.WriteShort(this.Flags);
                    buffer.WriteByte();
                    buffer.WriteByte(1); // TODO: Item level. Timeless has it.
                    buffer.WriteInt(); // TODO: Item EXP. Timeless has it.
                    buffer.WriteInt(-1);
                    buffer.WriteInt(this.ViciousHammerApplied);
                    buffer.WriteShort(); // PVP stuff
                    buffer.WriteByte((byte)this.Potential); // 0/4 = No potential, 1/2/3 = Hidden potential, 5 = Rare, 6 = Epic, 7 = Unique
                    buffer.WriteByte(this.Stars); // stars
                    buffer.WriteShort(this.Potential1); // potential stat 1
                    buffer.WriteShort(this.Potential2); // potential stat 2
                    buffer.WriteShort(this.Potential3); // potential stat 3
                    buffer.WriteShort(); // potential stat 4 ?
                    buffer.WriteShort(); // potential stat 5 ?
                    buffer.WriteShort();
                    buffer.WriteShort(-1);
                    buffer.WriteShort(-1);
                    buffer.WriteShort(-1);

                    if (!this.IsEquippedCash && !this.IsCash)
                    {
                        buffer.WriteLong(-1);
                    }

                    buffer.WriteBytes(0x00, 0x40, 0xE0, 0xFD, 0x3B, 0x37, 0x4F, 0x01);
                    buffer.WriteInt(-1);
                }
                else
                {
                    buffer.WriteShort(this.Quantity);
                    buffer.WriteString(this.Creator);
                    buffer.WriteShort(this.Flags);

                    if (this.IsRechargeable)
                    {
                        buffer.WriteBytes(0x02, 0x00, 0x00, 0x00, 0x54, 0x00, 0x00, 0x34);
                    }
                }

                buffer.Flip();

                return buffer.GetContent();
            }
        }

        private EquipmentSlot GetEquippedSlot()
        {
            sbyte position = 0;

            if (this.MapleID >= 1000000 && this.MapleID < 1010000)
            {
                position -= 1;
            }
            else if (this.MapleID >= 1010000 && this.MapleID < 1020000)
            {
                position -= 2;
            }
            else if (this.MapleID >= 1020000 && this.MapleID < 1030000)
            {
                position -= 3;
            }
            else if (this.MapleID >= 1030000 && this.MapleID < 1040000)
            {
                position -= 4;
            }
            else if (this.MapleID >= 1040000 && this.MapleID < 1060000)
            {
                position -= 5;
            }
            else if (this.MapleID >= 1060000 && this.MapleID < 1070000)
            {
                position -= 6;
            }
            else if (this.MapleID >= 1070000 && this.MapleID < 1080000)
            {
                position -= 7;
            }
            else if (this.MapleID >= 1080000 && this.MapleID < 1090000)
            {
                position -= 8;
            }
            else if (this.MapleID >= 1102000 && this.MapleID < 1103000)
            {
                position -= 9;
            }
            else if (this.MapleID >= 1092000 && this.MapleID < 1100000)
            {
                position -= 10;
            }
            else if (this.MapleID >= 1300000 && this.MapleID < 1800000)
            {
                position -= 11;
            }
            else if (this.MapleID >= 1112000 && this.MapleID < 1120000)
            {
                position -= 12;
            }
            else if (this.MapleID >= 1122000 && this.MapleID < 1123000)
            {
                position -= 17;
            }
            else if (this.MapleID >= 1900000 && this.MapleID < 2000000)
            {
                position -= 18;
            }

            if (this.IsCash)
            {
                position -= 100;
            }

            return (EquipmentSlot)position;
        }

        public override Packet GetShowGainPacket()
        {
            Packet showGain = new Packet(MapleServerOperationCode.ShowItemGainInChat);

            showGain.WriteBytes(5, 1);
            showGain.WriteInt(((Item)this).MapleID);
            showGain.WriteInt(((Item)this).Quantity);

            return showGain;
        }
    }
}
