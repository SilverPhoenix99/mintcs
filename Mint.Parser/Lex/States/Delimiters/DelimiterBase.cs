namespace Mint.Lex.States.Delimiters
{
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

        public abstract char CloseDelimiter { get; }

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
}