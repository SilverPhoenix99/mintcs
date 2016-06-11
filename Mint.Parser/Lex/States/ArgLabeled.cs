using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Lex.States
{
    internal partial class ArgLabeled : ArgBase
    {
        protected override TokenType DoubleStarTokenType => kDSTAR;
        protected override TokenType StarTokenType => kSTAR;
        protected override TokenType PlusTokenType => kUPLUS;
        protected override TokenType LeftBracketTokenType => kLBRACK;
        protected override TokenType LeftParenTokenType => kLPAREN;
        protected override TokenType IfTokenType => kIF;
        protected override TokenType UnlessTokenType => kUNLESS;
        protected override TokenType UntilTokenType => kUNTIL;
        protected override TokenType WhileTokenType => kWHILE;

        public ArgLabeled(Lexer lexer) : base(lexer)
        { }

        protected override void EmitModifierKeywordToken(TokenType type)
        {
            Lexer.EmitToken(type, ts, te);
            Lexer.CurrentState = Lexer.BegState;
            Lexer.CommandStart = true;
        }

        protected override void EmitRescueToken()
        {
            Lexer.EmitToken(kRESCUE, ts, te);
            Lexer.CurrentState = Lexer.MidState;
        }

        protected override void EmitDivToken()
        {
            Lexer.EmitStringToken(ts, te);
        }
    }
}