using Mint.Compilation.Components;
using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Selectors
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
}
