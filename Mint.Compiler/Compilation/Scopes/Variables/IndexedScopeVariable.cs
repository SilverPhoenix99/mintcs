using System.Linq.Expressions;
using Mint.MethodBinding;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Scopes.Variables
{
    public class IndexedScopeVariable : BaseScopeVariable
    {
        public IndexedScopeVariable(Scope scope, Symbol name)
            : base(scope, name, Variable(typeof(LocalVariable), name.Name))
        { }

        protected override Expression CompileInitialization() =>
            // $Local = CallFrame.Current.Locals[@Name]
            Assign(
                Local,
                CallFrame.Expressions.LocalsIndexer(CallFrame.Expressions.Current(), Constant(Name))
            );
    }
}
