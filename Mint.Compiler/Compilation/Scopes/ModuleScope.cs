using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public class ModuleScope : MethodScope
    {
        public override Expression Nesting { get; }

        public override Expression Instance => Module.Cast<iObject>();

        public override Expression Module { get; }

        public ModuleScope(Compiler compiler) : base(compiler)
        {
            Module = Expression.Variable(typeof(Module), "module");
            Nesting = Expression.Variable(typeof(IList<Module>), "nesting");
        }
    }
}
