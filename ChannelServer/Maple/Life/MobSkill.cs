using System;
using System.Collections.Generic;
using Loki.Maple.Characters;

namespace Loki.Maple.Life
{
    public class MobSkill
    {
        public static Dictionary<short, List<int>> Summons { get; set; }

        public byte MapleID { get; private set; }
        public byte Level { get; private set; }
        public short EffectDelay { get; private set; }

        public int Duration { get; private set; }
        public short MpCost { get; private set; }
        public int ParameterA { get; private set; }
        public int ParameterB { get; private set; }
        public short Chance { get; private set; }
        public short TargetCount { get; private set; }
        public Point LT { get; private set; }
        public Point RB { get; private set; }
        public int Cooldown { get; private set; }
        public short PercentageLimitHP { get; private set; }
        public short SummonLimit { get; private set; }
        public short SummonEffect { get; private set; }

        public MobSkill(dynamic mobSkillDatum)
            : base()
        {
            this.MapleID = (byte)mobSkillDatum.skillid;
            this.Level = (byte)mobSkillDatum.skill_level;
            //this.EffectDelay = (short)mobSkillDatum.effect_delay;
        }

        public void Load(dynamic mobSkillDatum)
        {
            this.Duration = mobSkillDatum.buff_time;
            this.MpCost = (short)mobSkillDatum.mp_cost;
            this.ParameterA = (int)mobSkillDatum.x_property;
            this.ParameterB = (int)mobSkillDatum.y_property;
            this.Chance = (short)mobSkillDatum.chance;
            this.TargetCount = (short)mobSkillDatum.target_count;
            this.Cooldown = (int)mobSkillDatum.cooldown;
            this.LT = new Point((int)mobSkillDatum.ltx, (int)mobSkillDatum.lty);
            this.RB = new Point((int)mobSkillDatum.rbx, (int)mobSkillDatum.rby);
            this.PercentageLimitHP = (short)mobSkillDatum.hp_limit_percentage;
            this.SummonLimit = (short)mobSkillDatum.summon_limit;
            this.SummonEffect = (short)mobSkillDatum.summon_effect;
        }

