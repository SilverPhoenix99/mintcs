using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public class ModuleScope : MethodScope
    {
        public override Expression Nesting { get; }

        public ModuleScope(Compiler compiler) : base(compiler)
        {
            Nesting = Expression.Variable(typeof(IList<Module>), "nesting");
        }
    }
}
