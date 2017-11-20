namespace Mint
{
    public class RuntimeError : StandardError
    {
        public RuntimeError()
        { }

        public RuntimeError(string message) : base(message)
        { }

        public RuntimeError(string message, Exception innerException) : base(message, innerException)
        { }
    }
}