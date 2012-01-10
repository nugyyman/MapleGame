using Loki.Collections;
using System.Collections.Generic;

namespace Loki.Maple.Maps
{
    public abstract class MapObjects<T> : NumericalKeyedCollection<T> where T : MapObject
    {
        public Map Map { get; private set; }

        public MapObjects(Map parent)
        {
            this.Map = parent;
        }

        protected override int GetKeyForItem(T item)
        {
            return item.ObjectID;
        }

        public IEnumerable<T> GetInRange(MapObject reference, int range)
        {
            foreach (T loopObject in this)
            {
                if (reference.Position.DistanceFrom(loopObject.Position) <= range)
                {
                    yield return loopObject;
                }
            }
        }

        protected override void InsertItem(int index, T item)
        {
            item.Map = this.Map;
            item.ObjectID = this.Map.AssignObjectID();

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            T item = this.GetAtIndex(index);

            item.Map = null;
            item.ObjectID = -1;

            base.RemoveItem(index);
        }
    }
}
