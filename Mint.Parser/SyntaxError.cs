using System;

namespace Mint
{
    public class SyntaxError : Exception
    {
        public SyntaxError()
        { }

        public SyntaxError(string filename, int line, string message)
            : base($"{filename}:{line}: {message}")
        { }

        public SyntaxError(string filename, int line, string message, Exception innerException)
            : base($"{filename}:{line}: {message}", innerException)
        { }
    }
}
