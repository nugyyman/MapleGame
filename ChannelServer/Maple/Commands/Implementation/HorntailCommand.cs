using System;
using Loki.Maple.Characters;
using Loki.Maple.Data;
using Loki.Maple.Life;

namespace Loki.Maple.Commands.Implementation
{
    class HorntailCommand : Command
    {
        public override bool IsRestricted { get { return true; } }
        public override string Name { get { return "horntail"; } }
        public override string Parameters { get { return string.Empty; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length != 0)
            {
                ShowSyntax(caller);
            }
            else
            {
                Mob summoner = new Mob(Strings.Mobs["Summon Horntail"]) { Position = caller.Position };
                caller.Map.Mobs.Add(summoner);
                caller.Map.Mobs.Remove(summoner);
            }
        }
    }
}
