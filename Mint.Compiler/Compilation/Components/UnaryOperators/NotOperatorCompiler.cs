using System.Linq.Expressions;
using Mint.Parse;

namespace Mint.Compilation.Components
{
    internal class NotOperatorCompiler : CompilerComponentBase
    {
        private Ast<Token> Operand => Node[0];

        public NotOperatorCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var instance = Operand.Accept(Compiler);
            var visibility = Operand.GetVisibility();
            return CompilerUtils.Call(instance, Symbol.NOT_OP, visibility);
        }
    }
}