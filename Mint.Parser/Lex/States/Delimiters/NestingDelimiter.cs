namespace Mint.Lex.States.Delimiters
{
    internal class NestingDelimiter : SimpleDelimiter
    {
        private readonly char openDelimter;
        private int nesting;

        public override bool IsNested => nesting > 0;

        public NestingDelimiter(StringLiteral literal, char openDelimter, char closeDelimiter)
            : base(literal, closeDelimiter)
        {
            this.openDelimter = openDelimter;
        }

        public override void IncrementNesting()
        {
            if(Literal.Lexer.CurrentChar == openDelimter)
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