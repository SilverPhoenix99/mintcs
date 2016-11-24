using System.Linq.Expressions;
using Mint.MethodBinding.Methods;

namespace Mint.Compilation.Scopes
{
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
