using System;
using System.Collections;
using System.Collections.Generic;
using Loki.Data;
using Loki.IO;
using Loki.Maple.Maps;
using Loki.Net;

namespace Loki.Maple.Characters
{
    public class CharacterItems : IEnumerable<Item>
    {
        public Character Parent { get; private set; }
        public Dictionary<ItemType, byte> MaxSlots { get; private set; }
        private List<Item> Items { get; set; }

        public CharacterItems(Character parent, byte maxEquipmentSlots, byte maxUsableSlots, byte maxSetupSlots, byte maxEtceteraSlots)
            : base()
        {
            this.Parent = parent;

            this.MaxSlots = new Dictionary<ItemType, byte>(Enum.GetValues(typeof(ItemType)).Length);

            this.MaxSlots.Add(ItemType.Equipment, maxEquipmentSlots);
            this.MaxSlots.Add(ItemType.Usable, maxUsableSlots);
            this.MaxSlots.Add(ItemType.Setup, maxSetupSlots);
            this.MaxSlots.Add(ItemType.Etcetera, maxEtceteraSlots);
            this.MaxSlots.Add(ItemType.Cash, 96);

            this.Items = new List<Item>();
        }

        public void Add(Item item, bool fromDrop = false, bool autoMerge = true)
        {
            if (this.Available(item.MapleID) % item.MaxPerStack != 0 && autoMerge)
            {
                foreach (Item loopItem in this)
                {
                    if (loopItem.MapleID == item.MapleID && loopItem.Quantity < loopItem.MaxPerStack)
                    {
                        if (loopItem.Quantity + item.Quantity <= loopItem.MaxPerStack)
                        {
                            loopItem.Quantity += item.Quantity;
                            loopItem.Update();

                            item.Quantity = 0;

                            break;
                        }
                        else
                        {
                            item.Quantity -= (short)(loopItem.MaxPerStack - loopItem.Quantity);
                            item.Slot = this.GetNextFreeSlot(item.Type);

                            loopItem.Quantity = loopItem.MaxPerStack;
                            if (this.Parent.IsInitialized)
                            {
                                loopItem.Update();
                            }

                            break;
                        }
                    }
                }
            }

            if (item.Quantity > 0)
            {
                item.Parent = this;

                if (this.Parent.IsInitialized && item.Slot == 0)
                {
                    item.Slot = this.GetNextFreeSlot(item.Type);
                }

                this.Items.Add(item);

                if (this.Parent.IsInitialized)
                {
                    using (Packet outPacket = new Packet(MapleServerOperationCode.ModifyInventoryItem))
                    {
                        outPacket.WriteBool(fromDrop);
                        outPacket.WriteBytes(1, 0);
                        outPacket.WriteByte((byte)item.Type);
                        outPacket.WriteSByte(item.Slot);
                        outPacket.WriteBytes(item.ToByteArray(true));

                        this.Parent.Client.Send(outPacket);
                    }
                }
            }
        }

        public void Remove(int mapleId, short quantity)
        {
            short leftToRemove = quantity;

            List<Item> toRemove = new List<Item>();

            foreach (Item loopItem in this)
            {
                if (loopItem.MapleID == mapleId)
                {
                    if (loopItem.Quantity > leftToRemove)
                    {
                        loopItem.Quantity -= leftToRemove;
                        loopItem.Update();
                        break;
                    }
                    else
                    {
                        leftToRemove -= loopItem.Quantity;
                        toRemove.Add(loopItem);
                    }
                }
            }

            foreach (Item loopItem in toRemove)
            {
                this.Remove(loopItem, true);
            }
        }

        public void Remove(Item item, bool removeFromSlot, bool fromDrop = false)
        {
            if (removeFromSlot && item.IsEquipped)
            {
                throw new InvalidOperationException("Cannot remove equipped items from slot.");
            }

            if (removeFromSlot)
            {
                using (Packet outPacket = new Packet(MapleServerOperationCode.ModifyInventoryItem))
                {
                    outPacket.WriteBool(fromDrop);
                    outPacket.WriteBytes(1, 3);
                    outPacket.WriteByte((byte)item.Type);
                    outPacket.WriteShort((short)item.Slot);

                    this.Parent.Client.Send(outPacket);
                }
            }

            if (item.Assigned)
            {
                item.Delete();
            }

            item.Parent = null;

            bool wasEquipped = item.IsEquipped;

            this.Items.Remove(item);

            if (wasEquipped)
            {
                this.Parent.UpdateLook();
            }
        }

