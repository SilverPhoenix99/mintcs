using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Lex.States
{
    internal partial class Class : Shared
    {
        public Class(Lexer lexer) : base(lexer)
        { }


        protected override TokenType DoubleStarTokenType => kDSTAR;
        protected override TokenType StarTokenType => kSTAR;
        protected override TokenType AmpersandTokenType => kAMPER;
        protected override TokenType PlusTokenType => kUPLUS;
        protected override TokenType LeftBracketTokenType => kLBRACK;
        protected override TokenType LeftParenTokenType => kLPAREN;


        protected override void EmitIdentifierToken()
        {
            var token = Lexer.EmitToken(tIDENTIFIER, ts, te);
            var isLocalVar = Lexer.IsVariableDefined(token.Text);
            if(isLocalVar)
            {
                Lexer.CurrentState = Lexer.EndState;
                Lexer.CanLabel = true;
            }
            else
            {
                Lexer.CurrentState = Lexer.CommandStart ? Lexer.CmdargState : Lexer.ArgState;
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


        protected override void EmitDivToken()
        {
            Lexer.EmitStringBeginToken(ts, te);
        }
    }
}