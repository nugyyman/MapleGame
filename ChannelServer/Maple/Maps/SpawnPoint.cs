using System.Dynamic;
using Loki.Maple.Life;
using System;
using System.Reflection;

namespace Loki.Maple.Maps
{
    public class SpawnPoint : LifeObject
    {
        private bool IsMob { get; set; }

        public SpawnPoint(dynamic datum)
            : base((DynamicObject)datum)
        {
            this.IsMob = ((string)datum.life_type).Equals("mob");
        }

        public void Spawn(Map map)
        {
            if (this.IsMob)
            {
                map.Mobs.Add(new Mob(this));
            }
            else
            {
                Type implementedType = Assembly.GetExecutingAssembly().GetType("Loki.Maple.Life.Implementation.Reactor" + this.MapleID.ToString());

                if (implementedType != null)
                {
                    map.Reactors.Add((Reactor)Activator.CreateInstance(implementedType, this));
                }
                else
                {
                    map.Reactors.Add(new Reactor(this));
                }
            }
        }
    }
}
