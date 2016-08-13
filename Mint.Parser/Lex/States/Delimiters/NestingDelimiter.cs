namespace Mint.Lex.States.Delimiters
{
    internal class NestingDelimiter : SimpleDelimiter
    {
        private readonly char openDelimiter;
        private int nesting;

        public override bool IsNested => nesting > 0;

        public NestingDelimiter(StringLiteral literal, char openDelimiter, char closeDelimiter)
            : base(literal, closeDelimiter)
        {
            this.openDelimiter = openDelimiter;
        }

        public override void IncrementNesting()
        {
            if(Literal.Lexer.CurrentChar == openDelimiter)
            {
                nesting++;
            }
        }

        public override void DecrementNesting()
        {
            nesting--;
        }
    }
}