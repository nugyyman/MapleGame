using System;
using System.Collections.Generic;
using Loki.Data;
using Loki.IO;
using Loki.Maple.Data;
using Loki.Net;

namespace Loki.Maple.Characters
{
    public class CharacterQuests
    {
        public Character Parent { get; private set; }

        public Dictionary<ushort, Dictionary<int, short>> Started { get; private set; }
        public Dictionary<ushort, DateTime> Completed { get; private set; }

        public CharacterQuests(Character parent)
        {
            this.Parent = parent;

            this.Started = new Dictionary<ushort, Dictionary<int, short>>();
            this.Completed = new Dictionary<ushort, DateTime>();
        }

        public void Handle(Packet inPacket)
        {
            QuestAction action = (QuestAction)inPacket.ReadByte();
            Quest quest = ChannelData.Quests[inPacket.ReadUShort()];

            int npcId = -1;

            switch (action)
            {
                case QuestAction.RestoreLostItem:
                    inPacket.ReadInt();
                    int itemID = inPacket.ReadInt();
                    // TODO: Handle this

                    break;

                case QuestAction.Start:
                    npcId = inPacket.ReadInt();
                    this.Started.Add(quest.ID, new Dictionary<int, short>());

                    foreach (KeyValuePair<int, short> requiredKills in ChannelData.Quests[quest.ID].PostRequiredKills)
                    {
                        this.Started[quest.ID].Add(requiredKills.Key, 0);
                    }

                    using (Packet outPacket = new Packet(MapleServerOperationCode.ShowLog))
                    {
                        outPacket.WriteByte(1);
                        outPacket.WriteUShort(quest.ID);
                        outPacket.WriteByte(1);
                        outPacket.WriteString("");

                        this.Parent.Client.Send(outPacket);
                    }

                    this.Update(quest.ID, npcId, 8);

                    break;

                case QuestAction.Complete:
                    npcId = inPacket.ReadInt();
                    inPacket.ReadInt();

                    foreach (KeyValuePair<int, short> item in quest.PostRequiredItems)
                    {
                        this.Parent.Items.Remove(item.Key, item.Value);
                    }

                    this.Parent.Experience += quest.ExperienceReward * ChannelServer.QuestExperienceRate;
                    this.Parent.Fame += (short)quest.FameReward;
                    this.Parent.Meso += quest.MesoReward * ChannelServer.MesoRate;

                    // TODO: Skill rewards

                    foreach (KeyValuePair<int, short> item in quest.ItemRewards)
                    {
                        if (item.Value > 0)
                        {
                            this.Parent.Items.Add(new Item(item.Key, item.Value));
                        }
                        else if (item.Value < 0)
                        {
                            this.Parent.Items.Remove(item.Key, Math.Abs(item.Value));
                        }
                    }

                    if (inPacket.Remaining >= 4)
                    {
                        int itemChoiceId = inPacket.ReadInt();

                        //this.Parent.Items.Add(new Item(itemChoiceId, quest.SelectibleItemRewards[itemChoiceId]));
                    }

                    using (Packet outPacket = new Packet(MapleServerOperationCode.ShowLog))
                    {
                        outPacket.WriteByte(1);
                        outPacket.WriteUShort(quest.ID);
                        outPacket.WriteByte(2);
                        outPacket.WriteLongDateTime(DateTime.UtcNow);

                        this.Parent.Client.Send(outPacket);
                    }

                    this.Delete(quest.ID);

                    this.Completed.Add(quest.ID, DateTime.UtcNow);

                    break;

                case QuestAction.Forfeit:
                    this.Delete(quest.ID);

                    using (Packet outPacket = new Packet(MapleServerOperationCode.ShowLog))
                    {
                        outPacket.WriteByte(1);
                        outPacket.WriteUShort(quest.ID);
                        outPacket.WriteByte();
                        outPacket.WriteByte();

                        this.Parent.Client.Send(outPacket);
                    }

                    break;

                case QuestAction.ScriptStart:
                    npcId = inPacket.ReadInt();

                    //throw new NotImplementedException("Scripted quests not implemented.");
                    break;

                case QuestAction.ScriptEnd:
                    npcId = inPacket.ReadInt();

                    //throw new NotImplementedException("Scripted quests not implemented.");
                    break;
            }
        }

