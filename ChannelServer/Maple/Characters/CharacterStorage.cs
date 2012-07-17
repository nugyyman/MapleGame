using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Data;
using Loki.Net;

namespace Loki.Maple.Characters
{
    public class CharacterStorage
    {
        public Character Parent { get; private set; }
        public byte Slots { get; set; }
        public int Meso { get; set; }
        public  StorageItem[] Items { get; set; }

        public CharacterStorage(Character parent)
        {
            this.Parent = parent;
        }

        public void ClearItems()
        {
            for (int i = 0; i < this.Items.Length; i++)
            {
                this.Items[i] = null;
            }
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
                this.Items = new StorageItem[this.Slots];
            }

            this.ClearItems();
            foreach (dynamic datum in new Datums("storage_items").Populate("AccountID = '{0}'", this.Parent.AccountID))
            {
                this.Items[this.GetNextFreeSlot()] = new StorageItem(new Item(datum), true);
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

            foreach(StorageItem sItem in this.Items)
            {
                if (sItem != null)
                {
                    if (!sItem.Assigned)
                    {
                        sItem.Assigned = true;
                        sItem.Item.SaveToStorage(this.Parent);
                    }
                }
            }
        }

        public void Open(int npcId)
        {
            this.Load();

            using (Packet outPacket = new Packet(MapleServerOperationCode.Storage))
            {
                outPacket.WriteByte(0x16);
                outPacket.WriteInt(npcId);
                outPacket.WriteByte(this.Slots);
                outPacket.WriteShort(0x7E);
                outPacket.WriteShort(0);
                outPacket.WriteInt(0);
                outPacket.WriteInt(this.Meso);
                outPacket.WriteShort(0);
                outPacket.WriteByte((byte)this.CountItems());

                for (int i = 0; i < this.CountItems(); i++)
                {
                    outPacket.WriteBytes(this.Items[i].Item.ToByteArray(true));
                }

                outPacket.WriteShort(0);
                outPacket.WriteByte(0);

                this.Parent.Client.Send(outPacket);
            }
        }

        public void StoreItem(StorageItem sItem)
        {
            this.Items[this.GetNextFreeSlot()] = sItem;

            using (Packet outPacket = new Packet(MapleServerOperationCode.Storage))
            {
                outPacket.WriteByte(0xD);
                outPacket.WriteByte(this.Slots);
                outPacket.WriteShort((short)(2 << (byte)sItem.Item.Type));
                outPacket.WriteShort(0);
                outPacket.WriteInt(0);
                outPacket.WriteByte((byte)this.CountTypeItems(sItem.Item.Type));

                for (int i = 0; i < Items.Length; i++)
                {
                    if (this.Items[i] != null)
                    {
                        if (this.Items[i].Item.Type == sItem.Item.Type)
                        {
                            outPacket.WriteBytes(this.Items[i].Item.ToByteArray(true));
                        }
                    }
                }

                this.Parent.Client.Send(outPacket);
            }
        }

        public void TakeItem(ItemType type, sbyte slot)
        {
            if (this.Items[slot].Assigned)
            {
                this.Items[slot].Item.DeleteFromStorage();
            }

            while(this.Items[slot] != null)
            {
                if (slot == (sbyte)(this.Items.Length - 1))
                {
                    this.Items[slot] = null;
                    break;
                }
                else
                {
                    this.Items[slot] = this.Items[slot + 1];
                }

                slot++;
            }

            using (Packet outPacket = new Packet(MapleServerOperationCode.Storage))
            {
                outPacket.WriteByte(0x9);
                outPacket.WriteByte(this.Slots);
                outPacket.WriteShort((short)(2 << (byte)type));
                outPacket.WriteShort(0);
                outPacket.WriteInt(0);
                outPacket.WriteByte((byte)this.CountTypeItems(type));

                for (int i = 0; i < this.Items.Length; i++)
                {
                    if (this.Items[i] != null)
                    {
                        if (this.Items[i].Item.Type == type)
                        {
                            outPacket.WriteBytes(this.Items[i].Item.ToByteArray(true));
                        }
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
            foreach (StorageItem sItem in this.Items)
            {
                if (sItem == null)
                {
                    return false;
                }
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
                    StorageItem sItem = this.Items[slot];
                    if (sItem != null)
                    {
                        if (!this.Parent.Items.IsFull((ItemType)type))
                        {
                            sItem.Item.Slot = 0;
                            this.Parent.Items.Add(sItem.Item);
                            this.TakeItem((ItemType)type, slot);
                        }
                        else
                        {
                            this.Parent.Items.NotifyFull();
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
                        if (quantity > 0 && item.Quantity >= quantity)
                        {
                            if (item.Quantity != quantity)
                            {
                                this.Parent.Items.Remove(item.MapleID, quantity);
                                this.StoreItem(new StorageItem(new Item(item.MapleID, quantity), false));
                            }
                            else
                            {
                                this.Parent.Items.Remove(item, true);
                                this.StoreItem(new StorageItem(item, false));
                            }

                            this.Parent.Meso -= this.Parent.Map.MapleID == 910000000 ? 500 : 100;
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
                            Log.Inform(meso);
                            meso = -int.MaxValue + this.Meso;
                            Log.Inform(meso);
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
                    break;
            }
        }

        public int CountItems()
        {
            int count = 0;

            for (int i = 0; i < this.Items.Length; i++)
            {
                if (this.Items[i] != null)
                {
                    count++;
                }
            }
            return count;
        }

        public sbyte GetNextFreeSlot()
        {
            for (sbyte i = 0; i < this.Items.Length; i++)
            {
                if (this.Items[i] == null)
                {
                    return i;
                }
            }
            throw new InventoryFullException(); // Cant really happen..
        }

        public int CountTypeItems(ItemType type)
        {
            int count = 0;

            for (int i = 0; i < this.Items.Length; i++)
            {
                if (this.Items[i] != null)
                {
                    if (this.Items[i].Item.Type == type)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }

    public class StorageItem
    {
        public Item Item { get; private set; }
        public bool Assigned { get; set; }

        public StorageItem(Item item, bool assigned)
        {
            this.Item = item;
            this.Assigned = assigned;
        }
    }
}
