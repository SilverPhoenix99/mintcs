using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class TrueCompiler : CompilerComponentBase
    {
        public TrueCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce() => TrueClass.Expressions.Instance;
    }
}
