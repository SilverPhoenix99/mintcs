namespace Mint.Lex.States.Delimiters
{
    internal interface Delimiter
    {
        char CurrentChar { get; }
        bool IsNested { get; }

        void IncrementNesting(char currentChar);

        void DecrementNesting(char currentChar);
    }

    internal class SimpleDelimiter : Delimiter
    {
        protected StringLiteral Literal { get; }

        public virtual char CurrentChar => Literal.Lexer.CurrentChar;

        public virtual bool IsNested => false;

        protected SimpleDelimiter(StringLiteral literal)
        {
            Literal = literal;
        }

        public virtual void IncrementNesting(char currentChar)
        { }

        public virtual void DecrementNesting(char currentChar)
        { }
    }
    
    internal class NewLineDelimiter : SimpleDelimiter
    {
        public override char CurrentChar
        {
            get
            {
                var currentChar = Literal.Lexer.CurrentChar;
                return currentChar == '\n' ? StringLiteral.LEXER_DELIMITER : currentChar;
            }
        }
        
        public NewLineDelimiter(StringLiteral literal) : base(literal)
        { }
    }

    internal class NestingDelimiter : SimpleDelimiter
    {
        private int nesting;
        private readonly char openDelimiter;
        private readonly char closeDelimiter;

        public override char CurrentChar
        {
            get
            {
                var currentChar = Literal.Lexer.CurrentChar;
                return currentChar == closeDelimiter ? StringLiteral.LEXER_DELIMITER : currentChar;
            }
        }

        public override bool IsNested => nesting > 0;

        public NestingDelimiter(StringLiteral literal, char openDelimiter, char closeDelimiter) : base(literal)
        {
            this.openDelimiter = openDelimiter;
            this.closeDelimiter = closeDelimiter;
        }

        public override void IncrementNesting(char currentChar)
        {
            if(currentChar == openDelimiter)
            {
                nesting++;
            }
        }

        public override void DecrementNesting(char currentChar)
        {
            if(currentChar == closeDelimiter)
            {
                nesting--;
            }
        }
    }
}