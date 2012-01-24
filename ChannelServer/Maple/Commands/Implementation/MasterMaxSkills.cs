using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Maple.Characters;

namespace Loki.Maple.Commands.Implementation
{
    class MasterMaxSkills : Command
    {
        public override bool IsRestricted { get { return true; } }
        public override string Name { get { return "maxskills"; } }
        public override string Parameters { get { return string.Empty; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length != 0)
            {
                ShowSyntax(caller);
            }
            else
            {
                caller.MaxSkills();
            }
        }
    }
}
