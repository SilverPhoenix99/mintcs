using System;
using System.Linq.Expressions;
using Mint.Compilation.Selectors;
using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
	internal class ModuleSelector : ComponentSelectorBase
	{
		private ModuleCompiler simpleName;
		private ModuleCompiler relativeName;
		private ModuleCompiler absoluteName;

        private Ast<Token> Name => Node[0];
		
		private ModuleCompiler SimpleName => simpleName ?? (simpleName = new SimpleNameModuleCompiler(Compiler));

        private ModuleCompiler AbsoluteName =>
            absoluteName ?? (absoluteName = new AbsoluteNameModuleCompiler(Compiler));

		private ModuleCompiler RelativeName =>
            relativeName ?? (relativeName = new RelativeNameModuleCompiler(Compiler));

        public ModuleSelector(Compiler compiler) : base(compiler)
	    { }

	    public override CompilerComponent Select()
	    {
            var type = Name.Value.Type;
            return type == kCOLON2 ? RelativeName
                 : type == kCOLON3 ? AbsoluteName
                 : SimpleName;
	    }
	}

    internal abstract class ModuleCompiler : CompilerComponentBase
    {
		protected Ast<Token> Name => Node[0];

        private Ast<Token> Body => Node[1];

        protected ModuleCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
            Push(Body);
        }
    }

	internal class SimpleNameModuleCompiler : ModuleCompiler
	{
	    public SimpleNameModuleCompiler(Compiler compiler) : base(compiler)
	    { }

        public override Expression Reduce()
	    {
			var body = Pop();

            // TODO

	        throw new NotImplementedException();
	    }
	}

    internal class AbsoluteNameModuleCompiler : ModuleCompiler
    {
        public AbsoluteNameModuleCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            // TODO

            throw new NotImplementedException();
        }
    }

    internal class RelativeNameModuleCompiler : ModuleCompiler
    {
        public RelativeNameModuleCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
			Push(Name[0]);
            base.Shift();
        }

        public override Expression Reduce()
        {
            // TODO

            throw new NotImplementedException();
        }
    }
}
