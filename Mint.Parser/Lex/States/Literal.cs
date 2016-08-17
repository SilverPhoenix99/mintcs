using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mint.Lex.States.Delimiters;
using Mint.Parse;
using static Mint.Parse.TokenType;
using static Mint.Lex.States.LiteralFeatures;

namespace Mint.Lex.States
{
    internal abstract class Literal : StateBase
    {
        protected int contentStart;

        public uint BraceCount { get; set; }

        public Token BeginToken { get; protected set; }

        protected Literal(Lexer lexer, int contentStart) : base(lexer)
        {
            this.contentStart = contentStart;
        }

        public void Resume(int te)
        {
            Lexer.CurrentState = this;
            contentStart = te;
        }

        protected virtual void EmitContent(int te)
        {
            Lexer.EmitStringContentToken(contentStart, te);
        }

        protected void EmitDBeg()
        {
            EmitContent(ts);
            Lexer.EmitToken(tSTRING_DBEG, ts, te);
            Lexer.CurrentState = Lexer.BegState;
            Lexer.CommandStart = true;
        }

        protected void EmitDVar(TokenType type)
        {
            EmitContent(ts);
            Lexer.EmitToken(tSTRING_DVAR, ts, ts + 1);
            Lexer.EmitToken(type, ts + 1, te);
            contentStart = te;
        }
    }
}
