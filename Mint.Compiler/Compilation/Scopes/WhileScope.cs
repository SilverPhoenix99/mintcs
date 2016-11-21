using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public class WhileScope : MethodScope
    {
        public WhileScope(Compiler compiler) : base(compiler, compiler.CurrentScope.Self)
        { }
    }
}