        public void Update(ushort questId, int npcId, byte progress)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.UpdateQuestInfo))
            {
                outPacket.WriteByte(progress);
                outPacket.WriteUShort(questId);
                outPacket.WriteInt(npcId);
                outPacket.WriteInt();

                this.Parent.Client.Send(outPacket);
            }
        }

        public void Delete(ushort questId)
        {
            if (this.Started.ContainsKey(questId))
            {
                this.Started.Remove(questId);
            }

            if (Database.Exists("quests_started", "QuestID = '{0}'", questId))
            {
                Database.Delete("quests_started", "QuestID = '{0}'", questId);
            }
        }

        public void Delete()
        {
            Database.Delete("quests_started", "CharacterID = '{0}'", this.Parent.ID);
            Database.Delete("quests_completed", "CharacterID = '{0}'", this.Parent.ID);
        }

        public void Load()
        {
            foreach (dynamic startedDatum in new Datums("quests_started").Populate("CharacterID = '{0}'", this.Parent.ID))
            {
                if (!this.Started.ContainsKey(startedDatum.QuestID))
                {
                    this.Started.Add(startedDatum.QuestID, new Dictionary<int, short>());
                }

                if (startedDatum.MobID != null && startedDatum.Killed != null)
                {
                    this.Started[startedDatum.QuestID].Add(startedDatum.MobID, startedDatum.Killed);
                }
            }

            foreach (dynamic completedDatum in new Datums("quests_completed").Populate("CharacterID = '{0}'", this.Parent.ID))
            {
                this.Completed.Add(completedDatum.QuestID, completedDatum.CompletionTime);
            }
        }

        public void Save()
        {
            foreach (KeyValuePair<ushort, Dictionary<int, short>> loopStarted in this.Started)
            {
                if (loopStarted.Value == null || loopStarted.Value.Count == 0)
                {
                    dynamic startedDatum = new Datum("quests_started");

                    startedDatum.CharacterID = this.Parent.ID;
                    startedDatum.QuestID = loopStarted.Key;

                    if (!Database.Exists("quests_started", "CharacterID = '{0}' && QuestID = '{1}'", this.Parent.ID, loopStarted.Key))
                    {
                        startedDatum.Insert();
                    }
                }
                else
                {
                    foreach (KeyValuePair<int, short> mobKills in loopStarted.Value)
                    {
                        dynamic startedDatum = new Datum("quests_started");

                        startedDatum.CharacterID = this.Parent.ID;
                        startedDatum.QuestID = loopStarted.Key;
                        startedDatum.MobID = mobKills.Key;
                        startedDatum.Killed = mobKills.Value;

                        if (Database.Exists("quests_started", "CharacterID = '{0}' && QuestID = '{1}' && MobID = '{2}'", this.Parent.ID, loopStarted.Key, mobKills.Key))
                        {
                            startedDatum.Update("CharacterID = '{0}' && QuestID = '{1}' && MobID = '{2}'", this.Parent.ID, loopStarted.Key, mobKills.Key);
                        }
                        else
                        {
                            startedDatum.Insert();
                        }
                    }
                }
            }

            foreach (KeyValuePair<ushort, DateTime> loopCompleted in this.Completed)
            {
                dynamic completedDatum = new Datum("quests_completed");

                completedDatum.CharacterID = this.Parent.ID;
                completedDatum.QuestID = loopCompleted.Key;
                completedDatum.CompletionTime = loopCompleted.Value;

                if (Database.Exists("quests_completed", "CharacterID = '{0}' && QuestID = '{1}'", this.Parent.ID, loopCompleted.Key))
                {
                    completedDatum.Update("CharacterID = '{0}' && QuestID = '{1}'", this.Parent.ID, loopCompleted.Key);
                }
                else
                {
                    completedDatum.Insert();
                }
            }
        }

        public byte[] ToByteArray()
        {
            using (ByteBuffer buffer = new ByteBuffer())
            {
                buffer.WriteByte(1);
                buffer.WriteShort((short)this.Started.Count);

                foreach (KeyValuePair<ushort, Dictionary<int, short>> questStatus in this.Started)
                {
                    buffer.WriteUShort(questStatus.Key);

                    string kills = string.Empty;

                    foreach (int kill in questStatus.Value.Values)
                    {
                        kills += kill.ToString().PadLeft(3, '\u0030');
                    }

                    buffer.WriteString(kills);
                }

                buffer.WriteShort();
                buffer.WriteByte(1);
                buffer.WriteShort((short)this.Completed.Count);

                foreach (KeyValuePair<ushort, DateTime> questStatus in this.Completed)
                {
                    buffer.WriteUShort(questStatus.Key);
                    buffer.WriteLongDateTime(questStatus.Value);
                }

                buffer.Flip();
                return buffer.GetContent();
            }
        }

        public bool CanComplete(ushort questId, bool onlyOnFinalKill = false)
        {
            foreach (KeyValuePair<int, short> requiredItem in ChannelData.Quests[questId].PostRequiredItems)
            {
                if (!this.Parent.Items.Contains(requiredItem.Key, requiredItem.Value))
                {
                    return false;
                }
            }

            foreach (ushort requiredQuest in ChannelData.Quests[questId].PostRequiredQuests)
            {
                if (!this.Completed.ContainsKey(requiredQuest))
                {
                    return false;
                }
            }

            foreach (KeyValuePair<int, short> requiredKill in ChannelData.Quests[questId].PostRequiredKills)
            {
                if (onlyOnFinalKill)
                {
                    if (this.Started[questId][requiredKill.Key] != requiredKill.Value)
                    {
                        return false;
                    }
                }
                else
                {
                    if (this.Started[questId][requiredKill.Key] < requiredKill.Value)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void NotifyComplete(ushort questId)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.ShowQuestCompletion))
            {
                outPacket.WriteUShort(questId);

                this.Parent.Client.Send(outPacket);
            }
        }
    }
}
