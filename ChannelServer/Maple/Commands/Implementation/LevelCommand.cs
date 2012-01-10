using Loki.Maple.Characters;

namespace Loki.Maple.Commands.Implementation
{
    class LevelCommand : Command
    {
        public override bool IsRestricted { get { return true; } }
        public override string Name { get { return "level"; } }
        public override string Parameters { get { return "level"; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length != 1)
            {
                ShowSyntax(caller);
            }
            else
            {
                caller.Level = byte.Parse(args[0]);
            }
        }
    }
}