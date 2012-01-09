using System;
using System.Collections.Generic;
using System.Linq;
using Loki.Data;
using Loki.Maple.Characters;
using Loki.Maple.Data;
using Loki.Net;
using Loki.Threading;

namespace Loki.Maple
{
	public class Buff
	{
		public CharacterBuffs Parent { get; set; }

		public int MapleID { get; set; }
		public byte SkillLevel { get; set; }

		public byte Type { get; set; }
		public Dictionary<PrimaryBuffStat, short> PrimaryStatups { get; set; }
		public Dictionary<SecondaryBuffStat, short> SecondaryStatups { get; set; }
		public DateTime End { get; set; }
		public int Value { get; set; } // NOTE: Needed for energy charge and possible other buffs.

		public Character Character
		{
			get
			{
				return this.Parent.Parent;
			}
		}

		public long PrimaryBuffMask
		{
			get
			{
				long mask = 0;

				foreach (KeyValuePair<PrimaryBuffStat, short> primaryStatup in this.PrimaryStatups)
				{
					mask |= (long)primaryStatup.Key;
				}

				return mask;
			}
		}

		public long SecondaryBuffMask
		{
			get
			{
				long mask = 0;
				foreach (KeyValuePair<SecondaryBuffStat, short> secondaryStatus in this.SecondaryStatups)
				{
					mask |= (long)secondaryStatus.Key;
				}

				return mask;
			}
		}

		public Buff(CharacterBuffs parent, Skill skill, int value)
		{
			this.Parent = parent;
			this.MapleID = skill.MapleID;
			this.SkillLevel = skill.CurrentLevel;
			this.Type = 1;
			this.Value = value;
			this.End = DateTime.Now.AddSeconds(skill.BuffTime);
			this.PrimaryStatups = new Dictionary<PrimaryBuffStat, short>();
			this.SecondaryStatups = new Dictionary<SecondaryBuffStat, short>();

			this.CalculateStatups(skill);

			Delay.Execute((this.End - DateTime.Now).TotalMilliseconds, () => { if (this.Parent.Contains(this)) this.Parent.Remove(this); });
		}

		public Buff(CharacterBuffs parent, dynamic buffDatum)
		{
			this.Parent = parent;
			this.MapleID = buffDatum.MapleID;
			this.SkillLevel = buffDatum.SkillLevel;
			this.Type = buffDatum.Type;
			this.Value = buffDatum.Value;
			this.End = buffDatum.End;
			this.PrimaryStatups = new Dictionary<PrimaryBuffStat, short>();
			this.SecondaryStatups = new Dictionary<SecondaryBuffStat, short>();

			if (this.Type == 1)
			{
				this.CalculateStatups(World.CachedSkills[this.MapleID][this.SkillLevel]);
			}

			Delay.Execute((this.End - DateTime.Now).TotalMilliseconds, () => this.Parent.Remove(this));
		}

        public Buff(CharacterBuffs parent, Item item, int value)
        {
            this.Parent = parent;
            this.MapleID = item.MapleID;
            this.SkillLevel = 0;
            this.Type = 1;
            this.Value = value;
            this.End = DateTime.Now.AddSeconds(item.CBuffTime);
            this.PrimaryStatups = new Dictionary<PrimaryBuffStat, short>();
            this.SecondaryStatups = new Dictionary<SecondaryBuffStat, short>();

            this.CalculateStatups(item);

            Delay.Execute((this.End - DateTime.Now).TotalMilliseconds, () => { if (this.Parent.Contains(this)) this.Parent.Remove(this); });
        }

		public void Save()
		{

			dynamic datum = new Datum("buffs");

			datum.CharacterID = this.Character.ID;
			datum.MapleID = this.MapleID;
			datum.SkillLevel = this.SkillLevel;
			datum.Type = this.Type;
			datum.Value = this.Value;
			datum.End = this.End;

			datum.Insert();
		}

