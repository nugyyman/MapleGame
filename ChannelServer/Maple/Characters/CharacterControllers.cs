using System.Collections.Generic;
using Loki.Collections;
using Loki.Maple.Life;
using Loki.Net;

namespace Loki.Maple.Characters
{
	public abstract class CharacterControllers<T> : NumericalKeyedCollection<T> where T : IControllable
	{
		public Character Parent { get; private set; }

		public CharacterControllers(Character parent)
			: base()
		{
			this.Parent = parent;
		}

		protected override void InsertItem(int index, T item)
		{
			item.Controller = this.Parent;
			item.Controller.Client.Send(item.GetControlRequestPacket());

			base.InsertItem(index, item);
		}

		protected override void RemoveItem(int index)
		{
			T item = this.GetAtIndex(index);

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

		protected override void ClearItems()
		{
			List<T> toRemove = new List<T>();

			foreach (T loopController in this)
			{
				toRemove.Add(loopController);
			}

			foreach (T loopController in toRemove)
			{
				this.Remove(loopController);
			}
		}
	}
}
