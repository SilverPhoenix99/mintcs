namespace Mint.Lex.States.Delimiters
{
    internal class SimpleDelimiter : DelimiterBase
    {
        protected override char CloseDelimiter { get; }

        public SimpleDelimiter(StringLiteral literal, char closeDelimiter) : base(literal)
        {
            CloseDelimiter = closeDelimiter;
        }
    }
}