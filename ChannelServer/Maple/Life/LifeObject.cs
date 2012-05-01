using Loki.Data;
using Loki.Maple.Maps;

namespace Loki.Maple.Life
{
    public abstract class LifeObject : MapObject
    {
        public int ID { get; set; }
        public int MapleID { get; private set; }
        public short Foothold { get; private set; }
        public short MinimumClickX { get; private set; }
        public short MaximumClickX { get; private set; }
        public int RespawnTime { get; private set; }
        public bool FacesLeft { get; private set; }

        public LifeObject(dynamic datum)
            : base()
        {
            this.ID = (int)datum.id;
            this.MapleID = (int)datum.lifeid;

            this.Position = new Point(datum.x_pos, datum.y_pos);
            this.Foothold = datum.foothold;
            this.MinimumClickX = datum.min_click_pos;
            this.MaximumClickX = datum.max_click_pos;
            this.RespawnTime = datum.respawn_time;
            this.FacesLeft = datum.flags.Contains("faces_left");
        }
    }
}
