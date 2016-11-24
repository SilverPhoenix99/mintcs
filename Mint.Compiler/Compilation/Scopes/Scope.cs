using System.Collections.Generic;
using System.Linq.Expressions;
using Mint.MethodBinding.Methods;

namespace Mint.Compilation.Scopes
{
    public interface Scope
    {
        Scope Parent { get; }

        Expression Nesting { get; }

        Expression CallFrame { get; set; }

        MemberExpression Self { get; }

        IDictionary<Symbol, ScopeVariable> Variables { get; }
    }

    public class ScopeVariable
    {
        public readonly Symbol Name;
        public readonly int Index;
        public readonly ParameterExpression Local;
        public readonly Expression InitialValue;

        public ScopeVariable(Symbol name, int index, ParameterExpression local = null, Expression initialValue = null)
        {
            Name = name;
            Index = index;
            Local = local ?? Expression.Variable(typeof(LocalVariable), name.Name);
            InitialValue = initialValue;
        }
    }
}
