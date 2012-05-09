using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Maple.Characters;
using Loki.Net;
using Loki.IO;

namespace Loki.Maple.Commands.Implementation
{
    public class PacketCommand : Command
    {
        public override bool IsRestricted { get { return true; } }
        public override string Name { get { return "packet"; } }
        public override string Parameters { get { return "{ client | server } packet"; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length < 3)
            {
                ShowSyntax(caller);
            }
            else
            {
                if (args[0].ToLower().Equals("server"))
                {
                    caller.Client.Send(ByteArrayToPacket(StringToByteArray(args)));
                }
                else if (args[0].ToLower().Equals("client"))
                {
                    caller.Client.HandlePacket(ByteArrayToPacket(StringToByteArray(args)));
                }
                else
                {
                    ShowSyntax(caller);
                }
            }
        }

        static byte[] StringToByteArray(string[] args)
        {
            using (ByteBuffer buffer = new ByteBuffer())
            {
                for (int i = 1; i < args.Length; i++)
                {
                    buffer.WriteByte(byte.Parse(args[i], System.Globalization.NumberStyles.HexNumber));
                }

                buffer.Flip();
                return buffer.GetContent();
            }
        }

        static Packet ByteArrayToPacket(byte[] buffer)
        {
            Packet outPacket = new Packet(buffer);

            for (int i = 2; i < buffer.Length; i++)
            {
                outPacket.WriteByte(buffer[i]);
            }

            return outPacket;
        }
    }
}
