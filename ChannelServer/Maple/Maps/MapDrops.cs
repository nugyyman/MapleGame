using Loki.Maple.Characters;
using Loki.Maple.Data;
using Loki.Net;
using Loki.Threading;

namespace Loki.Maple.Maps
{
	public class MapDrops : MapObjects<Drop>
	{
		public MapDrops(Map parent) : base(parent) { }

		protected override void InsertItem(int index, Drop item)
		{
			lock (this)
			{
				item.Picker = null;

				base.InsertItem(index, item);

				if (item.Expiry != null)
				{
					item.Expiry.Cancel(); // Mostly for disposal.
				}

				item.Expiry = new Delay(60 * 1000, () => { if (item.Map == this.Map) this.Remove(item); });// TODO: Real constant... 

				if (World.IsInitialized)
				{
					lock (this.Map.Characters)
					{
						if (item.Owner == null)
						{
							foreach (Character character in this.Map.Characters)
							{
								using (Packet create = item.GetCreatePacket(character))
								{
									character.Client.Send(create);
								}
							}
						}
						else
						{
							foreach (Character character in this.Map.Characters)
							{
								using (Packet create = item.GetCreatePacket())
								{
									character.Client.Send(create);
								}
							}
						}
					}
				}
			}
		}

		protected override void RemoveItem(int index)
		{
			lock (this)
			{
				Drop item = this.GetAtIndex(index);

				if (item.Expiry != null)
				{
					item.Expiry.Cancel();
				}

				if (World.IsInitialized)
				{
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
