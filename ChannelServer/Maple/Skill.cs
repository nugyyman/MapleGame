using System;
using Loki.Data;
using Loki.Maple.Characters;
using Loki.Maple.Data;
using Loki.Net;
using Loki.Threading;

namespace Loki.Maple
{
    public class Skill
    {
        public CharacterSkills Parent { get; set; }

        private byte currentLevel;
        private byte maxLevel;
        private DateTime cooldownEnd = DateTime.MinValue;

        public int ID { get; private set; }
        public int MapleID { get; set; }

        public sbyte MobCount { get; set; }
        public sbyte HitCount { get; set; }
        public short Range { get; set; }
        public int BuffTime { get; set; }
        public short CostMP { get; set; }
        public short CostHP { get; set; }
        public short Damage { get; set; }
        public int FixedDamage { get; set; }
        public byte CriticalDamage { get; set; }
        public sbyte Mastery { get; set; }
        public int OptionalItemCost { get; set; }
        public int CostItem { get; set; }
        public short ItemCount { get; set; }
        public short CostBullet { get; set; }
        public short CostMeso { get; set; }
        public short ParameterA { get; set; }
        public short ParameterB { get; set; }
        public short Speed { get; set; }
        public short Jump { get; set; }
        public short Strength { get; set; }
        public short WeaponAttack { get; set; }
        public short WeaponDefense { get; set; }
        public short MagicAttack { get; set; }
        public short MagicDefense { get; set; }
        public short Accuracy { get; set; }
        public short Avoid { get; set; }
        public short HP { get; set; }
        public short MP { get; set; }
        public short Probability { get; set; }
        public short Morph { get; set; }
        public Point LT { get; private set; }
        public Point RB { get; private set; }
        public int Cooldown { get; set; }

        public bool HasBuff
        {
            get
            {
                return this.BuffTime > 0;
            }
        }

        public byte CurrentLevel
        {
            get
            {
                return currentLevel;
            }
            set
            {
                currentLevel = value;

                if (this.Parent != null)
                {
                    this.Recalculate();

                    if (this.Character.IsInitialized)
                    {
                        this.Update();
                    }
                }
            }
        }

        public byte MaxLevel
        {
            get
            {
                return maxLevel;
            }
            set
            {
                maxLevel = value;

                if (this.Parent != null && this.Character.IsInitialized)
                {
                    this.Update();
                }
            }
        }

        public Skill CachedReference
        {
            get
            {
                return World.CachedSkills[this.MapleID][this.CurrentLevel];
            }
        }

        public Character Character
        {
            get
            {
                return this.Parent.Parent;
            }
        }

        public bool IsFromFourthJob
        {
            get
            {
                return this.MapleID > 1000000 && (this.MapleID / 10000).ToString()[2] == '2'; // TODO: Redo that.
            }
        }

        public bool IsFromBeginner
        {
            get
            {
                return this.MapleID % 10000000 > 999 && this.MapleID % 10000000 < 1003;
            }
        }

        public Skill(int mapleId) // Used for used for non-fourth-job skills
        {
            this.MapleID = mapleId;
            this.CurrentLevel = 0;
            this.MaxLevel = (byte)World.CachedSkills[this.MapleID].Count;
        }

        public Skill(int mapleId, byte currentLevel, byte maxLevel) // Used for fourth job skills
        {
            this.MapleID = mapleId;
            this.currentLevel = currentLevel;
            this.maxLevel = maxLevel;
        }

