using System;

namespace Mint.MethodBinding.Parameters
{
    public class InvalidParameterError : Exception
    {
        public InvalidParameterError(string message)
            : base(message)
        { }
    }
}