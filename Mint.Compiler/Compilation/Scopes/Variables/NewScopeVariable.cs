using System.Linq.Expressions;
using Mint.MethodBinding;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Scopes.Variables
{
    public class NewScopeVariable : BaseScopeVariable
    {
        public NewScopeVariable(Scope scope,
                                Symbol name,
                                ParameterExpression local = null)
            : base(scope, name, local ?? Variable(typeof(LocalVariable), name.Name))
        { }

        protected override Expression CompileInitialization() =>
            // $Local = CallFrame.Current.AddLocal(new LocalVariable(@Name))
            Assign(
                Local,
                CallFrame.Expressions.AddLocal(
                    CallFrame.Expressions.Current(),
                    LocalVariable.Expressions.New(Constant(Name))
                )
            );
    }
}
