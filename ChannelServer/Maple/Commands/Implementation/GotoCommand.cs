using System.Collections.Generic;
using Loki.Maple.Characters;
using Loki.Maple.Data;

namespace Loki.Maple.Commands.Implementation
{
    class GoToCommand : Command
    {
        public override bool IsRestricted { get { return true; } }
        public override string Name { get { return "goto"; } }
        public override string Parameters { get { return "character"; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length != 1)
            {
                ShowSyntax(caller);
            }
            else
            {
                try
                {
                    caller.ChangeMap(World.Characters[args[0]].Map.MapleID, World.Characters[args[0]].ClosestPortal.ID);
                }
                catch (KeyNotFoundException)
                {
                    caller.Notify("[Command] Character '" + args[0] + "' could not be found.");
                }
            }
        }
    }
}
