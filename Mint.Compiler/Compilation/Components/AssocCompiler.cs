using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class AssocCompiler :CompilerComponentBase
    {
        public AssocCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var left = Pop();
            var right = Pop();
            return CompilerUtils.NewArray(left, right);
        }
    }
}
