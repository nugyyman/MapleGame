using System.Collections.ObjectModel;
using Loki.Data;
using Loki.Maple.Life;
using System;
using System.Collections.Generic;

namespace Loki.Maple.Data
{
    public class CachedMobs : KeyedCollection<int, Mob>
    {
        public CachedMobs()
            : base()
        {
            using (Log.Load("Mobs"))
            {
                foreach (dynamic mobDatum in new Datums("mob_data").Populate())
                {
                    this.Add(new Mob(mobDatum));
                }

                foreach (dynamic mobSkillDatum in new Datums("mob_skills").Populate())
                {
                    if (this.Contains(mobSkillDatum.mobid))
                    {
                        this[mobSkillDatum.mobid].Skills.Add(new MobSkill(mobSkillDatum));
                    }
                }

                foreach (dynamic mobSkillDatum in new Datums("skill_mob_data").Populate())
                {
                    foreach (Mob loopMob in this)
                    {
                        foreach (MobSkill loopSkill in loopMob.Skills)
                        {
                            if (loopSkill.MapleID == mobSkillDatum.skillid && loopSkill.Level == mobSkillDatum.skill_level)
                            {
                                loopSkill.Load(mobSkillDatum);
                            }
                        }
                    }
                }

                MobSkill.Summons = new Dictionary<short, List<int>>();

                foreach (dynamic mobSummonDatum in new Datums("skill_mob_summons").Populate())
                {
                    if (!MobSkill.Summons.ContainsKey(mobSummonDatum.level))
                    {
                        MobSkill.Summons.Add(mobSummonDatum.level, new List<int>());
                    }

                    MobSkill.Summons[mobSummonDatum.level].Add(mobSummonDatum.mobid);
                }

                foreach (dynamic mobSummonDatum in new Datums("mob_summons").Populate())
                {
                    if (this.Contains(mobSummonDatum.mobid))
                    {
                        this[mobSummonDatum.mobid].DeathSummons.Add(mobSummonDatum.summonid);
                    }
                }
            }

            using (Log.Load("Loots"))
            {
                foreach (dynamic lootDatum in new Datums("drop_data").Populate())
                {
                    int dropperId = lootDatum.dropperid;

                    if (this.Contains(dropperId))
                    {
                        this[dropperId].Loots.Add(new Loot(lootDatum));
                    }
                }
            }
        }

        protected override int GetKeyForItem(Mob item)
        {
            return item.MapleID;
        }
    }
}
