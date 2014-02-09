using System.Collections.Generic;
using Loki.Data;
using Loki.Maple.Characters;
using Loki.Maple.Data;
using Loki.Maple.Maps;
using Loki.Maple.Shops;
using Loki.Net;

namespace Loki.Maple.Life
{
    public class Npc : LifeObject, ISpawnable
    {
        public static string Format(string value)
        {
            return value.ClearFormatters().Replace("	", "");
        }

        public void Act(Packet inPacket)
        {
            int length = inPacket.Remaining;

            using (Packet outPacket = new Packet(MapleServerOperationCode.UpdateNpc))
            {
                outPacket.WriteInt(this.ID);

                if (length == 6)
                {
                    outPacket.WriteBytes(inPacket.ReadBytes(length));
                }
                else if (length > 6)
                {
                    outPacket.WriteBytes(inPacket.ReadBytes(length - 5));
                }

                this.Controller.Map.Broadcast(outPacket);
            }
        }

        public Character Controller { get; set; }

        public Dictionary<Character, NpcEventHandler> Callbacks = new Dictionary<Character, NpcEventHandler>();
        public Dictionary<Character, NpcTextEventHandler> TextCallbacks = new Dictionary<Character, NpcTextEventHandler>();
        public Dictionary<Character, int[]> StyleSelectionHelpers = new Dictionary<Character, int[]>();

        public Shop Shop { get; set; }
        public int StorageCost { get; set; }

        public Npc(dynamic lifeDatum) : base((Datum)lifeDatum) { }

        private Packet GetInternalPacket(bool requestControl)
        {
            Packet spawn = new Packet(requestControl ? MapleServerOperationCode.RequestNpcController : MapleServerOperationCode.SpawnNpc);

            if (requestControl)
            {
                spawn.WriteByte(1);
            }

            spawn.WriteInt(this.ID);
            spawn.WriteInt(this.MapleID);
            spawn.WriteShort(this.Position.X);
            spawn.WriteShort(this.Position.Y);
            spawn.WriteBool(!this.FacesLeft);
            spawn.WriteShort(this.Foothold);
            spawn.WriteShort(this.MinimumClickX);
            spawn.WriteShort(this.MaximumClickX);
            spawn.WriteBool(this.ShowOnMinimap);

            return spawn;
        }

        public Packet GetCreatePacket()
        {
            return this.GetSpawnPacket();
        }

        public Packet GetSpawnPacket()
        {
            return this.GetInternalPacket(false);
        }

        public Packet GetDestroyPacket()
        {
            Packet destroy = new Packet(MapleServerOperationCode.RemoveNpc);

            destroy.WriteInt(this.ObjectID);

            return destroy;
        }

        public bool ShowOnMinimap
        {
            get
            {
                return true; // TODO.
            }
        }

        public Packet GetControlRequestPacket()
        {
            return this.GetInternalPacket(true);
        }

        public virtual void Converse(Character talker, NpcEventArgs args)
        {
            if (Shop != null)
            {
                Shop.Show(talker);
            }
            else if (this.StorageCost > 0)
            {
                talker.Storage.Open(this);
            }
            else
            {
                ShowOkDialog(talker, ". . .");

                Log.Warn("'{0}' attempted to converse with unimplemented NPC {1}.", talker.Name, this.MapleID);
            }
        }

        public void HandleResult(Character talker, Packet inPacket)
        {
            NpcMessageType lastMessageType = (NpcMessageType)inPacket.ReadByte();
            byte action = inPacket.ReadByte();
            int selection = -1;

            byte endTalkByte;

            switch (lastMessageType)
            {
                case NpcMessageType.RequestText:
                case NpcMessageType.RequestNumber:
                case NpcMessageType.RequestStyle:
                case NpcMessageType.Choice:
                    endTalkByte = 0;
                    break;

                default:
                    endTalkByte = byte.MaxValue;
                    break;
            }

            if (action != endTalkByte)
            {
                if (lastMessageType == NpcMessageType.RequestText)
                {
                    string text = inPacket.ReadString();

                    foreach (KeyValuePair<Character, NpcTextEventHandler> loopPair in this.TextCallbacks)
                    {
                        if (loopPair.Key == talker)
                        {
                            loopPair.Value(talker, new NpcTextEventArgs(action, selection, text));
                            break;
                        }
                    }
                }
                else
                {
                    if (inPacket.Remaining >= 4)
                    {
                        selection = inPacket.ReadInt();
                    }
                    else if (inPacket.Remaining > 0)
                    {
                        selection = inPacket.ReadByte();
                    }

                    if (lastMessageType == NpcMessageType.RequestStyle)
                    {
                        selection = this.StyleSelectionHelpers[talker][selection];
                    }

                    foreach (KeyValuePair<Character, NpcEventHandler> loopPair in this.Callbacks)
                    {
                        if (loopPair.Key == talker)
                        {
                            loopPair.Value(talker, new NpcEventArgs(action, selection));
                            break;
                        }
                    }
                }
            }
            else
            {
                talker.LastNpc = null;
            }
        }

