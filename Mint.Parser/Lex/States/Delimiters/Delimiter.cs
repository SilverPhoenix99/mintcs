using Mint.Parse;

namespace Mint.Lex.States.Delimiters
{
    internal interface Delimiter
    {
        bool IsNested { get; }

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
}
