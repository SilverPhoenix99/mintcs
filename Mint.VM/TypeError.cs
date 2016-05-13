using System;

namespace Mint
{
    public class TypeError : Exception
    {
        public TypeError()
        { }

        public TypeError(string message) : base(message)
        { }

        public TypeError(string message, Exception innerException) : base(message, innerException)
        { }
    }
}