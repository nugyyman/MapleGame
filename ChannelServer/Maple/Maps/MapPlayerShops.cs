using Loki.Maple.Interaction;
using Loki.Net;

namespace Loki.Maple.Maps
{
    public class MapPlayerShops : MapObjects<PlayerShop>
    {
        public MapPlayerShops(Map parent) : base(parent) { }

        protected override void InsertItem(int index, PlayerShop item)
        {
            lock (this)
            {
                base.InsertItem(index, item);

                using (Packet create = item.GetCreatePacket())
                {
                    this.Map.Broadcast(create);
                }
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (this)
            {
                PlayerShop item = this.GetAtIndex(index);

                base.RemoveItem(index);

                using (Packet destroy = item.GetCreatePacket())
                {
                    this.Map.Broadcast(destroy);
                }
            }
        }
    }
}
