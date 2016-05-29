using System.Linq.Expressions;
using Mint.Parse;

namespace Mint.Compilation.Components
{
    internal class NotOperatorCompiler : CompilerComponentBase
    {
        private Ast<Token> Operand => Node[0];

        public NotOperatorCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var instance = Pop();
            var visibility = GetVisibility(Operand);
            return CompilerUtils.Call(instance, Symbol.NOT_OP, visibility);
        }
    }
}