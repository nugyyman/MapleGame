using System.Collections.Generic;
using Loki.Maple.Characters;
using Loki.Maple.Data;

namespace Loki.Maple.Commands.Implementation
{
	class WarnCommand : Command
	{
		public override bool IsRestricted { get { return true; } }
		public override string Name { get { return "warn"; } }
		public override string Parameters { get { return "{ character | -map } text"; } }

		public override void Execute(Character caller, string[] args)
		{
			if (args.Length < 2)
			{
				ShowSyntax(caller);
			}
			else
			{
				if (args[0].ToLower() == "-map")
				{
					caller.Map.Notify("[Warning] " + CombineArgs(args, 1), NoticeType.Pink);
					caller.Map.Notify("[GameMaster Warning]\r\n\r\n" + CombineArgs(args, 1) + "\r\n", NoticeType.Popup);
				}
				else
				{
					try
					{
						World.Characters[args[0]].Notify("[Warning] " + CombineArgs(args, 1), NoticeType.Pink);
						World.Characters[args[0]].Notify("[GameMaster Warning]\r\n\r\n" + CombineArgs(args, 1) + "\r\n", NoticeType.Popup);
					}
					catch (KeyNotFoundException)
					{
						caller.Notify("[Command] Character '" + args[0] + "' could not be found.");
					}
				}
			}
		}
	}
}
