using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

namespace Loki.Maple.Life
{
    public class MobSkills : Collection<MobSkill>
    {
        public Mob Parent { get; private set; }

        public MobSkills(Mob parent)
            : base()
        {
            this.Parent = parent;
        }

        public MobSkill Random // TODO: Skill.Chance, not just random.
        {
            get
            {
                return base[Application.Random.Next(this.Count - 1)];
            }
        }

        public new MobSkill this[int mapleId]
        {
            get
            {
                foreach (MobSkill loopMobSkill in this)
                {
                    if (loopMobSkill.MapleID == mapleId)
                    {
                        return loopMobSkill;
                    }
                }

                throw new KeyNotFoundException();
            }
        }

        public bool Contains(int mapleId, short level)
        {
            foreach (MobSkill loopSkill in this)
            {
                if (loopSkill.MapleID == mapleId && loopSkill.Level == level)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
