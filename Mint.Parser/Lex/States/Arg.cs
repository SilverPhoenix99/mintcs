using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Lex.States
{
    internal partial class Arg : ArgBase
    {
        protected override bool CanLabel => true;

        public Arg(Lexer lexer) : base(lexer)
        { }

        protected override void EmitLeftBrace()
        {
            Lexer.CurrentState = Lexer.BegState;
            TokenType tokenType;
            if(Lexer.LeftParenCounter > 0 && Lexer.LeftParenCounter == Lexer.ParenNest)
            {
                tokenType = kLAMBEG;
                Lexer.LeftParenCounter = 0;
                Lexer.ParenNest--;
            }
            else
            {
                tokenType = kLBRACE2;
                Lexer.CanLabel = true;
                Lexer.CommandStart = true;
            }
            Lexer.EmitToken(tokenType, ts, te);
            Lexer.IncrementBraceCount();
            Lexer.Cond.Push(false);
            Lexer.Cmdarg.Push(false);
        }
    }
}