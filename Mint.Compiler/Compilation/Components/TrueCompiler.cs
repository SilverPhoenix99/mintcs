using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    class TrueCompiler : CompilerComponentBase
    {
        public TrueCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce() => CompilerUtils.TRUE;
    }
}
