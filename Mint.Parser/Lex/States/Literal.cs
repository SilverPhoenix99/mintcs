using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Lex.States
{
    internal abstract class Literal : StateBase
    {
        protected int contentStart;

        protected Literal(Lexer lexer, int contentStart) : base(lexer)
        {
            this.contentStart = contentStart;
        }


        public uint BraceCount { get; set; }
        public Token BeginToken { get; protected set; }


        public void Resume(int te)
        {
            Lexer.CurrentState = this;
            contentStart = te;
        }


        protected virtual void EmitContent(int te)
        {
            Lexer.EmitStringContentToken(contentStart, te);
        }


        protected virtual void EmitDBeg()
        {
            EmitContent(ts);
            Lexer.EmitToken(tSTRING_DBEG, ts, te);
            Lexer.CurrentState = Lexer.BegState;
            Lexer.CommandStart = true;
        }


        protected virtual void EmitDVar(TokenType type)
        {
            EmitContent(ts);
            Lexer.EmitToken(tSTRING_DVAR, ts, ts + 1);
            Lexer.EmitToken(type, ts + 1, te);
            contentStart = te;
        }
    }
}
