using System.Linq.Expressions;
using Mint.Compilation.Scopes;
using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class WhileCompiler : CompilerComponentBase
    {
        private Ast<Token> ConditionNode => Node[0];

        private Ast<Token> BodyNode => Node[1];

        public WhileCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            BeginScope();

            try
            {
                var condition = ConditionNode.Accept(Compiler);
                var body = BodyNode.Accept(Compiler);

                condition = ToBool(condition);

                return Compile(condition, body);
            }
            finally
            {
                Compiler.EndScope();
            }
        }

        private void BeginScope() => Compiler.CurrentScope = new LoopScope(Compiler);

        private Expression ToBool(Expression condition)
        {
            condition = CompilerUtils.ToBool(condition);

            if(Node.Value.Type == kUNTIL || Node.Value.Type == kUNTIL_MOD)
            {
                condition = CompilerUtils.Negate(condition);
            }
            return condition;
        }

        protected virtual Expression Compile(Expression condition, Expression body)
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

             throw new System.NotImplementedException();

            /*return Block(
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
                Label(scope.BreakLabel, NilClass.Expressions.Instance)
            );*/
        }
    }
}
