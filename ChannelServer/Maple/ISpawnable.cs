using Loki.Net;

namespace Loki.Maple
{
	public interface ISpawnable
	{
		Packet GetCreatePacket();
		Packet GetSpawnPacket();
		Packet GetDestroyPacket();
	}
}