        public Skill(dynamic skillDatum)
        {
            if (!World.IsInitialized)
            {
                this.MapleID = skillDatum.skillid;
                this.CurrentLevel = (byte)skillDatum.skill_level;
                this.MobCount = (sbyte)skillDatum.mob_count;
                this.HitCount = (sbyte)skillDatum.hit_count;
                this.Range = (short)skillDatum.range;
                this.BuffTime = skillDatum.buff_time;
                this.CostMP = (short)skillDatum.mp_cost;
                this.CostHP = (short)skillDatum.hp_cost;
                this.Damage = (short)skillDatum.damage;
                this.FixedDamage = (short)skillDatum.fixed_damage;
                this.CriticalDamage = (byte)skillDatum.critical_damage;
                this.Mastery = (sbyte)skillDatum.mastery;
                this.OptionalItemCost = (short)skillDatum.optional_item_cost;
                this.CostItem = (short)skillDatum.item_cost;
                this.ItemCount = (short)skillDatum.item_count;
                this.CostBullet = (short)skillDatum.bullet_cost;
                this.CostMeso = (short)skillDatum.money_cost;
                this.ParameterA = (short)skillDatum.x_property;
                this.ParameterB = (short)skillDatum.y_property;
                this.Speed = (short)skillDatum.speed;
                this.Jump = (short)skillDatum.jump;
                this.Strength = (short)skillDatum.str;
                this.WeaponAttack = (short)skillDatum.weapon_atk;
                this.WeaponDefense = (short)skillDatum.weapon_def;
                this.MagicAttack = (short)skillDatum.magic_atk;
                this.MagicDefense = (short)skillDatum.magic_def;
                this.Accuracy = (short)skillDatum.accuracy;
                this.Avoid = (short)skillDatum.avoid;
                this.HP = (short)skillDatum.hp;
                this.MP = (short)skillDatum.mp;
                this.Probability = (short)skillDatum.prop;
                this.Morph = (short)skillDatum.morph;
                this.LT = new Point(skillDatum.ltx, skillDatum.lty);
                this.RB = new Point(skillDatum.rbx, skillDatum.rby);
                this.Cooldown = (short)skillDatum.cooldown_time;
            }
            else
            {
                this.ID = skillDatum.ID;
                this.Assigned = true;

                this.MapleID = skillDatum.MapleID;
                this.CurrentLevel = skillDatum.CurrentLevel;
                this.MaxLevel = skillDatum.MaxLevel;
                this.CooldownEnd = skillDatum.CooldownEnd;
            }
        }

        public void Recalculate()
        {
            this.MobCount = this.CachedReference.MobCount;
            this.HitCount = this.CachedReference.HitCount;
            this.Range = this.CachedReference.Range;
            this.BuffTime = this.CachedReference.BuffTime;
            this.CostMP = this.CachedReference.CostMP;
            this.CostHP = this.CachedReference.CostHP;
            this.Damage = this.CachedReference.Damage;
            this.FixedDamage = this.CachedReference.FixedDamage;
            this.CriticalDamage = this.CachedReference.CriticalDamage;
            this.Mastery = this.CachedReference.Mastery;
            this.OptionalItemCost = this.CachedReference.OptionalItemCost;
            this.CostItem = this.CachedReference.CostItem;
            this.ItemCount = this.CachedReference.ItemCount;
            this.CostBullet = this.CachedReference.CostBullet;
            this.CostMeso = this.CachedReference.CostMeso;
            this.ParameterA = this.CachedReference.ParameterA;
            this.ParameterB = this.CachedReference.ParameterB;
            this.Speed = this.CachedReference.Speed;
            this.Jump = this.CachedReference.Jump;
            this.Strength = this.CachedReference.Strength;
            this.WeaponAttack = this.CachedReference.WeaponAttack;
            this.WeaponDefense = this.CachedReference.WeaponDefense;
            this.MagicAttack = this.CachedReference.MagicAttack;
            this.MagicDefense = this.CachedReference.MagicDefense;
            this.Accuracy = this.CachedReference.Accuracy;
            this.Avoid = this.CachedReference.Avoid;
            this.HP = this.CachedReference.HP;
            this.MP = this.CachedReference.MP;
            this.Probability = this.CachedReference.Probability;
            this.Morph = this.CachedReference.Morph;
            this.LT = this.CachedReference.LT;
            this.RB = this.CachedReference.RB;
            this.Cooldown = this.CachedReference.Cooldown;
        }

        private bool Assigned { get; set; } // TODO: Move the assigneds somewhere good.

        public void Save()
        {
            dynamic datum = new Datum("skills");

            datum.CharacterID = this.Character.ID;
            datum.MapleID = this.MapleID;
            datum.CurrentLevel = this.CurrentLevel;
            datum.MaxLevel = this.MaxLevel;
            datum.CooldownEnd = this.CooldownEnd;

            if (this.Assigned)
            {
                datum.Update("ID = '{0}'", this.ID);
            }
            else
            {
                datum.Insert();

                this.ID = Database.Fetch("skills", "ID", "CharacterID = '{0}' && MapleID = '{1}'", this.Character.ID, this.MapleID);

                this.Assigned = true;
            }
        }

        public void Delete()
        {
            Database.Delete("skills", "ID = '{0}'", this.ID);
            this.Assigned = false;
        }

