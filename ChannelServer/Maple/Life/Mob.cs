using System;
using System.Collections.Generic;
using Loki.Maple.Characters;
using Loki.Maple.Data;
using Loki.Maple.Maps;
using Loki.Net;
using Loki.Threading;

namespace Loki.Maple.Life
{
    public class Mob : MapObject, ISpawnable
    {
        public int MapleID { get; private set; }
        public Character Controller { get; set; }
        public Dictionary<Character, uint> Attackers { get; private set; }
        public SpawnPoint SpawnPoint { get; private set; }
        public byte Stance { get; set; }
        public bool IsProvoked { get; set; }
        public bool CanDrop { get; set; }
        public List<Loot> Loots { get; private set; }
        public short Foothold { get; set; }
        public MobSkills Skills { get; private set; }
        public Dictionary<MobSkill, DateTime> Cooldowns { get; private set; }
        public List<MobStatus> Buffs { get; private set; }
        public List<int> DeathSummons { get; private set; }

        public short Level { get; private set; }
        //public FLAGS Flags { get; private set; }
        public uint MaxHP { get; private set; }
        public uint MaxMP { get; private set; }
        public uint CurrentHP { get; set; }
        public uint CurrentMP { get; set; }
        public uint HpRecovery { get; private set; }
        public uint MpRecovery { get; private set; }
        public int ExplodeHP { get; private set; }
        public uint Experience { get; private set; }
        public int Link { get; private set; }
        public short SummonType { get; private set; }
        public int KnockBack { get; private set; }
        public int FixedDamage { get; private set; }
        public int DeathBuff { get; private set; }
        public int DeathAfter { get; private set; }
        public double Traction { get; private set; }
        public int DamagedBySkillOnly { get; private set; }
        public int DamagedByMobOnly { get; private set; }
        public int DropItemPeriod { get; private set; }
        public byte HpBarForeColor { get; private set; }
        public byte HpBarBackColor { get; private set; }
        public byte CarnivalPoints { get; private set; }
        public int WeaponAttack { get; private set; }
        public int WeaponDefense { get; private set; }
        public int MagicAttack { get; private set; }
        public int MagicDefense { get; private set; }
        public short Accuracy { get; private set; }
        public short Avoidability { get; private set; }
        public short Speed { get; private set; }
        public short ChaseSpeed { get; private set; }

        public bool IsFacingLeft
        {
            get
            {
                return Math.Abs(this.Stance) % 2 == 1;
            }
        }

        public Mob CachedReference
        {
            get
            {
                return World.CachedMobs[this.MapleID];
            }
        }

        public Mob(dynamic datum)
            : base()
        {
            this.MapleID = datum.mobid;

            this.Level = datum.mob_level;
            //this.Flags = mobData.flags;
            this.MaxHP = datum.hp;
            this.MaxMP = datum.mp;
            this.CurrentHP = this.MaxHP;
            this.CurrentMP = this.MaxMP;
            this.HpRecovery = datum.hp_recovery;
            this.MpRecovery = datum.mp_recovery;
            this.ExplodeHP = (int)datum.explode_hp;
            this.Experience = datum.experience;
            this.Link = (int)datum.link;
            this.SummonType = (short)datum.summon_type;
            this.KnockBack = (int)datum.knockback;
            this.FixedDamage = (int)datum.fixed_damage;
            this.DeathBuff = (int)datum.death_buff;
            this.DeathAfter = (int)datum.death_after;
            this.Traction = datum.traction;
            this.DamagedBySkillOnly = (int)datum.damaged_by_skill_only;
            this.DamagedByMobOnly = (int)datum.damaged_by_mob_only;
            //this.DropItemPeriod = datum.drop_item_period;
            this.HpBarForeColor = datum.hp_bar_color;
            this.HpBarBackColor = datum.hp_bar_bg_color;
            this.CarnivalPoints = datum.carnival_points;
            this.WeaponAttack = (int)datum.physical_attack;
            this.WeaponDefense = (int)datum.physical_defense;
            this.MagicAttack = (int)datum.magical_attack;
            this.MagicDefense = (int)datum.magical_defense;
            this.Accuracy = (short)datum.accuracy;
            this.Avoidability = (short)datum.avoidability;
            this.Speed = (short)datum.speed;
            this.ChaseSpeed = (short)datum.chase_speed;

            this.Loots = new List<Loot>();
            this.Skills = new MobSkills(this);
            this.DeathSummons = new List<int>();
        }

