using Mint.Binding.Arguments;
using System.Linq.Expressions;
using Mint.Parse;

namespace Mint.Compilation.Components
{
    internal abstract class BinaryOperatorCompiler : CompilerComponentBase
    {
        private Ast<Token> LeftNode => Node[0];
        protected abstract Symbol Operator { get; }

        protected BinaryOperatorCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var left = Pop();
            var right = Pop();
            
            var visibility = GetVisibility(LeftNode);
            var argument = new InvocationArgument(ArgumentKind.Simple, right);
            return CompilerUtils.Call(left, Operator, visibility, argument);
        }
    }
}