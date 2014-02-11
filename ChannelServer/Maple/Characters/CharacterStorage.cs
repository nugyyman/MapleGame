using System;
using System.Collections.Generic;
using Loki.Data;
using Loki.Net;
using Loki.Maple.Life;

namespace Loki.Maple.Characters
{
    public class CharacterStorage : List<Item>
    {
        public Character Parent { get; private set; }
        public byte Slots { get; set; }
        public int Meso { get; set; }
        private Npc Npc { get; set; }

        public CharacterStorage(Character parent)
            : base()
        {
            this.Parent = parent;
        }

        public void Load()
        {
            if (!Database.Exists("storages", "AccountID = '{0}'", this.Parent.AccountID))
            {
                dynamic datum = new Datum("storages");

                datum.AccountID = this.Parent.AccountID;
                datum.Insert();
            }

            foreach (dynamic datum in new Datums("storages").Populate("AccountID = '{0}'", this.Parent.AccountID))
            {
                this.Slots = datum.Slots;
                this.Meso = datum.Meso;
            }

            this.Clear();
            foreach (dynamic datum in new Datums("storage_items").Populate("AccountID = '{0}'", this.Parent.AccountID))
            {
                this.Add(new Item(datum));
            }
        }

        public void Delete()
        {
            Database.Delete("storages", "AccountID = '{0}'", this.Parent.AccountID);
            Database.Delete("storage_items", "AccountID = '{0}'", this.Parent.AccountID);
        }

        public void Save()
        {
            dynamic datum = new Datum("storages");

            datum.AccountID = this.Parent.AccountID;
            datum.Slots = this.Slots;
            datum.Meso = this.Meso;
            datum.Update("AccountID = '{0}'", this.Parent.AccountID);

            foreach (Item loopItem in this)
            {
                if (!loopItem.Assigned)
                {
                    loopItem.Assigned = true;
                    loopItem.SaveToStorage(this.Parent);
                }
            }
        }

        public void Open(Npc npc)
        {
            this.Load();
            this.Npc = npc;

            using (Packet outPacket = new Packet(MapleServerOperationCode.Storage))
            {
                outPacket.WriteByte(0x16);
                outPacket.WriteInt(this.Npc.MapleID);
                outPacket.WriteByte(this.Slots);
                outPacket.WriteShort(0x7E);
                outPacket.WriteShort(0);
                outPacket.WriteInt(0);
                outPacket.WriteInt(this.Meso);
                outPacket.WriteShort(0);
                outPacket.WriteByte((byte)this.Count);

                foreach (Item loopItem in this)
                {
                    outPacket.WriteBytes(loopItem.ToByteArray(true));
                }

                outPacket.WriteShort(0);
                outPacket.WriteByte(0);

                this.Parent.Client.Send(outPacket);
            }
        }

        public void StoreItem(Item item)
        {
            this.Add(item);

            using (Packet outPacket = new Packet(MapleServerOperationCode.Storage))
            {
                outPacket.WriteByte(0xD);
                outPacket.WriteByte(this.Slots);
                outPacket.WriteShort((short)(2 << (byte)item.Type));
                outPacket.WriteShort(0);
                outPacket.WriteInt(0);
                outPacket.WriteByte((byte)this.CountTypeItems(item.Type));

                foreach (Item loopItem in this)
                {
                    if (loopItem.Type == item.Type)
                    {
                        outPacket.WriteBytes(loopItem.ToByteArray(true));
                    }
                }

                this.Parent.Client.Send(outPacket);
            }
        }

        public void TakeItem(ItemType type, sbyte slot)
        {
            if (this[slot].Assigned)
            {
                this[slot].DeleteFromStorage();
            }
            this.RemoveAt(slot);

            using (Packet outPacket = new Packet(MapleServerOperationCode.Storage))
            {
                outPacket.WriteByte(0x9);
                outPacket.WriteByte(this.Slots);
                outPacket.WriteShort((short)(2 << (byte)type));
                outPacket.WriteShort(0);
                outPacket.WriteInt(0);
                outPacket.WriteByte((byte)this.CountTypeItems(type));

                foreach (Item loopItem in this)
                {
                    if (loopItem.Type == type)
                    {
                        outPacket.WriteBytes(loopItem.ToByteArray(true));
                    }
                }

                this.Parent.Client.Send(outPacket);
            }
        }

