using System;

namespace Loki.Maple
{
	public class InventoryFullException : Exception
	{
		public override string Message
		{
			get
			{
				return "The inventory is full.";
			}
		}
	}
}
