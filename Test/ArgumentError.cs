using System;

namespace Mint
{
    public class ArgumentError : Exception
    {
        public ArgumentError()
        { }

        public ArgumentError(string message) : base(message)
        { }

        public ArgumentError(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
