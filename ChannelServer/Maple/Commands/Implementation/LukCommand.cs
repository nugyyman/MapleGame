using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Maple.Characters;

namespace Loki.Maple.Commands.Implementation
{
    class LukCommand : Command
    {
        public override bool IsRestricted { get { return false; } }
        public override string Name { get { return "luk"; } }
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
                if (caller.Luck + stat > 32767)
                {
                    caller.Notify("Your stats can't reach more than 32767.");
                    return;
                }
                if (caller.Luck + stat < 4 && stat < 0)
                {
                    caller.Notify("Your stats can't be lower than 4.");
                    return;
                }
                caller.AvailableAP -= stat;
                caller.Luck += stat;
            }
        }
    }
}