        public void Update()
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.UpdateSkills))
            {
                outPacket.WriteByte(1);
                outPacket.WriteShort(1);
                outPacket.WriteInt(this.MapleID);
                outPacket.WriteInt(this.CurrentLevel);
                outPacket.WriteInt(this.MaxLevel);

                // TODO: This is expiration time: Implement it.
                outPacket.WriteByte();
                outPacket.WriteBytes(PacketConstants.Item);
                outPacket.WriteBytes((byte)0xBB, 0x46, (byte)0xE6, 0x17);
                outPacket.WriteByte(2); // 1 to show it, 2 to hide it.

                outPacket.WriteByte(1);

                this.Character.Client.Send(outPacket);
            }
        }

        public bool IsCoolingDown
        {
            get
            {
                return DateTime.Now < this.CooldownEnd;
            }
        }

        public int RemainingCooldownSeconds
        {
            get
            {
                return Math.Min(0, (int)(this.CooldownEnd - DateTime.Now).TotalSeconds);
            }
        }

        public DateTime CooldownEnd
        {
            get
            {
                return cooldownEnd;
            }
            set
            {
                cooldownEnd = value;

                if (this.IsCoolingDown)
                {
                    using (Packet outPacket = new Packet(MapleServerOperationCode.Cooldown))
                    {
                        outPacket.WriteInt(this.MapleID);
                        outPacket.WriteInt((short)this.RemainingCooldownSeconds);

                        this.Character.Client.Send(outPacket);
                    }

                    Delay.Execute(this.RemainingCooldownSeconds * 1000, () =>
                    {
                        using (Packet outPacket = new Packet(MapleServerOperationCode.Cooldown))
                        {
                            outPacket.WriteInt(this.MapleID);
                            outPacket.WriteInt(0);

                            this.Character.Client.Send(outPacket);
                        }
                    });
                }
            }
        }

        public bool Cast()
        {
            if (this.IsCoolingDown)
            {
                throw new HackException("Casting skill while cooldown.");
            }

            this.Character.CurrentHP -= this.CostHP;
            this.Character.CurrentMP -= this.CostMP;

            if (this.Cooldown > 0)
            {
                this.CooldownEnd = DateTime.Now.AddSeconds(this.Cooldown);
            }

            if (this.HasBuff)
            {
                this.Character.Buffs.Add(this, 0); // TODO: Value.
                this.ShowBuffEffect(this.Character, 1, 3);
            }
            else
            {
                switch (this.MapleID)
                {
                    case (int)SkillNames.GM.HasteNormal:
                    case (int)SkillNames.SuperGM.Bless:
                    case (int)SkillNames.SuperGM.HasteSuper:
                    case (int)SkillNames.SuperGM.HealDispel:
                    case (int)SkillNames.SuperGM.HolySymbol:
                    case (int)SkillNames.SuperGM.HyperBody:
                    case (int)SkillNames.SuperGM.Resurrection:
                        this.ShowBuffEffect(this.Character, 1, 3);
                        break;
                }
            }

            switch (this.MapleID)
            {
                case 9101001:
                    return true;
            }

            return true;
        }

        public void ShowBuffEffect(Character character, byte effect, byte direction)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.ShowItemGainInChat))
            {
                outPacket.WriteByte(effect);
                outPacket.WriteInt(this.MapleID);
                outPacket.WriteByte(0xA9);
                outPacket.WriteByte(1);

                character.Client.Send(outPacket);
            }

            using (Packet outPacket = new Packet(MapleServerOperationCode.ShowForeignEffect))
            {
                outPacket.WriteInt(character.ObjectID);

                if (this.MapleID == (int)SkillNames.Buccaneer.SuperTransformation || this.MapleID == (int)SkillNames.Marauder.Transformation || this.MapleID == (int)SkillNames.WindArcher3.EagleEye || this.MapleID == (int)SkillNames.ThunderBreaker3.Transformation)
                {
                    outPacket.WriteByte(1);
                    outPacket.WriteInt(this.MapleID);
                    outPacket.WriteByte(direction);
                }
                else
                {
                    outPacket.WriteByte(effect); //buff level
                    outPacket.WriteInt(this.MapleID);
                    outPacket.WriteByte(1);

                    if (direction != (byte)3)
                    {
                        outPacket.WriteByte(direction);
                    }
                }

                character.Map.Broadcast(character, outPacket);
            }
        }
    }
}
