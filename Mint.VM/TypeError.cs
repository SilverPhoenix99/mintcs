using System;

namespace Mint
{
    internal class TypeError : Exception
    {
        public TypeError()
        { }

        public TypeError(string message) : base(message)
        { }

        public TypeError(string message, Exception innerException) : base(message, innerException)
        { }
    }
}