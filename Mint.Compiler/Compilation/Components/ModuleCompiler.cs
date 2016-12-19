﻿using System.Linq.Expressions;
using Mint.Compilation.Scopes;
using Mint.MethodBinding.Methods;
using Mint.Parse;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal abstract class ModuleCompiler : CompilerComponentBase
    {
		protected virtual Ast<Token> NameNode => Node[0];

        protected abstract Expression Container { get; }

        protected Ast<Token> Body => Node[1];

        protected Expression Name => Constant(new Symbol(NameNode.Value.Value));

        protected Expression Nesting => Compiler.BuildNesting();

        protected ModuleCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
	    {
            var scope = new ModuleScope(Compiler);
            var moduleVar = scope.Module as ParameterExpression;
            Expression header = Assign(moduleVar, GetModule());
            header = CallFrame.Expressions.Push(header);

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
