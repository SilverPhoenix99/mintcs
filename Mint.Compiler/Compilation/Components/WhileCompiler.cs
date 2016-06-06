using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class WhileCompiler : CompilerComponentBase
    {
        public WhileCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
            BeginScope();
            base.Shift();
        }

        public override Expression Reduce()
        {
            try
            {
                var condition = Pop();
                var body = Pop();

                condition = ToBool(condition);
                
                return Reduce(condition, body);
            }
            finally
            {
                EndScope();
            }
        }

        private void BeginScope()
        {
            var closure = Compiler.CurrentScope.Closure;
            Compiler.CurrentScope = Compiler.CurrentScope.Enter(ScopeType.While, closure);
        }

        private void EndScope()
        {
            Compiler.CurrentScope = Compiler.CurrentScope.Previous;
        }

        private Expression ToBool(Expression condition)
        {
            condition = CompilerUtils.ToBool(condition);

            if(Node.Value.Type == kUNTIL || Node.Value.Type == kUNTIL_MOD)
            {
                condition = CompilerUtils.Negate(condition);
            }
            return condition;
        }

        protected virtual Expression Reduce(Expression condition, Expression body)
        {
            var scope = Compiler.CurrentScope;

            /*
             * next:
             *     if(cond)
             *     {
             * redo:
             *         body;
             *         goto next;
             *     }
             * break: nil;
             */

            return Block(
                typeof(iObject),
                Label(scope.NextLabel),
                IfThen(
                    condition,
                    Block(
                        Label(scope.RedoLabel),
                        body,
                        Goto(scope.NextLabel)
                    )
                ),
                Label(scope.BreakLabel, CompilerUtils.NIL)
            );
        }
    }
}
