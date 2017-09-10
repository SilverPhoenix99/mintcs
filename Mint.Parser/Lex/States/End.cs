using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Lex.States
{
    internal partial class End : Shared
    {
        public End(Lexer lexer) : base(lexer)
        { }


        protected override bool CanLabel => Lexer.CanLabel && !Lexer.CommandStart;


        protected override void EmitIdentifierToken()
        {
            var token = Lexer.EmitToken(tIDENTIFIER, ts, te);
            Lexer.CurrentState = Lexer.EndState;
            var isLocalVar = Lexer.IsVariableDefined(token.Text);
            if(isLocalVar)
            {
                Lexer.CanLabel = true;
            }
        }


        protected override void EmitFidToken()
        {
            Lexer.EmitToken(tFID, ts, te - 1);
            Lexer.CurrentState = Lexer.CommandStart ? Lexer.CmdargState : Lexer.ArgState;
        }


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


        protected override void EmitConstantToken()
        {
            Lexer.EmitToken(tCONSTANT, ts, te);
            Lexer.CurrentState = Lexer.EndState;
        }
    }
}