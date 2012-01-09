using System;

namespace Loki.Maple
{
	public class DelayException : Exception
	{
		public override string Message
		{
			get
			{
				return "Operation has timed out due to network delay.";
			}
		}
	}
}
