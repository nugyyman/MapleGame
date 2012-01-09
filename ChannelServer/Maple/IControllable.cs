using Loki.Maple.Characters;
using Loki.Net;

namespace Loki.Maple
{
	public interface IControllable
	{
		Packet GetControlRequestPacket();
		Packet GetControlCancelPacket();

		Character Controller { get; set; }

		void AssignController();
	}
}
