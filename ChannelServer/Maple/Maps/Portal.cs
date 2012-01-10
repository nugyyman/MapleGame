using System;
using Loki.Data;
using Loki.Maple.Data;

namespace Loki.Maple.Maps
{
    public class Portal : MapObject
    {
        public byte ID { get; private set; }
        public string Label { get; private set; }
        public int DestinationMapID { get; set; }
        public string DestinationLabel { get; set; }
        public string Script { get; private set; }
        public bool OnlyOnce { get; private set; }

        public bool IsSpawnPoint
        {
            get
            {
                return this.Label == "sp";
            }
        }

        public Map DestinationMap
        {
            get
            {
                return World.Maps[this.DestinationMapID];
            }
        }

        public Portal Link
        {
            get
            {
                return World.Maps[this.DestinationMapID].Portals[this.DestinationLabel];
            }
        }

        public Portal(dynamic datum)
        {
            this.ID = datum.id;
            this.Label = datum.label;

            this.DestinationMapID = datum.destination;
            this.DestinationLabel = datum.destination_label;

            this.Script = datum.script;
            this.OnlyOnce = datum.flags.Contains("only_once");

            this.Position = new Point(datum.x_pos, datum.y_pos);
        }
    }
}
