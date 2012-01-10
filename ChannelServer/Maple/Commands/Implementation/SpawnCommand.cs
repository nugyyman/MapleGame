using System;
using Loki.Maple.Characters;
using Loki.Maple.Data;
using Loki.Maple.Life;

namespace Loki.Maple.Commands.Implementation
{
    class SpawnCommand : Command
    {
        public override bool IsRestricted { get { return true; } }
        public override string Name { get { return "spawn"; } }
        public override string Parameters { get { return "{ id | exact name } [ amount ]"; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length < 1)
            {
                ShowSyntax(caller);
            }
            else
            {
                int amount = 0;
                bool isAmountSpecified;

                if (args.Length > 1)
                {
                    isAmountSpecified = int.TryParse(args[args.Length - 1], out amount);
                }
                else
                {
                    isAmountSpecified = false;
                }

                if (amount < 1)
                {
                    amount = 1;
                }

                int mobId = -1;

                try
                {
                    mobId = int.Parse(args[0]);
                }
                catch (FormatException)
                {
                    mobId = Strings.Mobs[isAmountSpecified ? CombineArgs(args, 0, args.Length - 1) : CombineArgs(args)];
                }

                if (World.CachedMobs.Contains(mobId))
                {
                    for (int i = 0; i < amount; i++)
                    {
                        caller.Map.Mobs.Add(new Mob(mobId, caller.Position));
                    }
                }
                else
                {
                    caller.Notify("[Command] Invalid mob.");
                }
            }
        }
    }
}
