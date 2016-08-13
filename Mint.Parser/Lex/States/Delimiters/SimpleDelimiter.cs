namespace Mint.Lex.States.Delimiters
{
    internal class SimpleDelimiter : Delimiter
    {
        public char CloseDelimiter { get; }

        public virtual bool IsNested => false;

        protected StringLiteral Literal { get; }

        public SimpleDelimiter(StringLiteral literal, char closeDelimiter)
        {
            Literal = literal;
            CloseDelimiter = closeDelimiter;
        }

        public virtual void IncrementNesting()
        { }

        public virtual void DecrementNesting()
        { }
    }
}