using System;

namespace Mint
{
    public class SyntaxError : Exception
    {
        public SyntaxError(string message) : base(message)
        { }


        public SyntaxError(string message, Exception innerException) : base(message, innerException)
        { }


        public SyntaxError(string filename, int line, string message, Exception innerException = null)
            : this($"{filename}:{line}: {message}", innerException)
        { }
    }
}
