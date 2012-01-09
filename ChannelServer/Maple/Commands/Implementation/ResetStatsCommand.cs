using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Maple.Characters;
using Loki.Net;

namespace Loki.Maple.Commands.Implementation
{
    class ResetStatsCommand : Command
    {
        public override bool IsRestricted { get { return false; } }
        public override string Name { get { return "resetstats"; } }
        public override string Parameters { get { return string.Empty; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length != 0)
            {
                ShowSyntax(caller);
            }
            else
            {
                short availableAp = 0;
                if (availableAp + caller.Strength - 4 > short.MaxValue)
                    availableAp = short.MaxValue;
                else
                    availableAp += caller.Strength;
                caller.Strength = 4;
                if (availableAp + caller.Dexterity - 4 > short.MaxValue)
                    availableAp = short.MaxValue;
                else
                    availableAp += caller.Dexterity;
                caller.Dexterity = 4;
                if (availableAp + caller.Intelligence - 4 > short.MaxValue)
                    availableAp = short.MaxValue;
                else
                    availableAp += caller.Intelligence;
                caller.Intelligence = 4;
                if (availableAp + caller.Luck - 4 > short.MaxValue)
                    availableAp = short.MaxValue;
                else
                    availableAp += caller.Luck;
                caller.Luck = 4;
                if (availableAp + caller.AvailableAP > short.MaxValue)
                    caller.AvailableAP = short.MaxValue;
                else
                    caller.AvailableAP += availableAp;
            }
        }
    }
}
