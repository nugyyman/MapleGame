using System.Collections.Generic;
using Loki.Maple.Characters;
using Loki.Maple.Data;

namespace Loki.Maple.Commands.Implementation
{
    class ClearItemsCommand : Command
    {
        public override bool IsRestricted { get { return true; } }
        public override string Name { get { return "clearItems"; } }
        public override string Parameters { get { return "[ character ]"; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length > 1)
            {
                ShowSyntax(caller);
            }
            else
            {
                if (args.Length == 0)
                {
                    caller.Items.Clear(true);
                }
                else
                {
                    World.Characters[args[0]].Items.Clear(true);
                }
            }
        }
    }
}
