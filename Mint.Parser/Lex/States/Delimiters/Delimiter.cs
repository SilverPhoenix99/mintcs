namespace Mint.Lex.States.Delimiters
{
    internal interface Delimiter
    {
        bool IsNested { get; }

        char CloseDelimiter { get; }

        void IncrementNesting();

        void DecrementNesting();
    }
}