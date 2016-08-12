namespace Mint.Lex.States.Delimiters
{
    internal class NewLineDelimiter : DelimiterBase
    {
        public override bool CanJump => true;

        public override char CloseDelimiter => '\n';

        public NewLineDelimiter(StringLiteral literal) : base(literal)
        { }
    }
}