		public void Apply()
		{
            if (this.SkillLevel == 0)
            {
                using (Packet outPacket = new Packet(MapleServerOperationCode.GiveBuff))
                {
                    outPacket.WriteLong(0);
                    outPacket.WriteLong(this.SecondaryBuffMask);

                    foreach (KeyValuePair<SecondaryBuffStat, short> secondaryStatup in this.SecondaryStatups)
                    {
                        outPacket.WriteShort(secondaryStatup.Value);
                        outPacket.WriteInt(-this.MapleID);
                        outPacket.WriteInt((int)(this.End - DateTime.Now).TotalMilliseconds);
                    }

                    outPacket.WriteShort();
                    outPacket.WriteShort();
                    outPacket.WriteByte();

                    this.Character.Client.Send(outPacket);
                }
                return;
            }
            else
            {
                switch (this.MapleID)
                {
                    case (int)SkillNames.Corsair.SpeedInfusion:
                    case (int)SkillNames.Buccaneer.SpeedInfusion:
                    case (int)SkillNames.ThunderBreaker.SpeedInfusion:
                        using (Packet outPacket = new Packet(MapleServerOperationCode.GiveBuff))
                        {
                            outPacket.WriteLong(this.PrimaryBuffMask);
                            outPacket.WriteLong(0);
                            outPacket.WriteShort(0);
                            outPacket.WriteInt(this.PrimaryStatups.ElementAt(0).Value);
                            outPacket.WriteInt(this.MapleID);
                            outPacket.WriteLong(0);
                            outPacket.WriteShort(0);
                            outPacket.WriteShort((short)(this.End - DateTime.Now).TotalSeconds);
                            outPacket.WriteShort(0);


                            this.Parent.Parent.Client.Send(outPacket);
                        }
                        using (Packet outPacket = new Packet(MapleServerOperationCode.GiveForeignBuff))
                        {
                            outPacket.WriteInt(this.Parent.Parent.ID);
                            outPacket.WriteLong(this.PrimaryBuffMask);
                            outPacket.WriteLong(0);
                            outPacket.WriteShort(0);
                            outPacket.WriteInt(this.PrimaryStatups.ElementAt(0).Value);
                            outPacket.WriteInt(this.MapleID);
                            outPacket.WriteLong(0);
                            outPacket.WriteShort(0);
                            outPacket.WriteShort((short)(this.End - DateTime.Now).TotalSeconds);
                            outPacket.WriteShort(0);

                            this.Parent.Parent.Map.Broadcast(outPacket);
                        }
                        break;

                    case (int)SkillNames.Marauder.EnergyCharge:
                    case (int)SkillNames.ThunderBreaker.EnergyCharge:
                        using (Packet outPacket = new Packet(MapleServerOperationCode.GiveBuff))
                        {
                            outPacket.WriteLong(this.PrimaryBuffMask);
                            outPacket.WriteLong();
                            outPacket.WriteShort();
                            outPacket.WriteInt(this.Value);
                            outPacket.WriteLong(0);
                            outPacket.WriteByte();
                            outPacket.WriteShort(5);
                            outPacket.WriteShort(1);

                            this.Parent.Parent.Client.Send(outPacket);
                        }
                        using (Packet outPacket = new Packet(MapleServerOperationCode.GiveForeignBuff))
                        {
                            outPacket.WriteInt(this.Parent.Parent.ID);
                            outPacket.WriteLong(this.PrimaryBuffMask);
                            outPacket.WriteLong();
                            outPacket.WriteShort();
                            outPacket.WriteInt(this.Value);
                            outPacket.WriteLong(0);
                            outPacket.WriteByte();
                            outPacket.WriteShort(5);
                            outPacket.WriteShort(1);

                            this.Parent.Parent.Map.Broadcast(outPacket);
                        }
                        break;

                    default:

                        using (Packet outPacket = new Packet(MapleServerOperationCode.GiveBuff))
                        {
                            outPacket.WriteLong(this.PrimaryBuffMask);
                            outPacket.WriteLong(this.SecondaryBuffMask);

                            foreach (KeyValuePair<PrimaryBuffStat, short> primaryStatup in this.PrimaryStatups)
                            {
                                outPacket.WriteShort(primaryStatup.Value);
                                outPacket.WriteInt(this.MapleID);
                                outPacket.WriteInt((int)(this.End - DateTime.Now).TotalMilliseconds);
                            }

                            foreach (KeyValuePair<SecondaryBuffStat, short> secondaryStatup in this.SecondaryStatups)
                            {
                                outPacket.WriteShort(secondaryStatup.Value);
                                outPacket.WriteInt(this.MapleID);
                                outPacket.WriteInt((int)(this.End - DateTime.Now).TotalMilliseconds);
                            }

                            outPacket.WriteShort();
                            outPacket.WriteShort();
                            outPacket.WriteByte();

                            this.Character.Client.Send(outPacket);
                        }

                        using (Packet outPacket = new Packet(MapleServerOperationCode.GiveForeignBuff))
                        {
                            outPacket.WriteInt(this.Character.ID);
                            outPacket.WriteLong(this.PrimaryBuffMask);
                            outPacket.WriteLong(this.SecondaryBuffMask);

                            foreach (KeyValuePair<PrimaryBuffStat, short> primaryStatup in this.PrimaryStatups)
                            {
                                outPacket.WriteShort(primaryStatup.Value);
                            }

                            foreach (KeyValuePair<SecondaryBuffStat, short> secondaryStatup in this.SecondaryStatups)
                            {
                                outPacket.WriteShort(secondaryStatup.Value);
                            }

                            outPacket.WriteShort();
                            outPacket.WriteByte();

                            this.Character.Map.Broadcast(outPacket);
                        }

                        break;
                }
            }
		}

