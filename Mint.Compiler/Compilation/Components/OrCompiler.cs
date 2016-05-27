using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class OrCompiler : CompilerComponentBase
    {
        public OrCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var left = Pop();
            var right = Pop();

            if(left is ConstantExpression || left is ParameterExpression)
            {
                var leftResult = ChooseLeft(left, right);
                right = ChooseRight(left, right);
                return Condition(CompilerUtils.ToBool(left), leftResult, right, typeof(iObject));
            }

            var leftVar = Variable(typeof(iObject));
            var assignLeft = Assign(leftVar, left);

            left = ChooseLeft(leftVar, right);
            right = ChooseLeft(leftVar, right);
            var condition = Condition(CompilerUtils.ToBool(leftVar), left, right, typeof(iObject));

            return Block(typeof(iObject), new[] { leftVar }, assignLeft, condition);
        }

        protected virtual Expression ChooseLeft(Expression left, Expression right) => left;

        protected virtual Expression ChooseRight(Expression left, Expression right) => right;
    }
}