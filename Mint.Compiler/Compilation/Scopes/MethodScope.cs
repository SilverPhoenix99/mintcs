using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public class MethodScope : LoopScope
    {
        public override CompilerClosure Closure { get; }

        public MethodScope(Compiler compiler) : base(compiler)
        {
            Closure = new CompilerClosure();
        }
    }
}
