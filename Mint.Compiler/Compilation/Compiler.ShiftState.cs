using System.Collections.Generic;
using Mint.Parse;

namespace Mint.Compilation
{
    public partial class Compiler
    {
        private class ShiftState
        {
            public readonly Ast<Token> Node;
            public readonly Stack<Ast<Token>> Children = new Stack<Ast<Token>>();

            public ShiftState(Ast<Token> node)
            {
                Node = node;
            }
        }
    }
}
