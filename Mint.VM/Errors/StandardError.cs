namespace Mint
{
    public class StandardError : Exception
    {
        public StandardError()
        { }

        public StandardError(string message) : base(message)
        { }

        public StandardError(string message, System.Exception innerException) : base(message, innerException)
        { }
    }
}