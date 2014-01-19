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
                        int state = rewardDatum.quest_state.Equals("start") ? 0 : 1;

                        switch ((string)rewardDatum.reward_type)
                        {
                            case "exp":
                                this[(ushort)rewardDatum.questid].ExperienceReward[state] = rewardDatum.rewardid;
                                break;

                            case "mesos":
                                this[(ushort)rewardDatum.questid].MesoReward[state] = rewardDatum.rewardid;
                                break;

                            case "fame":
                                this[(ushort)rewardDatum.questid].FameReward[state] = rewardDatum.rewardid;
                                break;

                            case "item": // TODO: Gender and Job Tracks // TODO: Job Tracks in general
                                if (state == 0)
                                {
                                    if (!this[(ushort)rewardDatum.questid].PreItemRewards.ContainsKey(rewardDatum.rewardid)) // Weird but needed? See quest 8801
                                    {
                                        this[(ushort)rewardDatum.questid].PreItemRewards.Add(rewardDatum.rewardid, (rewardDatum.quantity));
                                    }
                                }
                                else
                                {
                                    if (!this[(ushort)rewardDatum.questid].PostItemRewards.ContainsKey(rewardDatum.rewardid)) // Weird but needed? See quest 8801
                                    {
                                        this[(ushort)rewardDatum.questid].PostItemRewards.Add(rewardDatum.rewardid, (rewardDatum.quantity));
                                    }
                                }

                                break;

                            case "skill":
                                if (state == 0)
                                {
                                    this[(ushort)rewardDatum.questid].PreSkillRewards.Add(new Skill(rewardDatum.rewardid, (byte)rewardDatum.quantity, (byte)rewardDatum.master_level), (Job)rewardDatum.job);
                                }
                                else
                                {
                                    this[(ushort)rewardDatum.questid].PostSkillRewards.Add(new Skill(rewardDatum.rewardid, (byte)rewardDatum.quantity, (byte)rewardDatum.master_level), (Job)rewardDatum.job);
                                }
                                break;

                            case "pet_speed":
                                this[(ushort)rewardDatum.questid].PetSpeedReward[state] = true;
                                break;

                            case "pet_closeness":
                                this[(ushort)rewardDatum.questid].PetClosenessReward[state] = rewardDatum.rewardid;
                                break;

                            case "pet_skill":
                                this[(ushort)rewardDatum.questid].PetSkillReward[state] = rewardDatum.rewardid;
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
