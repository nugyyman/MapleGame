using System.Collections.Generic;
using Loki.Collections;
using Loki.Maple.Life;
using Loki.Net;

namespace Loki.Maple.Characters
{
	public class ControlledNpcs : NumericalKeyedCollection<Npc>
	{
		public Character Parent { get; private set; }

		public ControlledNpcs(Character parent)
		{
			this.Parent = parent;
		}

		protected override int GetKeyForItem(Npc item)
		{
			return item.ID;
		}

		protected override void InsertItem(int index, Npc item)
		{
			lock (this)
			{
				if (this.Parent.Client.IsAlive)
				{
					item.Controller = this.Parent;
					base.InsertItem(index, item);

					using (Packet requestControl = item.GetControlRequestPacket())
					{
						item.Controller.Client.Send(requestControl);
					}
				}
				else
				{
					item.AssignController();
				}
			}
		}

		protected override void RemoveItem(int index)
		{
			lock (this)
			{
				Npc item = this.GetAtIndex(index);

				if (item.Controller.Client.IsAlive)
				{
					using (Packet cancelControl = item.GetControlCancelPacket())
					{
						item.Controller.Client.Send(cancelControl);
					}
				}

				item.Controller = null;
				base.RemoveItem(index);
			}
		}

		protected override void ClearItems()
		{
			lock (this)
			{
				List<Npc> toRemove = new List<Npc>();

				foreach (Npc loopController in this)
				{
					toRemove.Add(loopController);
				}

				foreach (Npc loopController in toRemove)
				{
					this.Remove(loopController);
				}
			}
		}

		public void Act(Packet inPacket)
		{
			int id = inPacket.ReadInt();

			lock (this)
			{
				if (this.Contains(id))
				{
					this[id].Act(inPacket);
				}
			}
		}
	}
}
