using System;
using System.Linq.Expressions;
using Mint.Compilation.Scopes;
using Mint.Parse;

namespace Mint.Compilation.Components
{
    internal abstract class ModuleCompiler : CompilerComponentBase
    {
		protected Ast<Token> Name => Node[0];

        private Ast<Token> Body => Node[1];

        protected ModuleCompiler(Compiler compiler) : base(compiler)
        { }
    }

	internal class SimpleNameModuleCompiler : ModuleCompiler
	{
	    public SimpleNameModuleCompiler(Compiler compiler) : base(compiler)
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
