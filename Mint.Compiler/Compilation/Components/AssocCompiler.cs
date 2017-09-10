using System.Linq.Expressions;
using Mint.Parse;

namespace Mint.Compilation.Components
{
    internal class AssocCompiler : CompilerComponentBase
    {
        private SyntaxNode LeftNode => Node[0];

        private SyntaxNode RightNode => Node[1];

        public AssocCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var left = LeftNode.Accept(Compiler);
            var right = RightNode.Accept(Compiler);
            return CompilerUtils.NewArray(left, right);
        }
    }
}
