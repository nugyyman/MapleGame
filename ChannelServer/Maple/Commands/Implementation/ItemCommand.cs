using System;
using Loki.Maple.Characters;
using Loki.Maple.Data;

namespace Loki.Maple.Commands.Implementation
{
	class ItemCommand : Command
	{
		public override bool IsRestricted { get { return true; } }
		public override string Name { get { return "item"; } }
		public override string Parameters { get { return "{ id | exact name } [ quantity ]"; } }

		public override void Execute(Character caller, string[] args)
		{
			if (args.Length < 1)
			{
				ShowSyntax(caller);
			}
			else
			{
				short quantity = 0;
				bool isQuantitySpecified;

				if (args.Length > 1)
				{
					isQuantitySpecified = short.TryParse(args[args.Length - 1], out quantity);
				}
				else
				{
					isQuantitySpecified = false;
				}

				if (quantity < 1)
				{
					quantity = 1;
				}

				int itemId = -1;

				try
				{
					itemId = int.Parse(args[0]);
				}
				catch (FormatException)
				{
					itemId = Strings.Items[isQuantitySpecified ? CombineArgs(args, 0, args.Length - 1) : CombineArgs(args)];
				}

				if (World.CachedItems.Contains(itemId))
				{
					if (World.CachedItems[itemId].IsSealed)
					{
						caller.Notify("[Command] Restricted item.");
					}
					else
					{
						caller.Items.Add(new Item(itemId, quantity));
					}
				}
				else
				{
					caller.Notify("[Command] Invalid item.");
				}
			}
		}
	}
}
