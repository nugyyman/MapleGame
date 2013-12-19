using Loki.Maple.Characters;
using Loki.Maple.Data;

namespace Loki.Maple.Commands.Implementation
{
    class TipCommand : Command
    {
        public override bool IsRestricted { get { return true; } }
        public override string Name { get { return "tip"; } }
        public override string Parameters { get { return "[ -header:header ] message"; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length < 1)
            {
                ShowSyntax(caller);
            }
            else
            {
                if (args[0].ToLower().StartsWith("-header"))
                {
                    if (args.Length < 2)
                    {
                        ShowSyntax(caller);
                    }
                    else
                    {
                        ChannelData.Tip(CombineArgs(args, 1), args[0].Split(':')[1]);
                    }
                }
                else
                {
                    ChannelData.Tip(CombineArgs(args, 0));
                }
            }
        }
    }
}
