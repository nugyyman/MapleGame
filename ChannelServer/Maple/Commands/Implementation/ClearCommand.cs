using Loki.Maple.Characters;

namespace Loki.Maple.Commands.Implementation
{
    class ClearCommand : Command
    {
        public override bool IsRestricted { get { return true; } }
        public override string Name { get { return "clear"; } }
        public override string Parameters { get { return ""; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length != 0)
            {
                ShowSyntax(caller);
            }
            else
            {
                for (int i = 0; i < 500; i++)
                {
                    caller.Notify(string.Empty);
                }
            }
        }
    }
}