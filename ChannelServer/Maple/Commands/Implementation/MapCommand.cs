using System;
using Loki.Maple.Characters;
using Loki.Maple.Data;
using System.Collections.Generic;

namespace Loki.Maple.Commands.Implementation
{
    class MapCommand : Command
    {
        public override bool IsRestricted { get { return true; } }
        public override string Name { get { return "map"; } }
        public override string Parameters { get { return "{ { id | keyword | exact name } [portal] | -current }"; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length == 0)
            {
                ShowSyntax(caller);
            }
            else
            {
                if (args.Length == 1 && args[0] == "-current")
                {
                    caller.Notify("[Command] Current map: " + caller.Map.MapleID);
                    caller.Notify("   -X: " + caller.Position.X);
                    caller.Notify("   -Y: " + caller.Position.Y);
                    caller.Notify("   -CP: " + caller.ClosestPortal.ID);
                    caller.Notify("   -CSP: " + caller.ClosestSpawnPoint.ID);
                }
                else
                {
                    byte portal = 0;
                    bool isPortalSpecified;

                    if (args.Length > 1)
                    {
                        isPortalSpecified = byte.TryParse(args[args.Length - 1], out portal);
                    }
                    else
                    {
                        isPortalSpecified = false;
                    }

                    int mapId = -1;

                    try
                    {
                        mapId = int.Parse(args[0]);
                    }
                    catch (FormatException)
                    {
                        mapId = Strings.Maps[isPortalSpecified ? CombineArgs(args, 0, args.Length - 1) : CombineArgs(args)];
                    }

                    if (World.Maps.Contains(mapId))
                    {
                        caller.ChangeMap(mapId, portal);
                    }
                    else
                    {
                        caller.Notify("[Command] Invalid map.");
                    }
                }
            }
        }
    }
}
