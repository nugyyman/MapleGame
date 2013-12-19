using Loki.Maple.Characters;
using Loki.Maple.Data;

namespace Loki.Maple.Commands.Implementation
{
    class SayCommand : Command
    {
        public override bool IsRestricted { get { return true; } }
        public override string Name { get { return "say"; } }
        public override string Parameters { get { return "message"; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length < 1)
            {
                ShowSyntax(caller);
            }
            else
            {
                ChannelData.Notify(caller.Name + ": " + CombineArgs(args));
            }
        }
    }
}