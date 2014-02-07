using System.Collections.ObjectModel;

namespace Loki.Maple.Maps
{
    public class MapSpawnPoints : KeyedCollection<int, SpawnPoint>
    {
        public MapSpawnPoints(Map parent) : base() { }

        protected override int GetKeyForItem(SpawnPoint item)
        {
            return item.ID;
        }
    }
}
