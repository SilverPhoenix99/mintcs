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
}