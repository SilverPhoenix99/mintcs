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
        protected iLiteral currentLiteral;

        public virtual bool CanLabel => false;
        public virtual State OperatorState => Lexer.BegState;
        protected Lexer Lexer { get; }
        protected int eof => Lexer.DataLength;
        protected abstract int InitialState { get; }

        protected StateBase(Lexer lexer)
        {
            Lexer = lexer;
        }

        public void Advance()
        {
            Reset();
            InternalAdvance();
        }

        private void Reset()
        {
            ts = -1;
            te = -1;
            cs = InitialState;
            act = 0;
            tokStart = -1;
            isImaginary = false;
            isRational = false;
            numBase = 0;
            currentLiteral = Lexer.CurrentLiteral;
        }

        protected abstract void InternalAdvance();

        public abstract void EmitIdentifierToken(int ts, int te);

        public abstract void EmitFidToken(int ts, int te);
    }
}
