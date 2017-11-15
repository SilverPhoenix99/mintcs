using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class SimpleNameModuleCompiler : ModuleCompiler
    {
        protected override Expression Container => Compiler.CurrentScope.Module;

        public SimpleNameModuleCompiler(Compiler compiler) : base(compiler)
        { }

        protected override Expression GetModule() => Module.Expressions.GetOrCreateModule(Container, Name, Nesting);
    }
}
