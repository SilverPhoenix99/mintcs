using System.Linq.Expressions;
using Mint.Parse;

namespace Mint.Compilation.Components
{
    internal class SingletonClassCompiler : ClassCompiler
    {
        private Ast<Token> OperandNode => Node[0][0];

        public SingletonClassCompiler(Compiler compiler) : base(compiler)
        { }

        protected override Expression GetClass() => Object.Expressions.SingletonClass(OperandNode.Accept(Compiler));
    }
}
