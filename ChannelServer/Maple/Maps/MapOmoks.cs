using Loki.Maple.Interaction;
using Loki.Net;

namespace Loki.Maple.Maps
{
    public class MapOmoks : MapObjects<Omok>
    {
        public MapOmoks(Map parent) : base(parent) { }

        protected override void InsertItem(int index, Omok item)
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
                Omok item = this.GetAtIndex(index);

                base.RemoveItem(index);

                using (Packet destroy = item.GetCreatePacket())
                {
                    this.Map.Broadcast(destroy);
                }
            }
        }
    }
}