        public void Clear(bool removeFromSlot)
        {
            List<Item> toRemove = new List<Item>();

            foreach (Item loopItem in this)
            {
                toRemove.Add(loopItem);
            }

            foreach (Item loopItem in toRemove)
            {
                if (!(loopItem.IsEquipped && removeFromSlot))
                {
                    this.Remove(loopItem, removeFromSlot);
                }
            }
        }

        public bool Contains(int mapleId)
        {
            foreach (Item loopItem in this)
            {
                if (loopItem.MapleID == mapleId)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Contains(int mapleId, short quantity)
        {
            int count = 0;

            foreach (Item loopItem in this)
            {
                if (loopItem.MapleID == mapleId)
                {
                    count += loopItem.Quantity;
                }
            }

            return count >= quantity;
        }

        public int Available(int mapleId)
        {
            int count = 0;

            foreach (Item loopItem in this)
            {
                if (loopItem.MapleID == mapleId)
                {
                    count += loopItem.Quantity;
                }
            }

            return count;
        }

        public void Load()
        {
            foreach (dynamic itemDatum in new Datums("items").Populate("CharacterID = '{0}'", this.Parent.ID))
            {
                this.Add(new Item(itemDatum));
            }
        }

        public void Save()
        {

            foreach (Item item in this)
            {
                item.Save();
            }
        }

        public void Delete()
        {
            foreach (Item item in this)
            {
                item.Delete();
            }
        }

        public void NotifyStatus(byte status)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.ShowStatusInfo))
            {
                outPacket.WriteByte();
                outPacket.WriteByte(status);
                outPacket.WriteInt();
                outPacket.WriteInt();

                this.Parent.Client.Send(outPacket);
            }
        }

        public sbyte GetNextFreeSlot(ItemType type)
        {
            for (sbyte i = 1; i <= this.MaxSlots[type]; i++)
            {
                if (this[type, i] == null)
                {
                    return i;
                }
            }

            throw new InventoryFullException();
        }

