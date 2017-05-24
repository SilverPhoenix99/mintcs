using System;

namespace Mint
{
    public class RuntimeError : Exception
    {
        public RuntimeError()
        { }


        public RuntimeError(string message) : base(message)
        { }


        public RuntimeError(string message, Exception innerException) : base(message, innerException)
        { }
    }
}