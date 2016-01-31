using System;

namespace Mint
{
    class SyntaxError : Exception
    {
        public SyntaxError() : base() { }
        public SyntaxError(int line, string message) : base($":{line}: {message}") { }
        public SyntaxError(int line, string message, Exception innerException) : base($":{line}: {message}", innerException) { }
    }
}
