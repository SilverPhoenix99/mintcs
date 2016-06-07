namespace Mint.Lex.States
{
    internal interface State
    {
        State NextState { get; }

        void Advance(State caller);
    }
}
