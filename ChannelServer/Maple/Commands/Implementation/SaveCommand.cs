using System.Collections.Generic;
using Loki.Maple.Characters;
using Loki.Maple.Data;

namespace Loki.Maple.Commands.Implementation
{
    class SaveCommand : Command
    {
        public override bool IsRestricted { get { return false; } }
        public override string Name { get { return "save"; } }
        public override string Parameters { get { return "[ (restricted) { name | -all } ]"; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length > 1)
            {
                ShowSyntax(caller);
            }
            else
            {
                if (args.Length == 1)
                {
                    if (caller.IsMaster)
                    {
                        if (args[0].ToLower() == "-all")
                        {
                            foreach (Character loopCharacter in World.Characters)
                            {
                                loopCharacter.Save();
                            }

                            caller.Notify("[Command] All characters saved to database.");
                        }
                        else
                        {
                            try
                            {
                                Character target = World.Characters[args[0]];

                                target.Save();
                                caller.Notify("[Command] Character '" + args[0] + "' saved to database.");
                            }
                            catch (KeyNotFoundException)
                            {
                                caller.Notify("[Command] Character '" + args[0] + "' could not be found.");
                            }
                        }
                    }
                    else
                    {
                        caller.Notify("[Command] Restricted argument.");
                    }
                }
                else
                {
                    caller.Save();
                    caller.Notify("[Command] Character saved to database.");
                }
            }
        }
    }
}