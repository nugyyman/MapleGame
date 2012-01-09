using Loki.Maple.Characters;
using Loki.Maple.Data;

namespace Loki.Maple.Commands.Implementation
{
	class NoticeCommand : Command
	{
		public override bool IsRestricted { get { return true; } }
		public override string Name { get { return "notice"; } }
		public override string Parameters { get { return "{ -map | -world } message"; } }

		public override void Execute(Character caller, string[] args)
		{
			if (args.Length < 2)
			{
				ShowSyntax(caller);
			}
			else
			{
				switch (args[0].ToLower())
				{
					case "-map":
						caller.Map.Notify(CombineArgs(args, 1));
						break;

					case "-world":
						World.Notify(CombineArgs(args, 1));
						break;

					default:
						ShowSyntax(caller);
						break;
				}
			}
		}
	}
}