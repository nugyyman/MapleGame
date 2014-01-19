using System.Collections.Generic;
using Loki.Maple.Data;

namespace Loki.Maple
{
    public class Quest
    {
        public ushort ID { get; private set; }
        public ushort NextQuestID { get; private set; }
        public sbyte Area { get; private set; }
        public byte MinimumLevel { get; private set; }
        public byte MaximumLevel { get; private set; }
        public short PetCloseness { get; private set; }
        public sbyte TamingMobLevel { get; private set; }
        public int RepeatWait { get; private set; }
        public short Fame { get; private set; }
        public int TimeLimit { get; private set; }
        public bool AutoStart { get; private set; }

        public Quest NextQuest
        {
            get
            {
                return ChannelData.Quests[this.NextQuestID];
            }
        }

        public List<ushort> PreRequiredQuests { get; private set; }
        public List<ushort> PostRequiredQuests { get; private set; }
        public Dictionary<int, short> PreRequiredItems { get; private set; }
        public Dictionary<int, short> PostRequiredItems { get; private set; }
        public Dictionary<int, short> PostRequiredKills { get; private set; }
        public List<Job> ValidJobs { get; private set; }

        // Rewards (Start, End)
        public int[] ExperienceReward { get; set; }
        public int[] MesoReward { get; set; }
        public int[] PetClosenessReward { get; set; }
        public bool[] PetSpeedReward { get; set; }
        public int[] FameReward { get; set; }
        public int[] PetSkillReward { get; set; }
        public Dictionary<int, short> PreItemRewards { get; private set; }
        public Dictionary<int, short> PostItemRewards { get; private set; }
        public Dictionary<Skill, Job> PreSkillRewards { get; set; }
        public Dictionary<Skill, Job> PostSkillRewards { get; set; }



        public Quest(dynamic questDatum)
        {
            this.ID = questDatum.questid;
            this.NextQuestID = questDatum.next_quest;
            this.Area = (sbyte)questDatum.quest_area;
            this.MinimumLevel = questDatum.min_level;
            this.MaximumLevel = questDatum.max_level;
            this.PetCloseness = questDatum.pet_closeness;
            this.TamingMobLevel = questDatum.taming_mob_level;
            this.RepeatWait = questDatum.repeat_wait;
            this.Fame = questDatum.fame;
            this.TimeLimit = questDatum.time_limit;
            this.AutoStart = questDatum.flags.Contains("auto_start");

            this.PreRequiredQuests = new List<ushort>();
            this.PostRequiredQuests = new List<ushort>();
            this.PreRequiredItems = new Dictionary<int, short>();
            this.PostRequiredItems = new Dictionary<int, short>();
            this.PostRequiredKills = new Dictionary<int, short>();

            this.ExperienceReward = new int[2];
            this.MesoReward = new int[2];
            this.PetClosenessReward = new int[2];
            this.PetSpeedReward = new bool[2];
            this.FameReward = new int[2];
            this.PetSkillReward = new int[2];

            this.PreItemRewards = new Dictionary<int, short>();
            this.PostItemRewards = new Dictionary<int, short>();
            this.PreSkillRewards = new Dictionary<Skill, Job>();
            this.PostSkillRewards = new Dictionary<Skill, Job>();

            this.ValidJobs = new List<Job>();
        }
    }
}
