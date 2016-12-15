using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public class ClassScope : ModuleScope
    {
        public ClassScope(Compiler compiler) : base(compiler, Expression.Variable(typeof(Class), "class"))
        { }
    }
}
