namespace Mint.Lex.States.Delimiters
{
    internal class NestingDelimiter : SimpleDelimiter
    {
        private const string OPEN_DELIMITERS  = "(<[{";
        private const string CLOSE_DELIMITERS = ")>]}";

        private int nesting;

        public override bool IsNested => nesting > 0;

        public NestingDelimiter(StringLiteral literal, string delimiterText)
            : base(literal, delimiterText)
        {
            CloseDelimiter = (char) TryGetCloseDelimiter(OpenDelimiter);
        }

        public override void IncrementNesting()
        {
            if(Literal.Lexer.CurrentChar == OpenDelimiter)
            {
                nesting++;
            }
        }

        public override void DecrementNesting()
        {
            nesting--;
        }

        public static char? TryGetCloseDelimiter(char openDelimiter)
        {
            var index = OPEN_DELIMITERS.IndexOf(openDelimiter);
            return index >= 0 ? CLOSE_DELIMITERS[index] : (char?) null;
        }
    }
}