using static Mint.Parse.TokenType;

namespace Mint.Lex.States
{
    internal partial class Dot : Shared
    {
        protected override State OperatorState => Lexer.ArgState;

        public Dot(Lexer lexer) : base(lexer)
        { }

        protected override void EmitIdentifierToken()
        {
            Lexer.EmitToken(tIDENTIFIER, ts, te);
            Lexer.CurrentState = Lexer.CommandStart ? Lexer.CmdargState : Lexer.ArgState;
        }

        protected override void EmitFidToken()
        {
            Lexer.EmitToken(tFID, ts, te - 1);
            Lexer.CurrentState = Lexer.CommandStart ? Lexer.CmdargState : Lexer.ArgState;
        }

        protected override void EmitDoToken()
        {
            throw new System.MethodAccessException("Dot state has no keywords.");
        }

        protected override void EmitLeftBracket()
        {
            Lexer.EmitToken(kLBRACK2, ts, te);
            Lexer.CurrentState = OperatorState;
            Lexer.CanLabel = true;
            Lexer.ParenNest++;
        }
    }
}