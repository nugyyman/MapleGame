using Loki.Net;

namespace Loki.Maple.Maps
{
    public interface ISpawnable
    {
        Packet GetCreatePacket();
        Packet GetSpawnPacket();
        Packet GetDestroyPacket();
    }
}
