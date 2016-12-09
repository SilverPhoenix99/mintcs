using System.Linq.Expressions;
using Mint.Parse;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class RangeCompiler : CompilerComponentBase
    {
        private Ast<Token> LeftNode => Node[0];

        private Ast<Token> RightNode => Node[1];

        public RangeCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var left = LeftNode.Accept(Compiler);
            var right = RightNode.Accept(Compiler);
            var exclude = Constant(Node.Value.Type == TokenType.kDOT3);
            var range = Range.Expressions.New(left, right, exclude);
            return range.Cast<iObject>();
        }
    }
}