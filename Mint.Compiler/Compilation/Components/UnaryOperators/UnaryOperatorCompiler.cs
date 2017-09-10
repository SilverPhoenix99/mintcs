using System.Linq.Expressions;
using Mint.Parse;

namespace Mint.Compilation.Components
{
    internal abstract class UnaryOperatorCompiler : CompilerComponentBase
    {
        protected abstract Symbol Operator { get; }

        private SyntaxNode Operand => Node[0];

        protected UnaryOperatorCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var operand = Operand.Accept(Compiler);
            var visibility = Operand.GetVisibility();
            return CompilerUtils.Call(operand, Operator, visibility);
        }
    }
}
