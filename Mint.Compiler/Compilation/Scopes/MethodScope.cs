using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public class MethodScope : LoopScope
    {
        public override Expression Instance { get; }

        public MethodScope(Compiler compiler) : base(compiler, null)
        {
            Instance = Expression.Parameter(typeof(iObject), "instance");
        }
    }
}
