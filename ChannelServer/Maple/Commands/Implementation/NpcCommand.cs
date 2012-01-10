using System;
using Loki.Maple.Characters;
using Loki.Maple.Data;

namespace Loki.Maple.Commands.Implementation
{
    class NpcCommand : Command
    {
        public override bool IsRestricted { get { return true; } }
        public override string Name { get { return "npc"; } }
        public override string Parameters { get { return "{ id | exact name }"; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length < 1)
            {
                ShowSyntax(caller);
            }
            else
            {
                int npcId;

                try
                {
                    npcId = int.Parse(args[0]);
                }
                catch (FormatException)
                {
                    npcId = Strings.Npcs[CombineArgs(args)];
                }

                caller.Converse(npcId);
            }
        }
    }
}