        public Mob(int mapleId)
            : base()
        {
            this.MapleID = mapleId;

            this.Level = this.CachedReference.Level;
            //this.Flags = this.CachedReference.Flags;
            this.MaxHP = this.CachedReference.MaxHP;
            this.MaxMP = this.CachedReference.MaxMP;
            this.CurrentHP = this.CachedReference.CurrentHP;
            this.CurrentMP = this.CachedReference.CurrentMP;
            this.HpRecovery = this.CachedReference.HpRecovery;
            this.MpRecovery = this.CachedReference.MpRecovery;
            this.ExplodeHP = this.CachedReference.ExplodeHP;
            this.Experience = this.CachedReference.Experience;
            this.Link = this.CachedReference.Link;
            this.SummonType = this.CachedReference.SummonType;
            this.KnockBack = this.CachedReference.KnockBack;
            this.FixedDamage = this.CachedReference.FixedDamage;
            this.DeathBuff = this.CachedReference.DeathBuff;
            this.DeathAfter = this.CachedReference.DeathAfter;
            this.Traction = this.CachedReference.Traction;
            this.DamagedBySkillOnly = this.CachedReference.DamagedBySkillOnly;
            this.DamagedByMobOnly = this.CachedReference.DamagedByMobOnly;
            this.DropItemPeriod = this.CachedReference.DropItemPeriod;
            this.HpBarForeColor = this.CachedReference.HpBarForeColor;
            this.HpBarBackColor = this.CachedReference.HpBarBackColor;
            this.CarnivalPoints = this.CachedReference.CarnivalPoints;
            this.WeaponAttack = this.CachedReference.WeaponAttack;
            this.WeaponDefense = this.CachedReference.WeaponDefense;
            this.MagicAttack = this.CachedReference.MagicAttack;
            this.MagicDefense = this.CachedReference.MagicDefense;
            this.Accuracy = this.CachedReference.Accuracy;
            this.Avoidability = this.CachedReference.Avoidability;
            this.Speed = this.CachedReference.Speed;
            this.ChaseSpeed = this.CachedReference.ChaseSpeed;

            this.Loots = this.CachedReference.Loots;
            this.Skills = this.CachedReference.Skills;
            this.DeathSummons = this.CachedReference.DeathSummons;

            this.Attackers = new Dictionary<Character, uint>();
            this.Cooldowns = new Dictionary<MobSkill, DateTime>();
            this.Buffs = new List<MobStatus>();
            this.Stance = 5;
            this.CanDrop = true;
        }

        public Mob(SpawnPoint spawnPoint)
            : this(spawnPoint.MapleID)
        {
            this.SpawnPoint = spawnPoint;
            this.Foothold = spawnPoint.Foothold;
            this.Position = this.SpawnPoint.Position;
            this.Position.Y -= 1;
        }

        public Mob(int mapleId, Point position)
            : this(mapleId)
        {
            this.Foothold = 0; // TODO!
            this.Position = position;
            this.Position.Y -= 5;
        }

        public bool CanRespawn
        {
            get
            {
                return true; // TODO
            }
        }

        public int SpawnEffect { get; set; }

        public void AssignController()
        {
            if (this.Controller == null)
            {
                int leastControlled = int.MaxValue;
                Character newController = null;

                lock (this.Map.Characters)
                {
                    foreach (Character loopCharacter in this.Map.Characters)
                    {
                        if (loopCharacter.ControlledMobs.Count < leastControlled)
                        {
                            leastControlled = loopCharacter.ControlledMobs.Count;
                            newController = loopCharacter;
                        }
                    }
                }

                if (newController != null)
                {
                    this.IsProvoked = false;
                    newController.ControlledMobs.Add(this);
                }
            }
        }

