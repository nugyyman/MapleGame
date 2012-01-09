namespace Loki.Maple
{
	public class Shortcut
	{
		public byte Type;
		public int Action;

		public Shortcut(byte type, int action)
		{
			this.Type = type;
			this.Action = action;
		}
	}
}
