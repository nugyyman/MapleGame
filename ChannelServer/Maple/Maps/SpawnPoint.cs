using System.Dynamic;
using Loki.Maple.Life;

namespace Loki.Maple.Maps
{
    public class SpawnPoint : LifeObject
    {
        public SpawnPoint(dynamic datum) : base((DynamicObject)datum) { }

        public void Spawn(Map map)
        {
            //this.Map.Mobs.Add(new Mob(this));
            map.Mobs.Add(new Mob(this));
        }
    }
}
