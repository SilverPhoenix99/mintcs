using Mint.Parse;

namespace Mint.Lex.States
{
    internal abstract class StateBase : State
    {
        protected Lexer Lexer { get; }

        protected StateBase(Lexer lexer)
        {
            Lexer = lexer;
        }

        public abstract State Advance();
    }
}