        public Packet GetForeignBuffPacket()
        {
            switch (this.MapleID)
            {
                case (int)SkillNames.Corsair.SpeedInfusion:
                case (int)SkillNames.Buccaneer.SpeedInfusion:
                case (int)SkillNames.ThunderBreaker.SpeedInfusion:
                    using (Packet outPacket = new Packet(MapleServerOperationCode.GiveForeignBuff))
                    {
                        outPacket.WriteInt(this.Parent.Parent.ID);
                        outPacket.WriteLong(this.PrimaryBuffMask);
                        outPacket.WriteLong(0);
                        outPacket.WriteShort(0);
                        outPacket.WriteInt(this.PrimaryStatups.ElementAt(0).Value);
                        outPacket.WriteInt(this.MapleID);
                        outPacket.WriteLong(0);
                        outPacket.WriteShort(0);
                        outPacket.WriteShort((short)(this.End - DateTime.Now).TotalSeconds);
                        outPacket.WriteShort(0);

                        return outPacket;
                    }

                case (int)SkillNames.Marauder.EnergyCharge:
                case (int)SkillNames.ThunderBreaker.EnergyCharge:  
                    using (Packet outPacket = new Packet(MapleServerOperationCode.GiveForeignBuff))
                    {
                        outPacket.WriteInt(this.Parent.Parent.ID);
                        outPacket.WriteLong(this.PrimaryBuffMask);
                        outPacket.WriteLong();
                        outPacket.WriteShort();
                        outPacket.WriteInt(this.Value);
                        outPacket.WriteLong(0);
                        outPacket.WriteByte();
                        outPacket.WriteShort(5);
                        outPacket.WriteShort(1);

                        return outPacket;
                    }

                default:           
                    using (Packet outPacket = new Packet(MapleServerOperationCode.GiveForeignBuff))
                    {
                        outPacket.WriteInt(this.Character.ID);
                        outPacket.WriteLong(this.PrimaryBuffMask);
                        outPacket.WriteLong(this.SecondaryBuffMask);

                        foreach (KeyValuePair<PrimaryBuffStat, short> primaryStatup in this.PrimaryStatups)
                        {
                            outPacket.WriteShort(primaryStatup.Value);
                        }

                        foreach (KeyValuePair<SecondaryBuffStat, short> secondaryStatup in this.SecondaryStatups)
                        {
                            outPacket.WriteShort(secondaryStatup.Value);
                        }

                        outPacket.WriteShort();
                        outPacket.WriteByte();

                        return outPacket;
                    }
            }
        }

		public void CancelBuffEffect()
		{
			using (Packet outPacket = new Packet(MapleServerOperationCode.CancelBuff))
			{
				outPacket.WriteLong(this.PrimaryBuffMask);
				outPacket.WriteLong(this.SecondaryBuffMask);
				outPacket.WriteByte(3);

				this.Character.Client.Send(outPacket);
			}

			using (Packet outPacket = new Packet(MapleServerOperationCode.CancelForeignBuff))
			{
				outPacket.WriteInt(this.Character.ID);
				outPacket.WriteLong(this.PrimaryBuffMask);
				outPacket.WriteLong(this.SecondaryBuffMask);

				this.Character.Map.Broadcast(outPacket);
			}
		}

