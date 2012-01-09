using Loki.Maple.Data;
using Loki.Maple.Life;
using Loki.Net;

namespace Loki.Maple.Maps
{
	public class MapNpcs : MapObjects<Npc>
	{
		public MapNpcs(Map parent) : base(parent) { }

		protected override void InsertItem(int index, Npc item)
		{
			lock (this)
			{
				base.InsertItem(index, item);

				if (World.IsInitialized)
				{
					using (Packet create = item.GetCreatePacket())
					{
						item.Map.Broadcast(create);
					}

					item.AssignController();
				}
			}
		}

		protected override int GetKeyForItem(Npc item)
		{
			return item.ID;
		}

		protected override void RemoveItem(int index)
		{
			lock (this)
			{
				Npc item = this.GetAtIndex(index);

				if (World.IsInitialized)
				{
					item.Controller.ControlledNpcs.Remove(item);

					using (Packet destroy = item.GetDestroyPacket())
					{
						item.Map.Broadcast(destroy);
					}
				}

				base.RemoveItem(index);
			}
		}
	}
}
