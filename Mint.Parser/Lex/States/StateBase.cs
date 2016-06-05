﻿using Mint.Parse;

namespace Mint.Lex.States
{
    internal abstract class StateBase : State
    {
        protected int ts;
        protected int te;
        protected int cs;
        protected int act;
        protected int tokStart;
        protected bool commandStart;
        protected int numBase;
        protected bool isImaginary;
        protected bool isRational;
        protected iLiteral currentLiteral;

        protected Lexer Lexer { get; }
        protected int pe => Lexer.DataLength;
        protected int eof => Lexer.DataLength;
        //protected abstract State DefaultNextState => Lexer.BegState;

        protected StateBase(Lexer lexer)
        {
            Lexer = lexer;
        }

        public abstract State Advance(State caller);

        protected void Reset(int initialState)
        {
            ts = -1;
            te = -1;
            cs = initialState;
            act = 0;
            tokStart = -1;
            commandStart = Lexer.CommandStart;
            isImaginary = false;
            isRational = false;
            numBase = 0;
            currentLiteral = Lexer.CurrentLiteral;
        }
    }
}