        public void CancelItemEffect()
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.CancelBuff))
            {
                outPacket.WriteLong(0);
                outPacket.WriteLong(this.SecondaryBuffMask);
                outPacket.WriteByte(3);

                this.Character.Client.Send(outPacket);
            }
        }

		public void CalculateStatups(Skill skill)
		{
			if (skill.WeaponAttack > 0)
			{
				this.SecondaryStatups.Add(SecondaryBuffStat.WeaponAttack, skill.WeaponAttack);
			}

			if (skill.WeaponDefense > 0)
			{
				this.SecondaryStatups.Add(SecondaryBuffStat.WeaponDefense, skill.WeaponDefense);
			}

			if (skill.MagicAttack > 0)
			{
				this.SecondaryStatups.Add(SecondaryBuffStat.MagicAttack, skill.MagicAttack);
			}

			if (skill.MagicDefense > 0)
			{
				this.SecondaryStatups.Add(SecondaryBuffStat.MagicDefense, skill.MagicAttack);
			}

			if (skill.Accuracy > 0)
			{
				this.SecondaryStatups.Add(SecondaryBuffStat.Accuracy, skill.Accuracy);
			}

			if (skill.Avoid > 0)
			{
				this.SecondaryStatups.Add(SecondaryBuffStat.Avoid, skill.Avoid);
			}

			if (skill.Speed > 0)
			{
				this.SecondaryStatups.Add(SecondaryBuffStat.Speed, skill.Speed);
			}

			if (skill.Jump > 0)
			{
				this.SecondaryStatups.Add(SecondaryBuffStat.Jump, skill.Jump);
			}

			if (skill.Morph > 0)
			{
				this.SecondaryStatups.Add(SecondaryBuffStat.Morph, (short) (skill.Morph + 100 * (int)this.Character.Gender));
			}

			switch (this.MapleID)
			{
				case (int)SkillNames.Beginner.Recovery:
				case (int)SkillNames.Noblesse.Recovery:
					this.SecondaryStatups.Add(SecondaryBuffStat.Recovery, skill.ParameterA);
					break;

				case (int)SkillNames.Beginner.EchoOfHero:
				case (int)SkillNames.Noblesse.EchoOfHero:
					this.SecondaryStatups.Add(SecondaryBuffStat.EchoOfHero, skill.ParameterA);
					break;

				case (int)SkillNames.Fighter.PowerGuard:
				case (int)SkillNames.Page.PowerGuard:
					this.SecondaryStatups.Add(SecondaryBuffStat.PowerGuard, skill.ParameterA);
					break;

				case (int)SkillNames.Spearman.HyperBody:
				case (int)SkillNames.SuperGM.HyperBody:
					this.SecondaryStatups.Add(SecondaryBuffStat.HyperBodyHP, skill.ParameterA);
					this.SecondaryStatups.Add(SecondaryBuffStat.HyperBodyHP, skill.ParameterB);
					break;

				case (int)SkillNames.WhiteKnight.BwFireCharge:
				case (int)SkillNames.WhiteKnight.BwIceCharge:
				case (int)SkillNames.WhiteKnight.BwLitCharge:
				case (int)SkillNames.WhiteKnight.SwordFireCharge:
				case (int)SkillNames.WhiteKnight.SwordIceCharge:
				case (int)SkillNames.WhiteKnight.SwordLitCharge:
				case (int)SkillNames.Paladin.BwHolyCharge:
				case (int)SkillNames.Paladin.SwordHolyCharge:
				case (int)SkillNames.DawnWarrior.SoulCharge:
				case (int)SkillNames.ThunderBreaker.LightningCharge:
					this.SecondaryStatups.Add(SecondaryBuffStat.WKCharge, skill.ParameterA);
					break;

				case (int)SkillNames.DragonKnight.DragonBlood:
					this.SecondaryStatups.Add(SecondaryBuffStat.DragonBlood, skill.ParameterA);
					break;

				case (int)SkillNames.DragonKnight.DragonRoar:
					// TODO: HP?
					break;

				case (int)SkillNames.Hero.PowerStance:
				case (int)SkillNames.Paladin.PowerStance:
				case (int)SkillNames.DarkKnight.PowerStance:
					this.SecondaryStatups.Add(SecondaryBuffStat.Stance, skill.Probability);
					break;

				case (int)SkillNames.Magician.MagicGuard:
				case (int)SkillNames.BlazeWizard.MagicGuard:
					this.SecondaryStatups.Add(SecondaryBuffStat.MagicGuard, skill.ParameterA);
					break;

				case (int)SkillNames.Cleric.Invincible:
					this.SecondaryStatups.Add(SecondaryBuffStat.Invincible, skill.ParameterA);
					break;

				case (int)SkillNames.Priest.HolySymbol:
				case (int)SkillNames.SuperGM.HolySymbol:
					this.SecondaryStatups.Add(SecondaryBuffStat.HolySymbol, skill.ParameterA);
					break;

				case (int)SkillNames.FirePoisonArchMage.Infinity:
				case (int)SkillNames.IceLightningArchMage.Infinity:
					this.SecondaryStatups.Add(SecondaryBuffStat.Infinity, skill.ParameterA);
					break;

				case (int)SkillNames.FirePoisonArchMage.ManaReflection:
				case (int)SkillNames.IceLightningArchMage.ManaReflection:
				case (int)SkillNames.Bishop.ManaReflection:
					this.SecondaryStatups.Add(SecondaryBuffStat.ManaReflection, 1);
					break;

				case (int)SkillNames.Bishop.HolyShield:
					this.SecondaryStatups.Add(SecondaryBuffStat.HolyShield, skill.ParameterA);
					break;

				case (int)SkillNames.Priest.MysticDoor:
				case (int)SkillNames.Hunter.SoulArrow:
				case (int)SkillNames.Crossbowman.SoulArrow:
				case (int)SkillNames.WindArcher.SoulArrow:
					this.SecondaryStatups.Add(SecondaryBuffStat.SoulArrow, skill.ParameterA);
					break;

				case (int)SkillNames.Ranger.Puppet:
				case (int)SkillNames.Sniper.Puppet:
				case (int)SkillNames.WindArcher.Puppet:
					this.SecondaryStatups.Add(SecondaryBuffStat.Puppet, 1);
					break;

				case (int)SkillNames.Bowmaster.Concentrate:
					this.SecondaryStatups.Add(SecondaryBuffStat.Concentrate, skill.ParameterA);
					break;

				case (int)SkillNames.Bowmaster.Hamstring:
					this.SecondaryStatups.Add(SecondaryBuffStat.Hamstring, skill.ParameterA);
					//TODO: Affect monster's speed here.
					break;

				case (int)SkillNames.Marksman.Blind:
					this.SecondaryStatups.Add(SecondaryBuffStat.Blind, skill.ParameterA);
					//TODO: Affect monster's accuracy here.
					break;

				case (int)SkillNames.Bowmaster.SharpEyes:
				case (int)SkillNames.Marksman.SharpEyes:
					this.SecondaryStatups.Add(SecondaryBuffStat.SharpEyes, (short)(skill.ParameterA << 8 | skill.ParameterB));
					break;

				case (int)SkillNames.Rogue.DarkSight:
				case (int)SkillNames.WindArcher.WindWalk:
				case (int)SkillNames.NightWalker.DarkSight:
					this.SecondaryStatups.Add(SecondaryBuffStat.DarkSight, skill.ParameterA);
					break;

				case (int)SkillNames.Hermit.MesoUp:
					this.SecondaryStatups.Add(SecondaryBuffStat.MesoUp, skill.ParameterA);
					break;

				case (int)SkillNames.Hermit.ShadowPartner:
				case (int)SkillNames.NightWalker.ShadowPartner:
					this.SecondaryStatups.Add(SecondaryBuffStat.ShadowPartner, skill.ParameterA);
					break;

				case (int)SkillNames.ChiefBandit.MesoGuard:
					this.SecondaryStatups.Add(SecondaryBuffStat.MesoGuard, skill.ParameterA);
					break;

				case (int)SkillNames.ChiefBandit.Pickpocket:
					this.SecondaryStatups.Add(SecondaryBuffStat.PickPocket, skill.ParameterA);
					break;
				case (int)SkillNames.NightLord.ShadowStars:
					this.SecondaryStatups.Add(SecondaryBuffStat.ShadowClaw, 0);
					break;

				case (int)SkillNames.SuperGM.Hide:
					// TODO: Duration?
					this.SecondaryStatups.Add(SecondaryBuffStat.DarkSight, skill.ParameterA);
					break;

				case (int)SkillNames.Crusader.ComboAttack:
				case (int)SkillNames.DawnWarrior.ComboAttack:
					this.SecondaryStatups.Add(SecondaryBuffStat.Combo, (short)(this.Value + 1));
					break;


				case (int)SkillNames.Fighter.AxeBooster:
				case (int)SkillNames.Fighter.SwordBooster:
				case (int)SkillNames.Page.BwBooster:
				case (int)SkillNames.Page.SwordBooster:
				case (int)SkillNames.Spearman.PolearmBooster:
				case (int)SkillNames.Spearman.SpearBooster:
				case (int)SkillNames.Hunter.BowBooster:
				case (int)SkillNames.Crossbowman.CrossbowBooster:
				case (int)SkillNames.Assassin.ClawBooster:
				case (int)SkillNames.Bandit.DaggerBooster:
				case (int)SkillNames.FirePoisonMage.SpellBooster:
				case (int)SkillNames.IceLightningMage.SpellBooster:
				case (int)SkillNames.Brawler.KnucklerBooster:
				case (int)SkillNames.Gunslinger.GunBooster:
				case (int)SkillNames.DawnWarrior.SwordBooster:
				case (int)SkillNames.BlazeWizard.SpellBooster:
				case (int)SkillNames.WindArcher.BowBooster:
				case (int)SkillNames.NightWalker.ClawBooster:
				case (int)SkillNames.ThunderBreaker.KnucklerBooster:
					this.SecondaryStatups.Add(SecondaryBuffStat.Booster, skill.ParameterA);
					break;

				case (int)SkillNames.Hero.MapleWarrior:
				case (int)SkillNames.Paladin.MapleWarrior:
				case (int)SkillNames.DarkKnight.MapleWarrior:
				case (int)SkillNames.FirePoisonArchMage.MapleWarrior:
				case (int)SkillNames.IceLightningArchMage.MapleWarrior:
				case (int)SkillNames.Bishop.MapleWarrior:
				case (int)SkillNames.Bowmaster.MapleWarrior:
				case (int)SkillNames.Marksman.MapleWarrior:
				case (int)SkillNames.NightLord.MapleWarrior:
				case (int)SkillNames.Shadower.MapleWarrior:
				case (int)SkillNames.Corsair.MapleWarrior:
				case (int)SkillNames.Buccaneer.MapleWarrior:
					this.SecondaryStatups.Add(SecondaryBuffStat.MapleWarrrior, skill.ParameterA);
					break;
				case (int)SkillNames.Marauder.EnergyCharge:
				case (int)SkillNames.ThunderBreaker.EnergyCharge:
					this.PrimaryStatups.Add(PrimaryBuffStat.EnergyCharge, (short)this.Value);
					break;

				case (int)SkillNames.Pirate.Dash:
				case (int)SkillNames.ThunderBreaker.Dash:
					this.SecondaryStatups.Add(SecondaryBuffStat.Speed, skill.ParameterA);
					this.SecondaryStatups.Add(SecondaryBuffStat.Jump, skill.ParameterB);
					break;

				case (int)SkillNames.Corsair.SpeedInfusion:
				case (int)SkillNames.Buccaneer.SpeedInfusion:
				case (int)SkillNames.ThunderBreaker.SpeedInfusion:
					this.PrimaryStatups.Add(PrimaryBuffStat.SpeedInfusion, skill.ParameterA);
					break;



			}
		}

        public void CalculateStatups(Item item)
        {
            if (item.CWeaponAttack > 0)
            {
                this.SecondaryStatups.Add(SecondaryBuffStat.WeaponAttack, item.CWeaponAttack);
            }

            if (item.CWeaponDefense > 0)
            {
                this.SecondaryStatups.Add(SecondaryBuffStat.WeaponDefense, item.CWeaponDefense);
            }

            if (item.CMagicAttack > 0)
            {
                this.SecondaryStatups.Add(SecondaryBuffStat.MagicAttack, item.CMagicAttack);
            }

            if (item.CMagicDefense > 0)
            {
                this.SecondaryStatups.Add(SecondaryBuffStat.MagicDefense, item.CMagicAttack);
            }

            if (item.CAccuracy > 0)
            {
                this.SecondaryStatups.Add(SecondaryBuffStat.Accuracy, item.CAccuracy);
            }

            if (item.CAvoid > 0)
            {
                this.SecondaryStatups.Add(SecondaryBuffStat.Avoid, item.CAvoid);
            }

            if (item.CSpeed > 0)
            {
                this.SecondaryStatups.Add(SecondaryBuffStat.Speed, item.CSpeed);
            }

            if (item.CJump > 0)
            {
                this.SecondaryStatups.Add(SecondaryBuffStat.Jump, item.CJump);
            }
        }
	}
}
