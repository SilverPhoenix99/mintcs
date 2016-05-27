using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class SelfCompiler : CompilerComponentBase
    {
        public SelfCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        { }

        public override Expression Reduce() => Expression.Constant(Compiler.CurrentScope.Closure.Self);
    }
}