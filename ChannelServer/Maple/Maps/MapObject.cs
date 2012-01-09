using Loki.Net;

namespace Loki.Maple.Maps
{
	public abstract class MapObject
	{		
		public Point Position { get; set; }
		public virtual int ObjectID { get; set; }

		public Map Map { get; set; }

		public MapObject()
		{
			this.ObjectID = -1;
		}
	}
}
