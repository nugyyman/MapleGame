using Loki.Maple.Characters;
using Loki.Net;
using Loki.Threading;

namespace Loki.Maple.Maps
{
    public abstract class Drop : MapObject, ISpawnable
    {
        private MapObject dropper;

        public Character Owner { get; set; }
        public Character Picker { get; set; }
        public Point Origin { get; set; }
        public Delay Expiry { get; set; }

        public MapObject Dropper
        {
            get
            {
                return dropper;
            }
            set
            {
                this.Origin = value.Position;
                this.Position = value.Position; // TODO: this.Origin = value.Map.FootHolds.FindBelow(value.Position.Y - 25);
                dropper = value;
            }
        }

        public Drop() : base() { }

        public abstract Packet GetShowGainPacket();

        public Packet GetCreatePacket()
        {
            return this.GetInternalPacket(true, null);
        }

        public Packet GetSpawnPacket()
        {
            return this.GetInternalPacket(false, null);
        }

        public Packet GetCreatePacket(Character temporaryOwner)
        {
            return this.GetInternalPacket(true, temporaryOwner);
        }

        public Packet GetSpawnPacket(Character temporaryOwner)
        {
            return this.GetInternalPacket(false, temporaryOwner);
        }

        private Packet GetInternalPacket(bool dropped, Character temporaryOwner)
        {
            Packet spawn = new Packet(MapleServerOperationCode.DropItemFromMapObject);

            spawn.WriteByte((byte)(dropped ? 1 : 2));
            spawn.WriteInt(this.ObjectID);
            spawn.WriteBool(this is Meso);

            if (this is Meso)
            {
                spawn.WriteInt(((Meso)this).Amount);
            }
            else if (this is Item)
            {
                spawn.WriteInt(((Item)this).MapleID);
            }

            spawn.WriteInt(this.Owner == null ? temporaryOwner.ID : this.Owner.ID);
            spawn.WriteByte();
            spawn.WriteShort(this.Position.X);
            spawn.WriteShort(this.Position.Y);
            spawn.WriteInt(this.Dropper.ObjectID);

            if (dropped)
            {
                spawn.WriteShort(this.Origin.X);
                spawn.WriteShort(this.Origin.Y);
                spawn.WriteShort();
            }

            if (!(this is Meso)) // TODO: This is expiration time: Implement it.
            {
                spawn.WriteLong((long)ExpirationTime.DefaultTime);
            }

            spawn.WriteByte(1); // Pet equip pickup.
            spawn.WriteByte(1); // Also pet thingy

            return spawn;
        }

        public Packet GetDestroyPacket()
        {
            Packet spawn = new Packet(MapleServerOperationCode.RemoveItemFromMap);

            spawn.WriteByte((byte)(this.Picker == null ? 0 : 2)); // NOTE: 0 - Expire, 1 - None, 2 - Pickup
            spawn.WriteInt(this.ObjectID);
            spawn.WriteInt(this.Picker == null ? 0 : this.Picker.ID);

            return spawn;
        }
    }
}
