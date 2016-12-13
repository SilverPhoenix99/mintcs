using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class FalseCompiler : CompilerComponentBase
    {
        public FalseCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile() => FalseClass.Expressions.Instance;
    }
}
