using System;

namespace Mint.Reflection.Parameters
{
    public class InvalidParameterError : Exception
    {
        public InvalidParameterError()
        { }


        public InvalidParameterError(string message) : base(message)
        { }


        public InvalidParameterError(string message, Exception innerException) : base(message, innerException)
        { }
    }
}