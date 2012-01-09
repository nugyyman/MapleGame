using System;
using Loki.Maple.Characters;
using Loki.Maple.Data;

namespace Loki.Maple.Commands.Implementation
{
	class MasterCommand : Command
	{
		public override bool IsRestricted { get { return true; } }
		public override string Name { get { return "master"; } }
		public override string Parameters { get { return ""; } }

		public override void Execute(Character caller, string[] args)
		{
			if (args.Length != 0)
			{
				ShowSyntax(caller);
			}
			else
			{
				caller.Converse(9900001);
			}
		}
	}
}
