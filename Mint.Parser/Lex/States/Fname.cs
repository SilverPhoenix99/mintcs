using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Lex.States
{
    internal partial class Fname : Shared
    {
        public Fname(Lexer lexer) : base(lexer)
        { }


        protected override State OperatorState => Lexer.ArgState;
        protected override TokenType AmpersandTokenType => kAMPER;
        protected override TokenType NthRefTokenType => tGVAR;
        protected override TokenType BackRefTokenType => tGVAR;
        protected override TokenType IfTokenType => kIF;
        protected override TokenType UnlessTokenType => kUNLESS;
        protected override TokenType UntilTokenType => kUNTIL;
        protected override TokenType WhileTokenType => kWHILE;


        protected override void EmitIdentifierToken()
        {
            Lexer.EmitToken(tIDENTIFIER, ts, te);
            Lexer.CurrentState = Lexer.EndfnState;
        }


        protected override void EmitFidToken()
        {
            Lexer.EmitToken(tFID, ts, te - 1);
            Lexer.CurrentState = Lexer.EndfnState;
        }


        protected override void EmitDoToken()
        {
            Lexer.EmitToken(kDO, ts, te);
            Lexer.CurrentState = Lexer.BegState;
        }


        protected override void EmitModifierKeywordToken(TokenType type)
        {
            Lexer.EmitToken(type, ts, te);
            Lexer.CurrentState = Lexer.BegState;
        }


        protected override void EmitRescueToken()
        {
            Lexer.EmitToken(kRESCUE, ts, te);
            Lexer.CurrentState = Lexer.MidState;
        }


        protected override void EmitLeftBracket()
        {
            Lexer.EmitToken(kLBRACK2, ts, te);
            Lexer.CurrentState = OperatorState;
            Lexer.CanLabel = true;
            Lexer.ParenNest++;
        }


        protected override void EmitConstantToken()
        {
            Lexer.EmitToken(tCONSTANT, ts, te);
            Lexer.CurrentState = Lexer.EndfnState;
        }


        protected override void EmitBegKeywordToken(TokenType type)
        {
            Lexer.EmitToken(type, ts, te);
            Lexer.CurrentState = Lexer.BegState;
        }
    }
}