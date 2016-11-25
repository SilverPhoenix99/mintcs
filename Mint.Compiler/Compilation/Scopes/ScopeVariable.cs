using System.Linq.Expressions;
using Mint.MethodBinding.Methods;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Scopes
{
    public interface ScopeVariable
    {
        Scope Scope { get; }

        Symbol Name { get; }

        int Index { get; }

        ParameterExpression Local { get; }

        Expression GenerateLocal();
    }

    public abstract class BaseScopeVariable : ScopeVariable
    {
        public Scope Scope { get; }

        public Symbol Name { get; }

        public int Index { get; }

        public ParameterExpression Local { get; }

        protected BaseScopeVariable(Scope scope, Symbol name, int index, ParameterExpression local)
        {
            Scope = scope;
            Name = name;
            Index = index;
            Local = local;
        }

        public abstract Expression GenerateLocal();
    }

    public class NewScopeVariable : BaseScopeVariable
    {
        public Expression InitialValue { get; }

        public NewScopeVariable(Scope scope,
                                Symbol name,
                                int index,
                                ParameterExpression local = null,
                                Expression initialValue = null)
            : base(scope, name, index, local ?? Expression.Variable(typeof(LocalVariable), name.Name))
        {
            InitialValue = initialValue ?? Constant(null, typeof(iObject));
        }

        public override Expression GenerateLocal()
        {
            var localVariable = LocalVariable.Expressions.New(Constant(Name), InitialValue);
            return Scope.LocalsAdd(Assign(Local, localVariable));
        }
    }

    public class ReferencedScopeVariable : BaseScopeVariable
    {
        public ReferencedScopeVariable(Scope scope, ScopeVariable baseVariable, int index)
            : base(scope, baseVariable.Name, index, baseVariable.Local)
        { }

        public override Expression GenerateLocal() => Scope.LocalsAdd(Local);
    }
}
