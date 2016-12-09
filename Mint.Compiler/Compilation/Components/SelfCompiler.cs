using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class SelfCompiler : CompilerComponentBase
    {
        public SelfCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile() => Compiler.CurrentScope.Instance;
    }
}