using System;

namespace mint
{
    class SyntaxError : Exception
    {
        public SyntaxError() : base() { }
        public SyntaxError(string message) : base(message) { }
        public SyntaxError(string message, Exception innerException) : base(message, innerException) { }
    }
}
