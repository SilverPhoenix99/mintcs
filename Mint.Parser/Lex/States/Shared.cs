using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Lex.States
{
    internal abstract partial class Shared : StateBase
    {
        private const string SPACE_CHARS = "\t\n\v\f\r ";
        private const string PRINTABLE_SPACE_CHARS = "tnvfrs";

        private iLiteral currentLiteral;

        protected virtual TokenType DoubleStarTokenType => kPOW;
        protected virtual TokenType StarTokenType => kMUL;
        protected virtual TokenType AmpersandTokenType => kBIN_AND;
        protected virtual TokenType PlusTokenType => kPLUS;
        protected virtual TokenType LeftParenTokenType => kLPAREN2;
        protected virtual TokenType LeftBracketTokenType => kLBRACK2;
        protected virtual TokenType NthRefTokenType => tNTH_REF;
        protected virtual TokenType BackRefTokenType => tBACK_REF;
        protected virtual TokenType IfTokenType => kIF_MOD;
        protected virtual TokenType UnlessTokenType => kUNLESS_MOD;
        protected virtual TokenType UntilTokenType => kUNTIL_MOD;
        protected virtual TokenType WhileTokenType => kWHILE_MOD;

        protected Shared(Lexer lexer) : base(lexer)
        { }

        protected abstract void EmitIdentifierToken();

        protected abstract void EmitFidToken();

        protected abstract void EmitDoToken();

        protected virtual void EmitModifierKeywordToken(TokenType type)
        {
            Lexer.EmitToken(type, ts, te);
            Lexer.CurrentState = Lexer.BegState;
            Lexer.CanLabel = true;
            Lexer.CommandStart = true;
        }

        protected virtual void EmitRescueToken()
        {
            Lexer.EmitToken(kRESCUE_MOD, ts, te);
            Lexer.CurrentState = Lexer.BegState;
            Lexer.CanLabel = true;
        }

        protected virtual void EmitLeftBrace()
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
                tokenType = kLBRACE;
                Lexer.CanLabel = true;
            }
            Lexer.EmitToken(tokenType, ts, te);
            Lexer.IncrementBraceCount();
            Lexer.Cond.Push(false);
            Lexer.Cmdarg.Push(false);
        }

        protected virtual void EmitLeftBracket()
        {
            Lexer.EmitToken(LeftBracketTokenType, ts, te);
            Lexer.CurrentState = OperatorState;
            Lexer.CanLabel = true;
            Lexer.ParenNest++;
            Lexer.Cond.Push(false);
            Lexer.Cmdarg.Push(false);
        }

        protected virtual void EmitDivToken()
        {
            Lexer.EmitToken(kDIV, ts, te);
            Lexer.CurrentState = OperatorState;
        }

        protected virtual void EmitConstantToken()
        {
            Lexer.EmitToken(tCONSTANT, ts, te);
            Lexer.CurrentState = Lexer.CommandStart ? Lexer.CmdargState : Lexer.ArgState;
        }

        protected virtual void EmitBegKeywordToken(TokenType type)
        {
            Lexer.EmitToken(type, ts, te);
            Lexer.CurrentState = Lexer.BegState;
            Lexer.CommandStart = true;
        }

        protected override void Reset(int initialState)
        {
            base.Reset(initialState);
            currentLiteral = Lexer.CurrentLiteral;
        }
    }
}