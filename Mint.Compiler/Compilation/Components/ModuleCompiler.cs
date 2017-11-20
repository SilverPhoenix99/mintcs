using System.Linq.Expressions;
using Mint.Compilation.Scopes;
using Mint.MethodBinding;
using Mint.Parse;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal abstract class ModuleCompiler : CompilerComponentBase
    {
        protected virtual SyntaxNode NameNode => Node[0];

        protected abstract Expression Container { get; }

        protected SyntaxNode Body => Node[1];

        protected Expression Name => Constant(new Symbol(NameNode.Token.Text));

        protected Expression Nesting => Compiler.BuildNesting();

        protected ModuleCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var scope = new ModuleScope(Compiler);
            var moduleVar = scope.Module as ParameterExpression;
            Expression header = Assign(moduleVar, GetModule());
            header = CallFrame.Expressions.Push(null, header);

            Compiler.StartScope(scope);

            try
            {
                var body = Body.Accept(Compiler);
                body = Expression.TryFinally(body, CallFrame.Expressions.Pop());

                return scope.CompileBody(Expression.Block(header, body));
            }
            finally
            {
                Compiler.EndScope();
            }
        }

        protected abstract Expression GetModule();
    }
}
