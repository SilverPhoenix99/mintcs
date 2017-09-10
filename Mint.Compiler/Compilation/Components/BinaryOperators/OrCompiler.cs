using System.Linq.Expressions;
using Mint.Parse;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class OrCompiler : CompilerComponentBase
    {
        private SyntaxNode LeftNode => Node[0];

        private SyntaxNode RightNode => Node[1];

        public OrCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var left = LeftNode.Accept(Compiler);
            var right = RightNode.Accept(Compiler);

            if(left.NodeType == ExpressionType.Constant)
            {
                var value = (iObject) ((ConstantExpression) left).Value;
                return SelectBody(Object.ToBool(value), left, right);
            }

            if(left.NodeType == ExpressionType.Parameter)
            {
                return MakeCondition(CompilerUtils.ToBool(left), left, right);
            }

            var leftVar = Variable(typeof(iObject));

            return Block(
                typeof(iObject),
                new[] { leftVar },
                Assign(leftVar, left),
                MakeCondition(CompilerUtils.ToBool(leftVar), leftVar, right)
            );
        }

        protected virtual Expression SelectBody(bool condition, Expression left, Expression right)
        {
            return condition ? left : right;
        }

        protected virtual Expression MakeCondition(Expression condition, Expression left, Expression right)
        {
            return Condition(condition, left, right, typeof(iObject));
        }
    }
}