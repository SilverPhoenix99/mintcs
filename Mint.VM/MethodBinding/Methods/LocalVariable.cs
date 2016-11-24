using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.Methods
{
    public class LocalVariable
    {
        public iObject Value { get; set; }

        public Symbol Name { get; }

        public LocalVariable(Symbol name, iObject value = null)
        {
            Name = name;
            Value = value;
        }

        public static class Reflection
        {
            public static readonly PropertyInfo Value = Reflector<LocalVariable>.Property(_ => _.Value);

            public static readonly ConstructorInfo Ctor =
                Reflector.Ctor<LocalVariable>(typeof(Symbol), typeof(iObject));
        }

        public static class Expressions
        {
            public static MemberExpression Value(Expression localVariable) =>
                Property(localVariable, Reflection.Value);

            public static NewExpression New(Expression name, Expression value = null) =>
                Expression.New(Reflection.Ctor, name, value ?? Constant(null, typeof(iObject)));
        }
    }
}
