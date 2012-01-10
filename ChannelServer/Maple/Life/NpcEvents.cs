using System;
using Loki.Maple.Characters;

namespace Loki.Maple.Life
{
    public delegate void NpcEventHandler(Character talker, NpcEventArgs args);
    public delegate void NpcTextEventHandler(Character talker, NpcTextEventArgs args);

    public class NpcEventArgs : EventArgs
    {
        public byte Action { get; protected set; }
        public int Selection { get; protected set; }

        public NpcEventArgs(byte action, int selection)
        {
            this.Action = action;
            this.Selection = selection;
        }
    }

    public class NpcTextEventArgs : NpcEventArgs
    {
        public string Text { get; protected set; }

        public NpcTextEventArgs(byte action, int selection, string text)
            : base(action, selection)
        {
            this.Text = text;
        }
    }
}
