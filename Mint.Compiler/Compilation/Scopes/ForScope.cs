using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public class ForScope : WhileScope
    {
        public ForScope(Compiler compiler) : base(compiler)
        { }
    }
}
