using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.Maple.Characters;
using Loki.Maple.Maps;

namespace Loki.Maple.Commands.Implementation
{
    class ItemVacCommand : Command
    {
        public override bool IsRestricted { get { return true; } }
        public override string Name { get { return "itemvac"; } }
        public override string Parameters { get { return string.Empty; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length != 0)
            {
                ShowSyntax(caller);
            }
            else
            {
                lock (caller.Map.Drops)
                {
                    List<Drop> drops = new List<Drop>();

                    foreach (Drop loopDrop in caller.Map.Drops)
                    {
                        drops.Add(loopDrop);
                    }

                    foreach(Drop loopDrop in drops)
                    {
                        caller.Items.Pickup(loopDrop);
                    }
                }
            }
        }
    }
}
