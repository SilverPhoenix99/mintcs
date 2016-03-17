using System;

namespace Mint
{
    internal class NameError : Exception
    {
        public NameError()
        { }

        public NameError(string message) : base(message)
        { }

        public NameError(string message, Exception innerException) : base(message, innerException)
        { }
    }
}