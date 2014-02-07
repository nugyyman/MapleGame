using System.Collections.ObjectModel;
using Loki.Data;
using Loki.Maple.Life;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Loki.Maple.Data
{
    public class CachedReactors : KeyedCollection<int, Reactor>
    {
        public CachedReactors()
            : base()
        {
            using (Log.Load("Reactors"))
            {
                foreach (dynamic reactorDatum in new Datums("reactor_data").Populate())
                {
                    this.Add(new Reactor(reactorDatum));
                }

                foreach (dynamic reactorEventDatum in new Datums("reactor_events").Populate())
                {
                    this[reactorEventDatum.reactorid].States[reactorEventDatum.state] = new ReactorState(reactorEventDatum);
                }

                foreach (dynamic reactorDropDatum in new Datums("reactor_drop_data").Populate())
                {
                    if (this.Contains(reactorDropDatum.reactorid))
                    {
                        this[reactorDropDatum.reactorid].Loots.Add(new Loot(reactorDropDatum, false));
                    }
                }
            }
        }

        protected override int GetKeyForItem(Reactor item)
        {
            return item.MapleID;
        }
    }
}