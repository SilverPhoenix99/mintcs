using System;

namespace Mint.Lex.States
{
    internal class ExprBeg : StateBase
    {
        public ExprBeg(Lexer lexer) : base(lexer)
        { }

        public override State Advance()
        {
            throw new NotImplementedException();
        }
    }
}
