using System;

namespace Mint
{
    public class NoMethodError : Exception
    {
        public NoMethodError()
        { }


        public NoMethodError(string message) : base(message)
        { }


        public NoMethodError(string message, Exception innerException) : base(message, innerException)
        { }
    }
}