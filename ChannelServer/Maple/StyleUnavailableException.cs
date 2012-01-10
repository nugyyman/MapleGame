using System;

namespace Loki.Maple
{
    public class StyleUnavailableException : Exception
    {
        public override string Message
        {
            get
            {
                return "The specified style does not exist.";
            }
        }
    }
}
