using Loki.Data;
using Loki.Maple.Characters;
using Loki.Maple.Maps;
using System;

namespace Loki.Maple.Life.Implementation
{
    public class Reactor1002009 : Reactor
    {
        public Reactor1002009(SpawnPoint spawnPoint) : base(spawnPoint) { }

        public override void Act(Character attacker)
        {
            this.DropMesos(10, 20, 50);
        }
    }
}
