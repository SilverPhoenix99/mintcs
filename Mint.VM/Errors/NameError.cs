namespace Mint
{
    public class NameError : StandardError
    {
        public NameError()
        { }

        public NameError(string message) : base(message)
        { }

        public NameError(string message, Exception innerException) : base(message, innerException)
        { }
    }
}