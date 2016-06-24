namespace Mint.Lex.States.Delimiters
{
    internal class NewLineDelimiter : DelimiterBase
    {
        public override bool CanJump => true;

        protected override char CloseDelimiter => '\n';

        public NewLineDelimiter(StringLiteral literal) : base(literal)
        { }
    }
}