using System;

namespace Loki.Maple
{
	public class HackException : Exception
	{
		public HackException() : base("Player operation is illegal.") { }

		public HackException(string message) : base(message) { }
	}
}
