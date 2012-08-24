using System.Collections.Generic;
using Loki.Collections;
using Loki.Data;
using Loki.IO;

namespace Loki.Maple.Characters
{
    public class CharacterSkills : NumericalKeyedCollection<Skill>
    {
        public Character Parent { get; private set; }

        public CharacterSkills(Character parent)
            : base()
        {
            this.Parent = parent;
        }

        protected override void InsertItem(int index, Skill item)
        {
            item.Parent = this;

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            Skill item = this.GetAtIndex(index);

            item.Parent = null;

            base.RemoveItem(index);
        }

        public void Load()
        {
            foreach (dynamic datum in new Datums("skills").Populate("CharacterID = '{0}'", this.Parent.ID))
            {
                this.Add(new Skill(datum));
            }
        }

        public void Save()
        {
            foreach (Skill skill in this)
            {
                skill.Save();
            }
        }

        public void Delete()
        {
            foreach (Skill skill in this)
            {
                skill.Delete();
            }
        }

        public void DeleteJobSkills()
        {
            lock (this)
            {
                List<Skill> toRemove = new List<Skill>();

                foreach (Skill loopSkill in this)
                {
                    toRemove.Add(loopSkill);
                }

                foreach (Skill loopSkill in toRemove)
                {
                    if (loopSkill.MapleID / 1000000 != (short)this.Parent.Job / 100)
                    {
                        loopSkill.Delete();
                        loopSkill.Update();
                        base.Remove(loopSkill);
                    }
                }
            }
        }

        public byte[] ToByteArray()
        {
            using (ByteBuffer buffer = new ByteBuffer())
            {
                buffer.WriteByte();
                buffer.WriteByte(1);
                buffer.WriteShort((short)this.Count);

                List<Skill> cooldownSkills = new List<Skill>();

                foreach (Skill loopSkill in this)
                {
                    buffer.WriteInt(loopSkill.MapleID);
                    buffer.WriteInt(loopSkill.CurrentLevel);
                    buffer.WriteLong((long)ExpirationTime.DefaultTime);

                    if (loopSkill.IsFromFourthJob)
                    {
                        buffer.WriteInt(loopSkill.MaxLevel);
                    }

                    if (loopSkill.IsCoolingDown)
                    {
                        cooldownSkills.Add(loopSkill);
                    }
                }

                buffer.WriteShort((short)cooldownSkills.Count);

                foreach (Skill loopCooldown in cooldownSkills)
                {
                    buffer.WriteInt(loopCooldown.MapleID);
                    buffer.WriteShort((short)loopCooldown.RemainingCooldownSeconds);
                }

                buffer.Flip();
                return buffer.GetContent();
            }
        }

        protected override int GetKeyForItem(Skill item)
        {
            return item.MapleID;
        }
    }
}
