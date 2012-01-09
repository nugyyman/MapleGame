using System.Collections.Generic;
using Loki.Maple.Characters;
using Loki.Maple.Data;
using Loki.Net;
using System;
using Loki.IO;

namespace Loki.Maple.Commands.Implementation
{
	class LogLevelCommand : Command
	{
		public override bool IsRestricted { get { return true; } }
		public override string Name { get { return "logLevel"; } }
		public override string Parameters { get { return "level"; } }

		public override void Execute(Character caller, string[] args)
		{
			if (args.Length != 1)
			{
				ShowSyntax(caller);
			}
			else
			{
				Packet.LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), args[0]);
			}
		}
	}
}