        public void SwitchController(Character newController)
        {
            lock (this)
            {
                //Log.Warn(1);
                if (this.Controller != newController)
                {
                    //Log.Warn(2);
                    this.Controller.ControlledMobs.Remove(this);
                    //Log.Warn(3);
                    newController.ControlledMobs.Add(this);
                    //Log.Warn(4);
                }
                //Log.Warn(5);
            }
        }

        public void Move(Packet inPacket)
        {
            short movementId = inPacket.ReadShort();

            byte randomSkill = inPacket.ReadByte(); // TODO: Might be a byte!
            byte skillUnknown1 = inPacket.ReadByte();
            byte skillId = inPacket.ReadByte();
            byte skillLevel = inPacket.ReadByte();
            byte skillUnknown2 = inPacket.ReadByte();
            inPacket.ReadByte();
            inPacket.Skip(8);
            inPacket.ReadByte();
            inPacket.ReadInt();
            Point startPosition = new Point(inPacket.ReadShort(), inPacket.ReadShort());
            Movements movements = Movements.Parse(inPacket.ReadBytes());
            MobSkill skill = null;

            if (randomSkill == 1 && this.Skills.Count > 0)
            {
                skill = this.Skills.Random;
            }
            else if ((skillId >= 100 && skillId <= 200) && this.Skills.Contains(skillId, skillLevel)) // TODO: Is this necessary?
            {
                skill = this.Skills[skillId];
            }

            if (skill != null)
            {
                if (this.CurrentHP * 100 / this.MaxHP > skill.PercentageLimitHP ||
                    (this.Cooldowns.ContainsKey(skill) && this.Cooldowns[skill].AddSeconds(skill.Cooldown) >= DateTime.Now) ||
                    ((MobSkillName)skill.MapleID) == MobSkillName.Summon && this.Map.Mobs.Count >= 100)
                {
                    skill = null;
                }
            }

            if (skill != null)
            {
                skill.Cast(this);
            }

            using (Packet outPacket = new Packet(MapleServerOperationCode.MoveMonsterResponse))
            {
                outPacket.WriteInt(this.ObjectID);
                outPacket.WriteShort(movementId);
                outPacket.WriteByte(randomSkill);
                outPacket.WriteShort((short)this.CurrentMP);
                outPacket.WriteByte((byte)(skill == null ? 0 : skill.MapleID));
                outPacket.WriteByte((byte)(skill == null ? 0 : skill.Level));

                this.Controller.Client.Send(outPacket);
            }

            using (Packet outPacket = new Packet(MapleServerOperationCode.MoveMonster))
            {
                outPacket.WriteInt(this.ObjectID);
                outPacket.WriteByte(randomSkill);
                outPacket.WriteByte(skillUnknown1);
                outPacket.WriteByte(skillId);
                outPacket.WriteByte(skillLevel);
                outPacket.WriteByte(skillUnknown2);
                outPacket.WriteByte();
                outPacket.WriteShort(startPosition.X);
                outPacket.WriteShort(startPosition.Y);
                outPacket.WriteBytes(movements.ToByteArray());

                this.Controller.Map.Broadcast(this.Controller, outPacket);
            }

            foreach (Movement movement in movements) // TODO: Inheritable. Moves, has stance... AnimatedMapleMap? Fuck I hate inheritance.
            {
                if (movement is AbsoluteMovement)
                {
                    this.Position = ((AbsoluteMovement)movement).Position;
                }

                if (!(movement is EquipmentMovement))
                {
                    this.Stance = movement.NewStance;
                }
            }
        }