        public void Cast(Mob caster)
        {
            MobStatus status = MobStatus.Null;
            CharacterDisease disease = CharacterDisease.Null;
            bool heal = false;
            bool banish = false;
            bool dispel = false;

            switch ((MobSkillName)this.MapleID)
            {
                case MobSkillName.WeaponAttackUp:
                case MobSkillName.WeaponAttackUpAreaOfEffect:
                case MobSkillName.WeaponAttackUpMonsterCarnival:
                    status = MobStatus.WeaponAttackUp;
                    break;

                case MobSkillName.MagicAttackUp:
                case MobSkillName.MagicAttackUpAreaOfEffect:
                case MobSkillName.MagicAttackUpMonsterCarnival:
                    status = MobStatus.MagicAttackUp;
                    break;

                case MobSkillName.WeaponDefenseUp:
                case MobSkillName.WeaponDefenseUpAreaOfEffect:
                case MobSkillName.WeaponDefenseUpMonsterCarnival:
                    status = MobStatus.WeaponDefenseUp;
                    break;

                case MobSkillName.MagicDefenseUp:
                case MobSkillName.MagicDefenseUpAreaOfEffect:
                case MobSkillName.MagicDefenseUpMonsterCarnival:
                    status = MobStatus.MagicDefenseUp;
                    break;

                case MobSkillName.HealAreaOfEffect:
                    heal = true;
                    break;

                case MobSkillName.Seal:
                    disease = CharacterDisease.Sealed;
                    break;

                case MobSkillName.Darkness:
                    disease = CharacterDisease.Darkness;
                    break;

                case MobSkillName.Weakness:
                    disease = CharacterDisease.Weaken;
                    break;

                case MobSkillName.Stun:
                    disease = CharacterDisease.Stun;
                    break;

                case MobSkillName.Curse:
                    // TODO: Curse.
                    break;

                case MobSkillName.Poison:
                    disease = CharacterDisease.Poison;
                    break;

                case MobSkillName.Slow:
                    disease = CharacterDisease.Slow;
                    break;

                case MobSkillName.Dispel:
                    dispel = true;
                    break;

                case MobSkillName.Seduce:
                    disease = CharacterDisease.Seduce;
                    break;

                case MobSkillName.SendToTown:
                    // TODO: Send to town.
                    break;

                case MobSkillName.PoisonMist:
                    // TODO: Spawn poison mist.
                    break;

                case MobSkillName.Confuse:
                    disease = CharacterDisease.Confuse;
                    break;

                case MobSkillName.Zombify:
                    // TODO: Zombify.
                    break;

                case MobSkillName.WeaponImmunity:
                    status = MobStatus.WeaponImmunity;
                    break;

                case MobSkillName.MagicImmunity:
                    status = MobStatus.MagicImmunity;
                    break;

                case MobSkillName.WeaponDamageReflect:
                case MobSkillName.MagicDamageReflect:
                case MobSkillName.AnyDamageReflect:
                    // TODO: Reflect.
                    break;

                case MobSkillName.AccuracyUpMonsterCarnival:
                case MobSkillName.AvoidabilityUpMonsterCarnival:
                case MobSkillName.SpeedUpMonsterCarnival:
                    // TODO: Monster carnival buffs.
                    break;

                case MobSkillName.Summon:

                    foreach (int mobId in MobSkill.Summons[this.Level])
                    {
                        Mob summon = new Mob(mobId);
                        summon.Position = caster.Position;

                        switch (mobId)
                        {
                            case 8500003: // Papulatus bomb (High)
                                summon.Foothold = (short)Math.Ceiling(Application.Random.NextDouble() * 19.0);
                                summon.Position.Y -= 590;
                                break;

                            case 8500004: // Papulatus bomb (Low)
                                summon.Position.X += (short)(Application.Random.Next(1000) - 500);

                                if (summon.Position.Y != -590)
                                {
                                    summon.Position.Y = caster.Position.Y;
                                }

                                break;

                            case 8510100: //Pianus bomb

                                if (Math.Ceiling(Application.Random.NextDouble() * 5) == 1)
                                {
                                    summon.Position.Y = 78;
                                    summon.Position.X = (short)(Application.Random.Next(5) + (Application.Random.Next(2) == 1 ? 180 : 0));
                                }
                                else
                                {
                                    summon.Position.X += (short)(Application.Random.Next(1000) - 500);
                                }

                                break;
                        }

                        switch (caster.Map.MapleID)
                        {
                            case 220080001: // Papulatus' map

                                if (summon.Position.X < -890)
                                {
                                    summon.Position.X = (short)(Math.Ceiling(Application.Random.NextDouble() * 150) - 890);
                                }
                                else if (summon.Position.X > 230)
                                {
                                    summon.Position.X = (short)(230 - Math.Ceiling(Application.Random.NextDouble() * 150));
                                }

                                break;

                            case 230040420: // Pianus' map

                                if (summon.Position.X < -239)
                                {
                                    summon.Position.X = (short)(Math.Ceiling(Application.Random.NextDouble() * 150) - 239);
                                }
                                else if (summon.Position.X > 371)
                                {
                                    summon.Position.X = (short)(371 - Math.Ceiling(Application.Random.NextDouble() * 150));
                                }

                                break;
                        }

                        summon.SpawnEffect = this.SummonEffect;
                        caster.Map.Mobs.Add(summon);
                    }

                    break;
            }

            foreach (Mob affectedMob in this.GetAffectedMobs(caster))
            {
                if (heal)
                {
                    affectedMob.Heal((uint)this.ParameterA, this.ParameterB);
                }

                if (status != MobStatus.Null && !affectedMob.Buffs.Contains(status))
                {
                    affectedMob.Buff(status, (short)this.ParameterA, this);
                }
            }

            foreach (Character affectedCharacter in this.GetAffectedCharacters(caster))
            {
                if (dispel)
                {
                    //affectedCharacter.Dispel();
                }

                if (banish)
                {
                    affectedCharacter.ChangeMap(affectedCharacter.Map.ReturnMapID);
                }

                if (disease != CharacterDisease.Null)
                {
                    //affectedCharacter.Debuff(disease);
                }
            }

            caster.CurrentMP -= (uint)this.MpCost;

            if (caster.Cooldowns.ContainsKey(this))
            {
                caster.Cooldowns[this] = DateTime.Now;
            }
            else
            {
                caster.Cooldowns.Add(this, DateTime.Now);
            }
        }

        private IEnumerable<Character> GetAffectedCharacters(Mob caster)
        {
            Rectangle boundingBox = new Rectangle(this.LT + caster.Position, this.RB + caster.Position);

            foreach (Character character in caster.Map.Characters)
            {
                if (character.Position.IsInRectangle(boundingBox))
                {
                    yield return character;
                }
            }
        }

        private IEnumerable<Mob> GetAffectedMobs(Mob caster)
        {
            Rectangle boundingBox = new Rectangle(this.LT + caster.Position, this.RB + caster.Position);

            foreach (Mob mob in caster.Map.Mobs)
            {
                if (mob.Position.IsInRectangle(boundingBox))
                {
                    yield return mob;
                }
            }
        }
    }
}
