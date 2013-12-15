using System.Collections.Generic;
using Loki.Data;
using Loki.Collections;
using System.Collections.ObjectModel;

namespace Loki.Maple.Data
{
    public class QuestData : KeyedCollection<ushort, Quest>
    {
        protected override ushort GetKeyForItem(Quest item)
        {
            return item.ID;
        }

        public QuestData()
            : base()
        {
            using (Log.Load("Quests"))
            {
                foreach (dynamic questDatum in new Datums("quest_data").Populate())
                {
                    this.Add(new Quest(questDatum));
                }

                foreach (dynamic requirementDatum in new Datums("quest_requests").Populate())
                {
                    if (this.Contains((ushort)requirementDatum.questid))
                    {
                        string state = requirementDatum.quest_state;

                        switch ((string)requirementDatum.request_type)
                        {
                            case "mob":
                                this[(ushort)requirementDatum.questid].PostRequiredKills.Add(requirementDatum.objectid, requirementDatum.quantity);
                                break;

                            case "item":
                                switch (state)
                                {
                                    case "start":
                                        this[(ushort)requirementDatum.questid].PreRequiredItems.Add(requirementDatum.objectid, requirementDatum.quantity);
                                        break;

                                    case "end":
                                        this[(ushort)requirementDatum.questid].PostRequiredItems.Add(requirementDatum.objectid, requirementDatum.quantity);
                                        break;
                                }

                                break;

                            case "quest":
                                switch (state)
                                {
                                    case "start":
                                        this[(ushort)requirementDatum.questid].PreRequiredQuests.Add((ushort)requirementDatum.objectid);
                                        break;

                                    case "end":
                                        this[(ushort)requirementDatum.questid].PostRequiredQuests.Add((ushort)requirementDatum.objectid);
                                        break;
                                }

                                break;
                        }
                    }
                }

                foreach (dynamic rewardDatum in new Datums("quest_rewards").Populate())
                {
                    if (this.Contains((ushort)rewardDatum.questid))
                    {
                        string state = rewardDatum.quest_state;

                        switch ((string)rewardDatum.reward_type)
                        {
                            case "exp":
                                this[(ushort)rewardDatum.questid].ExperienceReward = rewardDatum.rewardid;
                                break;

                            case "mesos":
                                this[(ushort)rewardDatum.questid].MesoReward = rewardDatum.rewardid;
                                break;

                            case "fame":
                                this[(ushort)rewardDatum.questid].FameReward = rewardDatum.rewardid;
                                break;

                            case "item": // TODO: Gender and Job Tracks // TODO: Job Tracks in general
                                if (!this[(ushort)rewardDatum.questid].ItemRewards.ContainsKey(rewardDatum.rewardid)) // Weird but needed? See quest 8801
                                {
                                    this[(ushort)rewardDatum.questid].ItemRewards.Add(rewardDatum.rewardid, (rewardDatum.quantity));
                                }

                                break;

                            case "skill":
                                this[(ushort)rewardDatum.questid].SkillRewards.Add(new Skill(rewardDatum.rewardid, (byte)rewardDatum.quantity, (byte)rewardDatum.master_level), (Job)rewardDatum.job);
                                break;

                            case "pet_speed":
                                this[(ushort)rewardDatum.questid].PetSpeedReward = true;
                                break;

                            case "pet_closeness":
                                this[(ushort)rewardDatum.questid].PetClosenessReward = rewardDatum.rewardid;
                                break;

                            case "pet_skill":
                                this[(ushort)rewardDatum.questid].PetSkillReward = rewardDatum.rewardid;
                                break;
                        }
                    }
                }

                foreach (dynamic validJobDatum in new Datums("quest_required_jobs").Populate())
                {
                    if (this.Contains(validJobDatum.questid))
                    {
                        this[validJobDatum.questid].ValidJobs.Add((Job)validJobDatum.valid_jobid);
                    }
                }
            }
        }
    }
}
