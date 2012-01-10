using System.Collections.Generic;
using Loki.Maple.Characters;
using Loki.Maple.Life;

namespace Loki.Maple.Commands.Implementation
{
    class KillMobsCommand : Command
    {
        public override bool IsRestricted { get { return true; } }
        public override string Name { get { return "killMobs"; } }
        public override string Parameters { get { return "[ -drop ]"; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length > 1)
            {
                ShowSyntax(caller);
            }
            else
            {
                bool drop = false;

                if (args.Length == 1)
                {
                    if (args[0].ToLower() == "-drop" || args[0].ToLower() == "-drops")
                    {
                        drop = true;
                    }
                    else
                    {
                        ShowSyntax(caller);
                        return;
                    }
                }

                lock (caller.Map.Mobs)
                {
                    List<Mob> toKill = new List<Mob>();

                    foreach (Mob loopMob in caller.Map.Mobs)
                    {
                        toKill.Add(loopMob);
                    }

                    foreach (Mob loopMob in toKill)
                    {
                        loopMob.CanDrop = drop;
                        loopMob.Die();
                    }
                }
            }
        }
    }
}
