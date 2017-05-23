using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Lex.States
{
    internal partial class Mid : Beg
    {
        public Mid(Lexer lexer) : base(lexer)
        { }


        protected override bool CanLabel => false;


        protected override void EmitDoToken()
        {
            var tokenType = kDO;

            if(Lexer.LeftParenCounter > 0 && Lexer.LeftParenCounter == Lexer.ParenNest)
            {
                Lexer.LeftParenCounter = 0;
                Lexer.ParenNest--;
                tokenType = kDO_LAMBDA;
            }
            else if(Lexer.Cond.Peek)
            {
                tokenType = kDO_COND;
            }
            else if(Lexer.Cmdarg.Peek)
            {
                tokenType = kDO_BLOCK;
            }

            Lexer.EmitToken(tokenType, ts, te);
            Lexer.CurrentState = Lexer.BegState;
            Lexer.CommandStart = true;
        }


        protected override void EmitModifierKeywordToken(TokenType type)
        {
            Lexer.EmitToken(type, ts, te);
            Lexer.CurrentState = Lexer.BegState;
            Lexer.CanLabel = true;
            Lexer.CommandStart = true;
        }


        protected override void EmitRescueToken()
        {
            Lexer.EmitToken(kRESCUE_MOD, ts, te);
            Lexer.CurrentState = Lexer.BegState;
            Lexer.CanLabel = true;
        }
    }
}