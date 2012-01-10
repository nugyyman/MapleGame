using Loki.Maple.Characters;

namespace Loki.Maple.Commands.Implementation
{
    class SkillCommand : Command
    {
        public override bool IsRestricted { get { return true; } }
        public override string Name { get { return "skill"; } }
        public override string Parameters { get { return "id"; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length != 1)
            {
                ShowSyntax(caller);
            }
            else
            {
                caller.Skills.Add(new Skill(int.Parse(args[0])));
            }
        }
    }
}