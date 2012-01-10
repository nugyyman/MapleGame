using System;

namespace Loki.IO
{
    public class CryptographyException : Exception
    {
        public CryptographyException() : base("A cryptography error occured.") { }

        public CryptographyException(string message) : base(message) { }
    }
}
