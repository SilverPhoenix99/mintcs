using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class AndCompiler : OrCompiler
    {
        public AndCompiler(Compiler compiler) : base(compiler)
        { }

        protected override Expression SelectBody(bool condition, Expression left, Expression right)
        {
            return condition ? right : left;
        }

        protected override Expression MakeCondition(Expression condition, Expression left, Expression right)
        {
            return Expression.Condition(condition, right, left, typeof(iObject));
        }
    }
}