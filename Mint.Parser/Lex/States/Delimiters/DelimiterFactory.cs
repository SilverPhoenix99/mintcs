namespace Mint.Lex.States.Delimiters
{
    internal static class DelimiterFactory
    {
        public static Delimiter CreateDelimiter(StringLiteral literal, string text)
            => NestingDelimiter.TryCreate(literal, text) ?? new SimpleDelimiter(literal, text);
    }
}