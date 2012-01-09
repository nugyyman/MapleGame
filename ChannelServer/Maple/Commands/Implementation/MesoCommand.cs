using Loki.Maple.Characters;
using Loki.Maple.Data;

namespace Loki.Maple.Commands.Implementation
{
	class MesoCommand : Command
	{
		public override bool IsRestricted { get { return true; } }
		public override string Name { get { return "meso"; } }
		public override string Parameters { get { return "amount"; } }

		public override void Execute(Character caller, string[] args)
		{
			if (args.Length != 1)
			{
				ShowSyntax(caller);
			}
			else
			{
				caller.Meso += int.Parse(args[0]);
			}
		}
	}
}
