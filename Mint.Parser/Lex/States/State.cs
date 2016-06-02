namespace Mint.Lex.States
{
    internal interface State
    {
        State Advance();
    }
}
