using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class SplatCompiler :CompilerComponentBase
    {
        public SplatCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce() => Pop();
    }
}