        public void ModifyMeso(int meso)
        {
            this.Meso = meso;

            using (Packet outPacket = new Packet(MapleServerOperationCode.Storage))
            {
                outPacket.WriteByte(0x13);
                outPacket.WriteByte(this.Slots);
                outPacket.WriteShort(2);
                outPacket.WriteShort(0);
                outPacket.WriteInt(0);
                outPacket.WriteInt(meso);

                this.Parent.Client.Send(outPacket);
            }
        }

        public bool IsFull()
        {
            if (this.Count < this.Slots)
            {
                return false;
            }

            using (Packet outPacket = new Packet(MapleServerOperationCode.Storage))
            {
                outPacket.WriteByte(0x11);

                this.Parent.Client.Send(outPacket);
            }

            return true;
        }

        public void Handle(Packet inPacket)
        {
            byte action = inPacket.ReadByte();
            sbyte slot;
            Item item;

            switch (action)
            {
                case 4: // Take item
                    byte type = inPacket.ReadByte();
                    slot = (sbyte)inPacket.ReadByte();
                    item = this[slot];
                    if (item != null)
                    {
                        if (this.Parent.Map.MapleID == 910000000 || this.Parent.Map.MapleID == 910001000)
                        {
                            if (!this.Parent.Items.IsFull((ItemType)type))
                            {
                                item.Slot = 0;
                                this.Parent.Items.Add(item);
                                this.TakeItem((ItemType)type, slot);
                            }
                            else
                            {
                                this.Parent.Items.NotifyFull();
                            }
                        }
                        else
                        {
                            this.Parent.Notify("You don't have enough mesos to take the item");
                        }
                    }
                    break;

                case 5: // Store item
                    slot = (sbyte)inPacket.ReadShort();
                    int itemid = inPacket.ReadInt();
                    short quantity = inPacket.ReadShort();
                    item = this.Parent.Items[itemid, slot];
                    if (!this.IsFull())
                    {
                        if (this.Parent.Meso >= this.Npc.StorageCost)
                        {
                            if (quantity > 0 && item.Quantity >= quantity)
                            {
                                if (item.Quantity != quantity)
                                {
                                    this.Parent.Items.Remove(item.MapleID, quantity);
                                    this.StoreItem(new Item(item.MapleID, quantity));
                                }
                                else
                                {
                                    this.Parent.Items.Remove(item, true);
                                    item.Assigned = false;
                                    this.StoreItem(item);
                                }

                                this.Parent.Meso -= this.Npc.StorageCost;
                            }
                        }
                        else
                        {
                            this.Parent.Notify("You don't have enough mesos to store the item");
                        }
                    }
                    break;

                case 6: // Orginize items ?
                    break;

                case 7: // Modify meso
                    int meso = inPacket.ReadInt();

                    if (meso > 0 && this.Meso >= meso) // Take meso
                    {
                        if (this.Parent.Meso + meso < 0)
                        {
                            meso = int.MaxValue - this.Parent.Meso;
                            if (meso > this.Meso) return;
                        }
                        
                    }
                    else if (meso < 0 && this.Parent.Meso >= -meso) // Put meso
                    {
                        if (this.Meso - meso < 0)
                        {
                            meso = -int.MaxValue + this.Meso;
                            if (-meso > this.Parent.Meso) return;
                        }
                    }
                    else
                    {
                        return;
                    }
                    this.Parent.Meso += meso;
                    this.ModifyMeso(this.Meso - meso);
                    break;

                case 8: // Close
                    this.Save();
                    this.Npc = null;
                    break;
            }
        }

        public int CountTypeItems(ItemType type)
        {
            int count = 0;

            foreach (Item loopItem in this)
            {
                if (loopItem.Type == type)
                {
                    count++;
                }
                
            }
            return count;
        }
    }
}
