namespace Mint
{
    public class IndexError : StandardError
    {
        public IndexError()
        { }

        public IndexError(string message) : base(message)
        { }

        public IndexError(string message, Exception innerException) : base(message, innerException)
        { }
    }
}