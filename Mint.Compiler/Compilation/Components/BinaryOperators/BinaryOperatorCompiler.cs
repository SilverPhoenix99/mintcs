using Mint.MethodBinding.Arguments;
using System.Linq.Expressions;
using Mint.Parse;

namespace Mint.Compilation.Components
{
    internal abstract class BinaryOperatorCompiler : CompilerComponentBase
    {
        private SyntaxNode LeftNode => Node[0];

        private SyntaxNode RightNode => Node[1];

        protected abstract Symbol Operator { get; }

        protected BinaryOperatorCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var left = LeftNode.Accept(Compiler);
            var right = RightNode.Accept(Compiler);

            var visibility = LeftNode.GetVisibility();
            var argument = new InvocationArgument(ArgumentKind.Simple, right);
            return CompilerUtils.Call(left, Operator, visibility, argument);
        }
    }
}