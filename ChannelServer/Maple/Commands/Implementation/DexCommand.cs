using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Maple.Characters;

namespace Loki.Maple.Commands.Implementation
{
    class DexCommand : Command
    {
        public override bool IsRestricted { get { return false; } }
        public override string Name { get { return "dex"; } }
        public override string Parameters { get { return "stat"; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length != 1)
            {
                ShowSyntax(caller);
            }
            else
            {
                short stat = short.Parse(args[0]);
                if (stat > caller.AvailableAP)
                {
                    caller.Notify("You don't have anough available ability points.");
                    return;
                }
                if (caller.Dexterity + stat > 32767)
                {
                    caller.Notify("Your stats can't reach more than 32767.");
                    return;
                }
                if (caller.Dexterity + stat < 4 && stat < 0)
                {
                    caller.Notify("Your stats can't be lower than 4.");
                    return;
                }
                Log.Warn(caller.Dexterity);
                caller.AvailableAP -= stat;
                caller.Dexterity += stat;
            }
        }
    }
}
