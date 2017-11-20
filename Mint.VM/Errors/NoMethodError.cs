namespace Mint
{
    public class NoMethodError : NameError
    {
        public NoMethodError()
        { }

        public NoMethodError(string message) : base(message)
        { }

        public NoMethodError(string message, Exception innerException) : base(message, innerException)
        { }
    }
}