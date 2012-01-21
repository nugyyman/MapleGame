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
                caller.ResetStats();
            }
        }
    }
}
