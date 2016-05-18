using System;

namespace Mint.Reflection.Parameters
{
    public class InvalidParameterError : Exception
    {
        public InvalidParameterError(string message)
            : base(message)
        { }
    }
}