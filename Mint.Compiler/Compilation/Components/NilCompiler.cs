using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    class NilCompiler : CompilerComponentBase
    {
        public NilCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce() => CompilerUtils.NIL;
    }
}
