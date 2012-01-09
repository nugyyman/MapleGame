using System.Collections.Generic;
using Loki.Maple.Characters;
using Loki.Maple.Data;
using System;

namespace Loki.Maple.Commands.Implementation
{
	class BringCommand : Command
	{
		public override bool IsRestricted { get { return true; } }
		public override string Name { get { return "bring"; } }
		public override string Parameters { get { return "character [ to { -map | -character | -player } identifier ]"; } }

		public override void Execute(Character caller, string[] args)
		{
			if (args.Length == 0 || args.Length == 2 || args.Length == 3)
			{
				ShowSyntax(caller);
			}
			else
			{
				int mapId;
				byte portalId = 0;

				if (args.Length > 1)
				{
					if (args[1].ToLower() == "to" && (args[2].ToLower() == "-map" || args[2].ToLower() == "-character" || args[2].ToLower() == "-player" || args[2].ToLower() == "-char"))
					{
						switch (args[2].ToLower())
						{
							case "-map":
								try
								{
									mapId = int.Parse(args[3]);
								}
								catch (FormatException)
								{
									mapId = Strings.Maps[CombineArgs(args, 3)];
								}

								break;

							case "-player":
							case "-character":
							case "-char":
								try
								{
									mapId = World.Characters[args[3]].Map.MapleID;
									portalId = World.Characters[args[3]].ClosestPortal.ID;
								}
								catch (KeyNotFoundException)
								{
									caller.Notify("[Command] Character '" + args[3] + "' could not be found.");
									return;
								}

								break;

							default:
								return;
						}
					}
					else
					{
						ShowSyntax(caller);
						return;
					}
				}
				else
				{
					mapId = caller.Map.MapleID;
					portalId = caller.ClosestPortal.ID;
				}

				try
				{
					World.Characters[args[0]].ChangeMap(mapId, portalId);
				}
				catch (KeyNotFoundException)
				{
					caller.Notify("[Command] Character '" + args[0] + "' could not be found.");
				}
			}
		}
	}
}