        public void Buff(MobStatus buff, short value, MobSkill skill)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.ApplyMonsterStatus))
            {
                outPacket.WriteInt(this.ObjectID);
                outPacket.WriteInt((int)buff);
                outPacket.WriteShort(value);
                outPacket.WriteShort(skill.MapleID);
                outPacket.WriteShort(skill.Level);
                outPacket.WriteShort();
                outPacket.WriteShort(0); // Delay
                outPacket.WriteByte(1);

                this.Map.Broadcast(outPacket);
            }

            Delay.Execute(skill.Duration * 1000, () =>
                {
                    using (Packet outPacket = new Packet(MapleServerOperationCode.CancelMonsterStatus))
                    {
                        outPacket.WriteInt(this.ObjectID);
                        outPacket.WriteInt((int)buff);
                        outPacket.WriteShort(value);
                        outPacket.WriteByte(1);

                        this.Map.Broadcast(outPacket);
                    }

                    this.Buffs.Remove(buff);
                });
        }

        public void Heal(uint hp, int range)
        {
            this.CurrentHP = Math.Min(this.MaxHP, (uint)(this.CurrentHP + hp + Application.Random.Next(-range / 2, range / 2)));

            using (Packet outPacket = new Packet(MapleServerOperationCode.DamageMonster))
            {
                outPacket.WriteInt(this.ObjectID);
                outPacket.WriteByte();
                outPacket.WriteInt((int)-hp);
                outPacket.WriteByte();
                outPacket.WriteByte();
                outPacket.WriteByte();

                this.Map.Broadcast(outPacket);
            }
        }

        public void Die()
        {
            this.Map.Mobs.Remove(this);
        }

        public bool Damage(Character attacker, uint amount)
        {
            lock (this)
            {
                uint originalAmount = amount;

                amount = Math.Min(amount, this.CurrentHP);

                if (this.Attackers.ContainsKey(attacker))
                {
                    this.Attackers[attacker] += amount;
                }
                else
                {
                    this.Attackers.Add(attacker, amount);
                }

                this.CurrentHP -= amount;

                using (Packet outPacket = new Packet(MapleServerOperationCode.ShowMonsterHP))
                {
                    outPacket.WriteInt(this.ObjectID);
                    outPacket.WriteByte((byte)((this.CurrentHP * 100) / this.MaxHP));

                    attacker.Client.Send(outPacket);
                }

                if (this.CurrentHP <= 0)
                {
                    return true;
                }

                return false;
            }
        }

        private Packet GetInternalPacket(bool requestControl, bool fadeIn)
        {
            Packet spawn = new Packet(requestControl ? MapleServerOperationCode.SpawnMonsterController : MapleServerOperationCode.SpawnMonster);

            if (requestControl)
            {
                spawn.WriteByte((byte)(this.IsProvoked ? 2 : 1));
            }

            spawn.WriteInt(this.ObjectID);
            spawn.WriteByte(5);
            spawn.WriteInt(this.MapleID);
            spawn.Skip(15);
            spawn.WriteByte(0x88);
            spawn.Skip(6);
            spawn.WriteShort(this.Position.X);
            spawn.WriteShort(this.Position.Y);
            spawn.WriteByte(this.Stance);
            spawn.WriteShort();
            spawn.WriteShort(this.Foothold);

            if (this.SpawnEffect > 0)
            {
                spawn.WriteByte((byte)this.SpawnEffect);
                spawn.WriteByte();
                spawn.WriteShort();

                if (this.SpawnEffect == 15)
                {
                    spawn.WriteByte();
                }
            }

            spawn.WriteShort((short)(fadeIn ? -2 : -1));
            spawn.WriteInt();

            return spawn;
        }

        public Packet GetCreatePacket()
        {
            return this.GetInternalPacket(false, true);
        }

        public Packet GetSpawnPacket()
        {
            return this.GetInternalPacket(false, false);
        }

        public Packet GetDestroyPacket()
        {
            Packet destroy = new Packet(MapleServerOperationCode.KillMonster);

            destroy.WriteInt(this.ObjectID);
            destroy.WriteByte(1); // TODO: 0 = dissapear, 1 = fade out, 2+ = special
            destroy.WriteByte(1); // Same.

            return destroy;
        }

        public Packet GetControlRequestPacket()
        {
            return this.GetInternalPacket(true, false);
        }

        public Packet GetControlCancelPacket()
        {
            Packet cancelControl = new Packet(MapleServerOperationCode.SpawnMonsterController);

            cancelControl.WriteByte(0);
            cancelControl.WriteInt(this.ObjectID);

            return cancelControl;
        }
    }
}
