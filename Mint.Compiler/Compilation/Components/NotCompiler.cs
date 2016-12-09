using System.Linq.Expressions;
using Mint.Parse;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class NotCompiler : CompilerComponentBase
    {
        private Ast<Token> Operand => Node[0];

        public NotCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var condition = Operand.Accept(Compiler);

            if(condition.NodeType == ExpressionType.Constant)
            {
                var conditionValue = (iObject) ((ConstantExpression) condition).Value;
                return Object.ToBool(conditionValue)
                     ? FalseClass.Expressions.Instance
                     : TrueClass.Expressions.Instance;
            }

            condition = CompilerUtils.ToBool(condition);
            return Condition(
                condition,
                FalseClass.Expressions.Instance,
                TrueClass.Expressions.Instance,
                typeof(iObject)
            );
        }
    }
}