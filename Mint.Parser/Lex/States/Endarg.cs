using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Lex.States
{
    internal class Endarg : End
    {
        protected override bool CanLabel => false;

        public Endarg(Lexer lexer) : base(lexer)
        { }

        protected override void EmitIdentifierToken()
        {
            var token = Lexer.EmitToken(tIDENTIFIER, ts, te);
            Lexer.CurrentState = Lexer.EndState;
            var isLocalVar = Lexer.IsVariableDefined(token.Value);
            if(isLocalVar)
            {
                Lexer.CanLabel = true;
            }
        }

        protected override void EmitFidToken()
        {
            Lexer.EmitToken(tFID, ts, te - 1);
            Lexer.CurrentState = Lexer.EndState;
        }

        protected override void EmitDoToken()
        {
            var tokenType = kDO_BLOCK;

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

            Lexer.EmitToken(tokenType, ts, te);
            Lexer.CurrentState = Lexer.BegState;
            Lexer.CommandStart = true;
        }

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
                tokenType = kLBRACE_ARG;
                Lexer.CommandStart = true;
            }
            Lexer.EmitToken(tokenType, ts, te);
            Lexer.IncrementBraceCount();
            Lexer.Cond.Push(false);
            Lexer.Cmdarg.Push(false);
        }
    }
}