using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class IfCompiler : CompilerComponentBase
    {
        public IfCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var condition = Pop();
            var trueBody = Pop();

            var hasElse = Node.List.Count == 3;
            var elseBody = hasElse ? Pop() : CompilerUtils.NIL;

            condition = CompilerUtils.ToBool(condition);

            if(Node.Value.Type == kUNLESS || Node.Value.Type == kUNLESS_MOD)
            {
                condition = CompilerUtils.Negate(condition);
            }

            return Condition(condition, trueBody, elseBody, typeof(iObject));
        }
    }
}
