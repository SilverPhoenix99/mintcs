using System;

namespace Mint.Compilation
{
    internal class IncompleteCompilationError : CompilerException
    {
        public IncompleteCompilationError()
        { }

        public IncompleteCompilationError(string message) : base(message)
        { }

        public IncompleteCompilationError(string message, Exception innerException) : base(message, innerException)
        { }
    }
}