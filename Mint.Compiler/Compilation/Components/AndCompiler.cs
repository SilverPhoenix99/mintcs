using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class AndCompiler : OrCompiler
    {
        public AndCompiler(Compiler compiler) : base(compiler)
        { }

        protected override Expression ChooseLeft(Expression left, Expression right) => right;

        protected override Expression ChooseRight(Expression left, Expression right) => left;
    }
}