using System.Collections.ObjectModel;

namespace Loki.Interoperability
{
	public class Worlds : KeyedCollection<byte, World>
	{
		protected override byte GetKeyForItem(World item)
		{
			return item.ID;
		}
	}
}
