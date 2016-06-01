using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class PosfixWhileCompiler : WhileCompiler
    {
        public PosfixWhileCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
            base.Shift();
            Compiler.CurrentScope.RedoLabel = Compiler.CurrentScope.NextLabel;
        }

        protected override Expression Reduce(Expression condition, Expression body)
        {
            var scope = Compiler.CurrentScope;

            return Loop(
                Block(
                    typeof(iObject),
                    body,
                    IfThen(condition, Continue(scope.NextLabel)),
                    Break(scope.BreakLabel, CompilerUtils.NIL, typeof(iObject))
                ),
                scope.BreakLabel,
                scope.NextLabel
            );
        }
    }
}
