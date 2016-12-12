using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Mint.Compilation.Scopes;
using Mint.MethodBinding.Methods;
using Mint.Parse;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal abstract class ModuleCompiler : CompilerComponentBase
    {
		protected Ast<Token> Name => Node[0];

        protected Ast<Token> Body => Node[1];

        protected ModuleCompiler(Compiler compiler) : base(compiler)
        { }
    }

	internal class SimpleNameModuleCompiler : ModuleCompiler
	{
        protected virtual Expression Container => CallFrame.Expressions.Module(CallFrame.Expressions.Current());

	    public SimpleNameModuleCompiler(Compiler compiler) : base(compiler)
	    { }

        public override Expression Compile()
	    {
            var name = new Symbol(Name.Value.Value);

            Func<Module, IEnumerable<Module>, Module> f = (mod, nesting) => {
                var constant = mod.TryGetConstant(name, nesting);

                if(constant == null)
                {
                    constant = mod.SetConstant(name, new Module(name, null));
                }
                else if(!(constant is Module))
                {
                    throw new TypeError(constant.Inspect() + " is not a module");
                }

                return (Module) constant;
            };

            var scope = new ModuleScope(Compiler);
            var module = scope.Module as ParameterExpression;
            var header = Assign(module, Invoke(Constant(f), Container, Compiler.CurrentScope.Nesting));

            Compiler.CurrentScope = scope;

            try
            {
                var body = Body.Accept(Compiler);
                return scope.CompileBody(Expression.Block(header, body));
            }
            finally
            {
                Compiler.EndScope();
            }
	    }
	}

    internal class AbsoluteNameModuleCompiler : ModuleCompiler
    {
        public AbsoluteNameModuleCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            try
            {
                // TODO

                Compiler.CurrentScope = new ModuleScope(Compiler);
            }
            finally
            {
                Compiler.EndScope();
            }

            throw new NotImplementedException();
        }
    }

    internal class RelativeNameModuleCompiler : ModuleCompiler
    {
        public RelativeNameModuleCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            try
            {
                // TODO

                Compiler.CurrentScope = new ModuleScope(Compiler);
            }
            finally
            {
                Compiler.EndScope();
            }

            throw new NotImplementedException();
        }
    }
}
