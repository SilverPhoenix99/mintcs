using Mint.Parse;

namespace Mint.Lex.States.Delimiters
{
    internal interface Delimiter
    {
        bool IsNested { get; }

        string Text { get; }

        char OpenDelimiter { get; }

        char CloseDelimiter { get; }

        TokenType BeginType { get; }

        LiteralFeatures Features { get; set; }

        bool CanLabel { get; }

        bool HasInterpolation { get; }

        bool IsWords { get; }

        bool IsRegexp { get; }

        void IncrementNesting();

        void DecrementNesting();
    }

    internal static class DelimiterFactory
    {
        public static Delimiter CreateDelimiter(StringLiteral literal, string text)
        {
            var openDelimiter = text[text.Length - 1];
            var closeDelimiter = NestingDelimiter.TryGetCloseDelimiter(openDelimiter);

            return closeDelimiter != null ? new NestingDelimiter(literal, text) : new SimpleDelimiter(literal, text);
        }
    }
}
