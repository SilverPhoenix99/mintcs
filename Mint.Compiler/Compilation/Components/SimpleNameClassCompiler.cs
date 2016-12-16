using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class SimpleNameClassCompiler : ClassCompiler
    {
        protected virtual Expression Container => Compiler.CurrentScope.Module;

        public SimpleNameClassCompiler(Compiler compiler) : base(compiler)
        { }

        protected override Expression GetClass() =>
            Module.Expressions.GetOrCreateClass(Container, Name, CompileSuperclass(), Nesting);
    }
}
