using Loki.Maple.Maps;
using Loki.Net;
using Loki.Maple.Characters;

namespace Loki.Maple
{
    public class Meso : Drop
    {
        public int Amount { get; private set; }

        public Meso(int amount)
            : base()
        {
            this.Amount = amount;
        }

        public override Packet GetShowGainPacket()
        {
            Packet showGain = new Packet(MapleServerOperationCode.ShowLog);

            showGain.WriteBytes(0, 1, 0);
            showGain.WriteInt(this.Amount);
            showGain.WriteShort();

            return showGain;
        }
    }
}
