using System.Linq.Expressions;
using Mint.Parse;
using static System.Linq.Expressions.Expression;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class IfCompiler : CompilerComponentBase
    {
        private Ast<Token> Condition => Node[0];

        private Ast<Token> IfBodyNode => Node[1];

        private Ast<Token> ElseBodyNode => Node[2];

        private bool HasElse => Node.List.Count == 3;

        public IfCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var condition = Condition.Accept(Compiler);
            var trueBody = IfBodyNode.Accept(Compiler);
            var elseBody = HasElse ? ElseBodyNode.Accept(Compiler) : NilClass.Expressions.Instance;

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
