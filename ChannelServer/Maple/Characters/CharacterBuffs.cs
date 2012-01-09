using System;
using System.Collections;
using System.Collections.Generic;
using Loki.Data;
using Loki.Net;

namespace Loki.Maple.Characters
{
	public class CharacterBuffs : IEnumerable<Buff>
	{
		public Character Parent { get; private set; }

		private List<Buff> Buffs { get; set; }

		public CharacterBuffs(Character parent)
			: base()
		{
			this.Parent = parent;
			this.Buffs = new List<Buff>();
		}

		public void Add(Skill skill, int value)
		{
			this.Add(new Buff(this, skill, value));
		}

		public void Add(Buff buff)
		{
			foreach (Buff loopBuff in this.Buffs)
			{
				if (loopBuff.MapleID == buff.MapleID)
				{
					this.Remove(loopBuff);
					break;
				}
			}

			buff.Parent = this;

			this.Buffs.Add(buff);

			if (this.Parent.IsInitialized && buff.Type == 1)
			{
				buff.Apply();
			}
		}

		public void Remove(int mapleId)
		{
			this.Remove(this[mapleId]);
		}

		public void Remove(Buff buff)
		{
			this.Buffs.Remove(buff);

			if (this.Parent.IsInitialized)
			{
				buff.CancelBuffEffect();
			}
		}

		public void RemoveBooster()
		{
			foreach (Buff loopBuff in this.Buffs)
			{
				switch (loopBuff.MapleID)
				{
					case (int)SkillNames.Fighter.AxeBooster:
					case (int)SkillNames.Fighter.SwordBooster:
					case (int)SkillNames.Page.BwBooster: // TODO: Review names and remove abbreviations.
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
						this.Remove(loopBuff);
						break;
				}
			}
		}

		public bool Contains(Buff buff)
		{
			return this.Buffs.Contains(buff);
		}

		public bool Contains(int mapleId)
		{
			foreach (Buff loopBuff in this.Buffs)
			{
				if (loopBuff.MapleID == mapleId)
				{
					return true;
				}
			}

			return false;
		}

		public void Load()
		{
			foreach (dynamic datum in new Datums("buffs").Populate("CharacterID = '{0}'", this.Parent.ID))
			{
				if (datum.End > DateTime.Now)
				{
					this.Add(new Buff(this, datum));
				}
			}
		}

		public void Delete()
		{
			Database.Delete("buffs", "CharacterID = '{0}'", this.Parent.ID);
		}

		public void Save()
		{
			this.Delete();

			foreach (Buff loopBuff in this.Buffs)
			{
				loopBuff.Save();
			}
		}

		public Buff this[int mapleId]
		{
			get
			{
				foreach (Buff loopBuff in this.Buffs)
				{
					if (loopBuff.MapleID == mapleId)
					{
						return loopBuff;
					}
				}

				throw new KeyNotFoundException();
			}
		}

		public void CancelBuffEffect(Packet inPacket)
		{
			int mapleId = inPacket.ReadInt();

			switch (mapleId)
			{
				case (int)SkillNames.FirePoisonArchMage.BigBang:
				case (int)SkillNames.IceLightningArchMage.BigBang:
				case (int)SkillNames.Bishop.BigBang:
				case (int)SkillNames.Bowmaster.Hurricane:
				case (int)SkillNames.Marksman.PiercingArrow:
				case (int)SkillNames.Corsair.RapidFire:
				case (int)SkillNames.WindArcher.Hurricane:

					using (Packet outPacket = new Packet(MapleServerOperationCode.CancelSkillEffect))
					{
						outPacket.WriteInt(this.Parent.ID);
						outPacket.WriteInt(mapleId);

						this.Parent.Map.Broadcast(outPacket);
					}

					break;

				default:
					this.Remove(mapleId); // TODO: Shouldn't this happen even if it's one of the special skills?
					break;
			}
		}

        public void CancelItemEffect(Packet inPacket)
        {
            int mapleId = inPacket.ReadInt();
            Buff buff = new Buff(this, new Item(-mapleId), 0);
            this.Remove(buff); // TODO: Shouldn't this happen even if it's one of the special skills?
        }

		public IEnumerator<Buff> GetEnumerator()
		{
			return this.Buffs.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.Buffs).GetEnumerator();
		}
	}
}
