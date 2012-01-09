using System.Collections.Generic;
using Loki.Maple.Characters;
using Loki.Maple.Maps;

namespace Loki.Maple.Commands.Implementation
{
	class ClearDropsCommand : Command
	{
		public override bool IsRestricted { get { return true; } }
		public override string Name { get { return "clearDrops"; } }
		public override string Parameters { get { return "[ -pickup ]"; } }

		public override void Execute(Character caller, string[] args)
		{
			if (args.Length > 1)
			{
				ShowSyntax(caller);
			}
			else
			{
				lock (caller.Map.Drops)
				{
					List<Drop> toPick = new List<Drop>();

					foreach (Drop loopDrop in caller.Map.Drops)
					{
						toPick.Add(loopDrop);
					}

					foreach (Drop loopDrop in toPick)
					{
						if (args.Length == 1)
						{
							if (args[0].ToLower() == "-pickup")
							{
								loopDrop.Picker = caller;
							}
							else
							{
								ShowSyntax(caller);
								return;
							}
						}

						caller.Map.Drops.Remove(loopDrop);
					}
				}
			}
		}
	}
}
