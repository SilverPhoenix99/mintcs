namespace Mint.Lex.States.Delimiters
{
    internal interface Delimiter
    {
        char CurrentChar { get; }

        bool IsNested { get; }

        bool CanJump { get; }

        void IncrementNesting();

        void DecrementNesting();
    }

    internal abstract class DelimiterBase : Delimiter
    {
        public const char LEXER_DELIMITER = '\x4';

        protected StringLiteral Literal { get; }

        public virtual char CurrentChar
        {
            get
            {
                var currentChar = Literal.Lexer.CurrentChar;
                return currentChar == CloseDelimiter ? LEXER_DELIMITER : currentChar;
            }
        }

        protected abstract char CloseDelimiter { get; }

        public virtual bool IsNested => false;

        public virtual bool CanJump => false;

        protected DelimiterBase(StringLiteral literal)
        {
            Literal = literal;
        }

        public virtual void IncrementNesting()
        { }

        public virtual void DecrementNesting()
        { }
    }

    internal class SimpleDelimiter : DelimiterBase
    {
        protected override char CloseDelimiter { get; }

        public SimpleDelimiter(StringLiteral literal, char closeDelimiter) : base(literal)
        {
            CloseDelimiter = closeDelimiter;
        }
    }

    internal class NewLineDelimiter : DelimiterBase
    {
        public override bool CanJump => true;

        protected override char CloseDelimiter => '\n';

        public NewLineDelimiter(StringLiteral literal) : base(literal)
        { }
    }

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