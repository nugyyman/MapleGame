using Loki.IO;
using Loki.Net;

namespace Loki.Net
{
	public class Packet : ByteBuffer
	{
		public static LogLevel LogLevel { get; set; }

		public short OperationCode { get; private set;  }

		public Packet(byte[] data)
			: base(data)
		{
			this.OperationCode = this.ReadShort();
		}

		public Packet(short operationCode) : base()
		{
			this.OperationCode = operationCode;
			this.WriteShort(this.OperationCode);
		}

		public Packet(MapleServerOperationCode operationCode) : this((short)operationCode) { }
		public Packet(InteroperabilityOperationCode operationCode) : this((short)operationCode) { }
	}
}