        private void SendDialog(Character talker, string text, NpcMessageType messageType, byte speaker, int speakerNpc, params byte[] footer)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.NpcTalk))
            {
                outPacket.WriteByte(4); // UNK
                outPacket.WriteInt(this.MapleID);
                outPacket.WriteByte((byte)messageType);
                outPacket.WriteByte(speaker);

                if(speaker > 3)
                    outPacket.WriteInt(speakerNpc);

                outPacket.WriteString(Npc.Format(text));
                outPacket.WriteBytes(footer);

                talker.Client.Send(outPacket);
            }
        }

        public void ShowOkDialog(Character talker, string text, NpcEventHandler handler = null, byte speaker = 0, int speakerNpc = 0)
        {
            this.SendDialog(talker, text, NpcMessageType.Standard, speaker, speakerNpc, 0, 0);

            if (handler != null)
            {
                if (this.Callbacks.ContainsKey(talker))
                {
                    this.Callbacks[talker] = handler;
                }
                else
                {
                    this.Callbacks.Add(talker, handler);
                }
            }
            else if (this.Callbacks.ContainsKey(talker))
            {
                this.Callbacks.Remove(talker);
            }
        }

        public void ShowYesNoDialog(Character talker, string text, NpcEventHandler yesHandler, NpcEventHandler noHandler = null, byte speaker = 0, int speakerNpc = 0)
        {
            this.SendDialog(talker, text, NpcMessageType.YesNo, speaker, speakerNpc);

            NpcEventHandler handler = new NpcEventHandler(delegate(Character _talker, NpcEventArgs args)
            {
                if (args.Action == 0)
                {
                    if (noHandler != null)
                    {
                        noHandler(_talker, args);
                    }
                }
                else if (args.Action == 1)
                {
                    yesHandler(_talker, args);
                }
            });

            if (this.Callbacks.ContainsKey(talker))
            {
                this.Callbacks[talker] = handler;
            }
            else
            {
                this.Callbacks.Add(talker, handler);
            }
        }

        public void ShowNextDialog(Character talker, string text, NpcEventHandler nextHandler, byte speaker = 0, int speakerNpc = 0)
        {
            this.SendDialog(talker, text, NpcMessageType.Standard, speaker, speakerNpc, 0, 1);

            if (this.Callbacks.ContainsKey(talker))
            {
                this.Callbacks[talker] = nextHandler;
            }
            else
            {
                this.Callbacks.Add(talker, nextHandler);
            }
        }

        public void ShowNextPreviousDialog(Character talker, string text, NpcEventHandler nextHandler, NpcEventHandler previousHandler, byte speaker = 0, int speakerNpc = 0)
        {
            this.SendDialog(talker, text, NpcMessageType.Standard, speaker, speakerNpc, 1, 1);

            NpcEventHandler handler = new NpcEventHandler(delegate(Character _talker, NpcEventArgs args)
            {
                if (args.Action == 0)
                {
                    previousHandler(_talker, args);
                }
                else if (args.Action == 1)
                {
                    nextHandler(_talker, args);
                }
            });

            if (this.Callbacks.ContainsKey(talker))
            {
                this.Callbacks[talker] = handler;
            }
            else
            {
                this.Callbacks.Add(talker, handler);
            }
        }

        public void ShowChoiceDialog(Character talker, string text, NpcEventHandler handler, byte speaker = 0, int speakerNpc = 0, params string[] choices)
        {
            text += "#b\r\n";

            for (int i = 0; i < choices.Length; i++)
            {
                text += string.Format("#L{0}#{1}#l\r\n", i, choices[i]);
            }

            this.SendDialog(talker, text, NpcMessageType.Choice, speaker, speakerNpc);

            if (this.Callbacks.ContainsKey(talker))
            {
                this.Callbacks[talker] = handler;
            }
            else
            {
                this.Callbacks.Add(talker, handler);
            }
        }

        public void ShowAcceptDeclineDialog(Character talker, string text, NpcEventHandler acceptHandler, NpcEventHandler declineHandler, byte speaker = 0, int speakerNpc = 0)
        {
            this.SendDialog(talker, text, NpcMessageType.AcceptDecline, speaker, speakerNpc);

            NpcEventHandler handler = new NpcEventHandler(delegate(Character _talker, NpcEventArgs args)
            {
                if (args.Action == 0)
                {
                    declineHandler(_talker, args);
                }
                else if (args.Action == 1)
                {
                    acceptHandler(_talker, args);
                }
            });

            if (this.Callbacks.ContainsKey(talker))
            {
                this.Callbacks[talker] = handler;
            }
            else
            {
                this.Callbacks.Add(talker, handler);
            }
        }

        public void ShowTextRequestDialog(Character talker, string text, NpcTextEventHandler handler, string defaultText = "", byte speaker = 0, int speakerNpc = 0)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.NpcTalk))
            {
                outPacket.WriteByte(4); // UNK
                outPacket.WriteInt(this.MapleID);
                outPacket.WriteByte((byte)NpcMessageType.RequestText);
                outPacket.WriteByte(speaker);

                if (speaker > 3)
                    outPacket.WriteInt(speakerNpc);

                outPacket.WriteString(Npc.Format(text));
                outPacket.WriteString(defaultText);
                outPacket.WriteInt();

                talker.Client.Send(outPacket);
            }

            if (this.TextCallbacks.ContainsKey(talker))
            {
                this.TextCallbacks[talker] = handler;
            }
            else
            {
                this.TextCallbacks.Add(talker, handler);
            }
        }

        public void ShowNumberRequestDialog(Character talker, string text, NpcEventHandler handler, int defaultValue, int minimum, int maximum, byte speaker = 0, int speakerNpc = 0)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.NpcTalk))
            {
                outPacket.WriteByte(4); // UNK
                outPacket.WriteInt(this.MapleID);
                outPacket.WriteByte((byte)NpcMessageType.RequestNumber);
                outPacket.WriteByte(speaker);

                if (speaker > 3)
                    outPacket.WriteInt(speakerNpc);

                outPacket.WriteString(Npc.Format(text));
                outPacket.WriteInt(defaultValue);
                outPacket.WriteInt(minimum);
                outPacket.WriteInt(maximum);

                talker.Client.Send(outPacket);
            }

            if (this.Callbacks.ContainsKey(talker))
            {
                this.Callbacks[talker] = handler;
            }
            else
            {
                this.Callbacks.Add(talker, handler);
            }
        }

        public void ShowStyleRequestDialog(Character talker, string text, NpcEventHandler handler, params int[] styleChoices)
        {
            List<int> validStyles = new List<int>();

            foreach (int loopStyle in styleChoices)
            {
                if (ChannelData.AvailableStyles.Skins.Contains((byte)loopStyle) ||
                    ChannelData.AvailableStyles.MaleHairs.Contains(loopStyle) ||
                    ChannelData.AvailableStyles.FemaleHairs.Contains(loopStyle) ||
                    ChannelData.AvailableStyles.MaleFaces.Contains(loopStyle) ||
                    ChannelData.AvailableStyles.FemaleFaces.Contains(loopStyle))
                {
                    validStyles.Add(loopStyle);
                }
            }

            using (Packet outPacket = new Packet(MapleServerOperationCode.NpcTalk))
            {
                outPacket.WriteByte(4); // UNK
                outPacket.WriteInt(this.MapleID);
                outPacket.WriteByte((byte)NpcMessageType.RequestStyle);
                outPacket.WriteByte(); // speaker
                outPacket.WriteString(Npc.Format(text));

                outPacket.WriteByte((byte)validStyles.Count);

                foreach (int loopStyle in validStyles)
                {
                    outPacket.WriteInt(loopStyle);
                }

                talker.Client.Send(outPacket);
            }

            if (this.StyleSelectionHelpers.ContainsKey(talker))
            {
                this.StyleSelectionHelpers[talker] = validStyles.ToArray();
            }
            else
            {
                this.StyleSelectionHelpers.Add(talker, validStyles.ToArray());
            }

            if (this.Callbacks.ContainsKey(talker))
            {
                this.Callbacks[talker] = handler;
            }
            else
            {
                this.Callbacks.Add(talker, handler);
            }
        }

        public Packet GetControlCancelPacket()
        {
            Packet cancelControl = new Packet(MapleServerOperationCode.RequestNpcController);

            cancelControl.WriteByte(0);
            cancelControl.WriteInt(this.ObjectID);

            return cancelControl;
        }

        public void AssignController()
        {
            if (this.Controller == null)
            {
                int leastControlled = int.MaxValue;
                Character newController = null;

                lock (this.Map.Characters)
                {
                    foreach (Character loopCharacter in this.Map.Characters)
                    {
                        if (loopCharacter.ControlledNpcs.Count < leastControlled)
                        {
                            leastControlled = loopCharacter.ControlledNpcs.Count;
                            newController = loopCharacter;
                        }
                    }
                }

                if (newController != null)
                {
                    newController.ControlledNpcs.Add(this);
                }
            }
        }
    }
}
