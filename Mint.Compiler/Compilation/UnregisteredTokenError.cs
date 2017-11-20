namespace Mint.Compilation
{
    internal class UnregisteredTokenError : CompilerException
    {
        public UnregisteredTokenError()
        { }

        public UnregisteredTokenError(string message) : base(message)
        { }

        public UnregisteredTokenError(string message, Exception innerException) : base(message, innerException)
        { }
    }
}