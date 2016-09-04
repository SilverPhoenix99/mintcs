using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class PosfixWhileCompiler : WhileCompiler
    {
        public PosfixWhileCompiler(Compiler compiler) : base(compiler)
        { }

        protected override Expression Reduce(Expression condition, Expression body)
        {
            var scope = Compiler.CurrentScope;

            /*
             * redo:
             *     body;
             * next:
             *     if(cond)
             *     {
             *         goto redo;
             *     }
             * break: nil;
             */

            return Block(
                typeof(iObject),
                Label(scope.RedoLabel),
                body,
                Label(scope.NextLabel),
                IfThen(condition, Goto(scope.RedoLabel)),
                Label(scope.BreakLabel, NilClass.Expressions.Instance)
            );
        }
    }
}
