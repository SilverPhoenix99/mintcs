using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class IfCompiler : CompilerComponentBase
    {
        private bool HasElse => Node.List.Count == 3;

        public IfCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var condition = Pop();
            var trueBody = Pop();
            var elseBody = HasElse ? Pop() : NilClass.Expressions.Instance;

            if(condition.NodeType == ExpressionType.Constant)
            {
                return SelectBody((ConstantExpression) condition, trueBody, elseBody);
            }

            return MakeCondition(condition, trueBody, elseBody);
        }

        private Expression SelectBody(ConstantExpression condition, Expression trueBody, Expression elseBody)
        {
            var conditionValue = Object.ToBool((iObject) condition.Value);

            if(Node.Value.Type == kUNLESS || Node.Value.Type == kUNLESS_MOD)
            {
                conditionValue = !conditionValue;
            }

            return conditionValue ? trueBody : elseBody;
        }

        private Expression MakeCondition(Expression condition, Expression trueBody, Expression elseBody)
        {
            condition = CompilerUtils.ToBool(condition);

            if(Node.Value.Type == kUNLESS || Node.Value.Type == kUNLESS_MOD)
            {
                condition = CompilerUtils.Negate(condition);
            }

            return Condition(condition, trueBody, elseBody, typeof(iObject));
        }
    }
}
