using System;
using Loki.Maple.Characters;

namespace Loki.Maple.Commands.Implementation
{
	class JobCommand : Command
	{
		public override bool IsRestricted { get { return true; } }
		public override string Name { get { return "job"; } }
		public override string Parameters { get { return "{ name | id }"; } }

		public override void Execute(Character caller, string[] args)
		{
			if (args.Length != 1)
			{
				ShowSyntax(caller);
			}
			else
			{
				try
				{
					short jobId = short.Parse(args[0]);

					if (Enum.IsDefined(typeof(Job), jobId))
					{
						caller.Job = (Job)jobId;
					}
					else
					{
						caller.Notify("[Command] Invalid job ID.");
					}
				}
				catch (FormatException)
				{
					try
					{
						caller.Job = (Job)Enum.Parse(typeof(Job), args[0], true);
					}
					catch (ArgumentException)
					{
						caller.Notify("[Command] Invalid job name.");
					}
				}
			}
		}
	}
}