        public void NotifyFull()
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.ModifyInventoryItem))
            {
                outPacket.WriteBytes(1, 0);

                this.Parent.Client.Send(outPacket);
            }

            this.NotifyStatus(0xFF);
        }

        public bool IsFull(ItemType type)
        {
            short count = 0;

            foreach (Item item in this)
            {
                if (item.Type == type)
                {
                    count++;
                }
            }

            return (count == this.MaxSlots[type]);
        }

        public int RemainingSlots(ItemType type)
        {
            short remaining = this.MaxSlots[type];

            foreach (Item item in this)
            {
                if (item.Type == type)
                {
                    remaining--;
                }
            }

            return remaining;
        }

        public void Handle(Packet inPacket)
        {
            inPacket.ReadInt();

            ItemType type = (ItemType)inPacket.ReadByte();
            sbyte source = (sbyte)inPacket.ReadShort();
            sbyte destination = (sbyte)inPacket.ReadShort();
            short quantity = inPacket.ReadShort();

            try
            {
                Item item = this[type, source];

                if (destination < 0)
                {
                    item.Equip();
                }
                else if (source < 0 && destination > 0)
                {
                    item.Unequip(destination);
                }
                else if (destination == 0)
                {
                    item.Drop(quantity);
                }
                else
                {
                    item.Move(destination);
                }
            }
            catch (InventoryFullException)
            {
                this.NotifyFull();
            }
        }

        public void Pickup(Drop drop)
        {
            if (drop.Picker == null)
            {
                try
                {
                    drop.Picker = this.Parent;

                    if (drop is Meso)
                    {
                        this.Parent.Meso += ((Meso)drop).Amount; // TODO: Check for max meso.
                    }
                    else if (drop is Item)
                    {
                        ((Item)drop).Slot = this.GetNextFreeSlot(((Item)drop).Type); // TODO: Check for inv. full. 
                        this.Add((Item)drop, true);
                    }

                    this.Parent.Map.Drops.Remove(drop);

                    using (Packet showGain = drop.GetShowGainPacket())
                    {
                        drop.Picker.Client.Send(showGain);
                    }
                }
                catch (InventoryFullException)
                {
                    this.NotifyFull();
                }
            }
        }

        public void Pickup(Packet inPacket)
        {
            inPacket.ReadByte();
            inPacket.ReadInt();
            short x = inPacket.ReadShort();
            short y = inPacket.ReadShort(); // TODO: Check for vac hack.

            int objectId = inPacket.ReadInt();

            lock (this.Parent.Map.Drops)
            {
                if (this.Parent.Map.Drops.Contains(objectId))
                {
                    this.Pickup(this.Parent.Map.Drops[objectId]);
                }
            }
        }

        public Item this[ItemType type, sbyte slot]
        {
            get
            {
                foreach (Item item in this)
                {
                    if (item.Type == type && item.Slot == slot)
                    {
                        return item;
                    }
                }

                return null;
            }
        }

        public Item this[EquipmentSlot slot]
        {
            get
            {
                foreach (Item item in this)
                {
                    if (item.Slot == (sbyte)slot)
                    {
                        return item;
                    }
                }

                return null; // TODO: Should be keynotfoundexception, but I'm lazy.
            }
        }

        public Item this[int mapleId, sbyte slot]
        {
            get
            {
                foreach (Item item in this)
                {
                    if (item.Slot == (sbyte)slot && item.Type == Item.GetType(mapleId))
                    {
                        return item;
                    }
                }

                return null;
            }
        }

        public Item this[ItemType type, int mapleId]
        {
            get
            {
                foreach (Item item in this)
                {
                    if (item.Type == type && item.MapleID == mapleId)
                        return item;
                }
                return null;
            }
        }

        public IEnumerable<Item> this[ItemType type]
        {
            get
            {
                foreach (Item loopItem in this.Items)
                {
                    if (loopItem.Type == type && !loopItem.IsEquipped)
                    {
                        yield return loopItem;
                    }
                }
            }
        }

        public IEnumerable<Item> GetEquipped(EquippedQueryMode mode = EquippedQueryMode.Any)
        {
            foreach (Item loopItem in this.Items)
            {
                if (loopItem.IsEquipped)
                {
                    switch (mode)
                    {
                        case EquippedQueryMode.Any:
                            yield return loopItem;
                            break;

                        case EquippedQueryMode.Normal:
                            if (loopItem.Slot > -100)
                            {
                                yield return loopItem;
                            }
                            break;

                        case EquippedQueryMode.Cash:
                            if (loopItem.Slot < -100)
                            {
                                yield return loopItem;
                            }
                            break;
                    }
                }
            }
        }

        public byte[] ToByteArray()
        {
            using (ByteBuffer buffer = new ByteBuffer())
            {
                buffer.WriteByte(this.MaxSlots[ItemType.Equipment]);
                buffer.WriteByte(this.MaxSlots[ItemType.Usable]);
                buffer.WriteByte(this.MaxSlots[ItemType.Setup]);
                buffer.WriteByte(this.MaxSlots[ItemType.Etcetera]);
                buffer.WriteByte(this.MaxSlots[ItemType.Cash]);
                buffer.WriteBytes(new byte[] { 0, (byte)0x40, (byte)0xE0, (byte)0xFD, (byte)0x3B, (byte)0x37, (byte)0x4F, 1 });

                foreach (Item item in this.GetEquipped(EquippedQueryMode.Normal))
                {
                    buffer.WriteBytes(item.ToByteArray());
                }

                buffer.WriteShort();

                foreach (Item item in this.GetEquipped(EquippedQueryMode.Cash))
                {
                    buffer.WriteBytes(item.ToByteArray());
                }

                buffer.WriteShort();

                foreach (Item item in this[ItemType.Equipment])
                {
                    buffer.WriteBytes(item.ToByteArray());
                }

                buffer.WriteInt();

                foreach (Item item in this[ItemType.Usable])
                {
                    buffer.WriteBytes(item.ToByteArray());
                }

                buffer.WriteByte();

                foreach (Item item in this[ItemType.Setup])
                {
                    buffer.WriteBytes(item.ToByteArray());
                }

                buffer.WriteByte();

                foreach (Item item in this[ItemType.Etcetera])
                {
                    buffer.WriteBytes(item.ToByteArray());
                }

                buffer.WriteByte();

                foreach (Item item in this[ItemType.Cash])
                {
                    buffer.WriteBytes(item.ToByteArray());
                }

                buffer.Flip();
                return buffer.GetContent();
            }
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.Items).GetEnumerator();
        }

        public int SpaceTakenBy(Item item, bool autoMerge = true)
        {
            if (this.Available(item.MapleID) % item.MaxPerStack != 0 && autoMerge)
            {
                foreach (Item loopItem in this)
                {
                    if (loopItem.MapleID == item.MapleID && loopItem.Quantity < loopItem.MaxPerStack)
                    {
                        if (loopItem.Quantity + item.Quantity <= loopItem.MaxPerStack)
                        {
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }

                return 1;
            }
            else
            {
                return 1;
            }
        }

        public bool CouldReceive(IEnumerable<Item> items, bool autoMerge = true)
        {
            Dictionary<ItemType, int> spaceCount = new Dictionary<ItemType, int>(5);
            {
                spaceCount.Add(ItemType.Equipment, 0);
                spaceCount.Add(ItemType.Usable, 0);
                spaceCount.Add(ItemType.Setup, 0);
                spaceCount.Add(ItemType.Etcetera, 0);
                spaceCount.Add(ItemType.Cash, 0);
            }

            foreach (Item loopItem in items)
            {
                spaceCount[loopItem.Type] += this.SpaceTakenBy(loopItem, autoMerge);
            }

            foreach (KeyValuePair<ItemType, int> loopSpaceCount in spaceCount)
            {
                if (this.RemainingSlots(loopSpaceCount.Key) < loopSpaceCount.Value)
                {
                    return false;
                }
            }

            return true;
        }

        public void AddRange(IEnumerable<Item> items, bool fromDrop = false, bool autoMerge = true)
        {
            foreach (Item loopItem in items)
            {
                this.Add(loopItem, fromDrop, autoMerge);
            }
        }

        public int GetProjectile(Skill skill)
        {
            short bulletCount = 1;

            if (skill != null)
            {
                bulletCount = skill.CostBullet;
            }

            Item weapon = this[EquipmentSlot.Weapon];

            for (int i = 0; i < this.MaxSlots[ItemType.Usable]; i++)
            {
                Item loopItem = this[ItemType.Usable, (sbyte)i];

                if (loopItem != null)
                {
                    switch (weapon.WeaponType)
                    {
                        case WeaponType.Bow:
                            if (loopItem.IsArrowForBow)
                            {
                                loopItem.Quantity -= bulletCount;
                                loopItem.Update();
                                return loopItem.MapleID;
                            }
                            break;

                        case WeaponType.Crossbow:
                            if (loopItem.IsArrowForCrossbow)
                            {
                                loopItem.Quantity -= bulletCount;
                                loopItem.Update();
                                return loopItem.MapleID;
                            }
                            break;

                        case WeaponType.Claw:
                            if (loopItem.IsThrowingStar)
                            {
                                loopItem.Quantity -= bulletCount;
                                loopItem.Update();
                                return loopItem.MapleID;
                            }
                            break;

                        case WeaponType.Gun:
                            if (loopItem.IsBullet)
                            {
                                loopItem.Quantity -= bulletCount;
                                loopItem.Update();
                                return loopItem.MapleID;
                            }
                            break;

                        default:
                            //Player is using a ranged attack without a ranged weapon.
                            //Energy Orb should trigger this
                            return 0;
                    }
                }
            }

            return 0;
        }
    }

    public enum EquippedQueryMode // TODO: Move elsewhere. Maybe constants.
    {
        Any,
        Cash,
        Normal
    }
}
