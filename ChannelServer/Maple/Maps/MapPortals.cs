using System.Collections.Generic;

namespace Loki.Maple.Maps
{
	public class MapPortals : MapObjects<Portal>
	{
		public MapPortals(Map parent) : base(parent) { }

		protected override int GetKeyForItem(Portal item)
		{
			return item.ID;
		}

		public Portal this[string label]
		{
			get
			{
				foreach (Portal loopPortal in this)
				{
					if (loopPortal.Label.ToLower() == label.ToLower())
					{
						return loopPortal;
					}
				}

				throw new KeyNotFoundException();
			}
		}
	}
}
