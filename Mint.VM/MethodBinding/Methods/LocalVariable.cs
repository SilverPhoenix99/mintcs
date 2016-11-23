using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;

namespace Mint.MethodBinding.Methods
{
    public class LocalVariable
    {
        public Symbol Name { get; }

        public iObject Value { get; set; }

        public LocalVariable(Symbol name, iObject value = null)
        {
            Name = name;
            Value = value;
        }

        public static class Reflection
        {
            public static readonly PropertyInfo Value = Reflector<LocalVariable>.Property(_ => _.Value);
        }

        public static class Expressions
        {
            public static MemberExpression Value(Expression localVariable) =>
                Expression.Property(localVariable, Reflection.Value);
        }
    }
}
