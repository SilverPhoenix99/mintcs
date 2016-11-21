using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public class BlockScope : MethodScope
    {
        public BlockScope(Compiler compiler, Expression self) : base(compiler, self)
        { }
    }
}
