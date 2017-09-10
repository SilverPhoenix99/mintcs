using static Mint.Parse.TokenType;

namespace Mint.Lex.States
{
    internal partial class Endfn : End
    {
        public Endfn(Lexer lexer) : base(lexer)
        { }


        protected override bool CanLabel => !Lexer.CommandStart;


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
    }
}