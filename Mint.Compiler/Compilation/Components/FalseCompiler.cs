using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    class FalseCompiler : CompilerComponentBase
    {
        public FalseCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce() => CompilerUtils.FALSE;
    }
}
