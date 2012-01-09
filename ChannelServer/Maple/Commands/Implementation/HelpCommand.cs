using Loki.Maple.Characters;
using Loki.Net;

namespace Loki.Maple.Commands.Implementation
{
	class HelpCommand : Command
	{
		public override bool IsRestricted { get { return false; } }
		public override string Name { get { return "help"; } }
		public override string Parameters { get { return ""; } }

		public override void Execute(Character caller, string[] args)
		{
			if (args.Length != 0)
			{
				ShowSyntax(caller);
			}
			else
			{
				caller.Notify("[Help]");

				foreach (Command command in CommandFactory.Commands)
				{
					if ((command.IsRestricted && caller.IsMaster) || !command.IsRestricted && !(command is HelpCommand))
					{
						caller.Notify(string.Format("    !{0} {1}", command.Name, command.Parameters.ClearFormatters()));
					}
				}
			}
		}
	}
}