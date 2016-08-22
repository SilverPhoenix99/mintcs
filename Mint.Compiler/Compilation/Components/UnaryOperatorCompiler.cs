using System.Linq.Expressions;
using Mint.Parse;

namespace Mint.Compilation.Components
{
    internal abstract class UnaryOperatorCompiler : CompilerComponentBase
    {
        protected abstract Symbol Operator { get; }

        private Ast<Token> Operand => Node[0];

        protected UnaryOperatorCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var operand = Pop();
            var visibility = CompilerUtils.GetVisibility(Operand);
            return CompilerUtils.Call(operand, Operator, visibility);
        }
    }
}
