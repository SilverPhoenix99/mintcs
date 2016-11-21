using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public class ClassScope : ModuleScope
    {
        public ClassScope(Compiler compiler, Expression self) : base(compiler, self)
        { }
    }
}
