using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Net;
using System.IO;

namespace Loki.IO
{
    class PacketRecorder
    {
        string packets;

        public PacketRecorder()
        {
            Settings.Initialize();

            packets = Settings.GetString("Log/PacketRecord");
        }

        public void Record(Packet packet, string from, bool recived)
        {
            string operationCode = "NaP";

            if (recived && !from.Equals("Channel"))
            {
                operationCode = ((MapleClientOperationCode)packet.OperationCode).ToString();
            }
            else if (!recived && !from.Equals("Channel"))
            {
                operationCode = ((MapleServerOperationCode)packet.OperationCode).ToString();
            }

            if (this.packets.Contains("[" + (recived ? "recive" : "send") + ", " + operationCode + "]"))
            {
                using (TextWriter fileWriter = new StreamWriter(Application.ExecutablePath + "PacketRecords.log", true))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append(string.Format("[{0}] {1} packet {2} :\r\n", DateTime.Now.ToString(), (recived ? "Recived " : "Sent ") + operationCode, (recived ? "from " : "to ") + from));
                    sb.Append('\n');

                    if (packet == null || packet.GetContent().Length == 0)
                    {
                        sb.Append("(Empty)");
                    }
                    else
                    {
                        if (recived)
                        {
                            sb.AppendFormat("{0:X2} ", packet.OperationCode);
                            sb.AppendFormat("{0:X2} ", "00");
                        }

                        int lineSeparation = 0;

                        foreach (byte b in packet.GetContent())
                        {
                            if (lineSeparation == 22)
                            {
                                sb.Append("\r\n");
                                lineSeparation = 0;
                            }

                            sb.AppendFormat("{0:X2} ", b);
                            lineSeparation++;
                        }
                    }

                    fileWriter.WriteLine(sb.ToString() + "\r\n");
                }
            }
        }
    }
}
