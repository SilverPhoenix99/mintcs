using Mint.Parse;

namespace Mint.Lex.States
{
    internal abstract class StateBase : State
    {
        protected int ts;
        protected int te;
        protected int cs;
        protected int act;
        protected int tokStart;
        protected int numBase;
        protected bool isImaginary;
        protected bool isRational;

        protected virtual bool CanLabel => false;
        protected virtual State OperatorState => Lexer.BegState;
        protected Lexer Lexer { get; }
        protected int eof => Lexer.DataLength;
        protected virtual char CurrentChar => Lexer.CurrentChar;

        protected StateBase(Lexer lexer)
        {
            Lexer = lexer;
        }

        public abstract void Advance();

        protected virtual void Reset(int initialState)
        {
            ts = -1;
            te = -1;
            cs = initialState;
            act = 0;
            tokStart = -1;
            isImaginary = false;
            isRational = false;
            numBase = 0;
        }
    }
}
