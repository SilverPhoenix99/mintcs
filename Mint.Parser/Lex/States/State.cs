namespace Mint.Lex.States
{
    internal interface State
    {
        State OperatorState { get; }

        bool CanLabel { get; }

        void Advance();

        void EmitIdentifierToken(int ts, int te);

        void EmitFidToken(int ts, int te);
    }
}
