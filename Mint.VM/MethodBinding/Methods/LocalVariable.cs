using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.Methods
{
    public class LocalVariable
    {
        private iObject value;

        public iObject Value
        {
            get { return value; }
            set { this.value = value ?? new NilClass(); }
        }

        public Symbol Name { get; }

        public LocalVariable(Symbol name, iObject value)
        {
            Name = name;
            Value = value;
        }

        public LocalVariable(Symbol name) : this(name, null)
        { }

        public static class Reflection
        {
            public static readonly PropertyInfo Value = Reflector<LocalVariable>.Property(_ => _.Value);

            public static readonly ConstructorInfo Ctor1 = Reflector<LocalVariable>.Ctor<Symbol>();

            public static readonly ConstructorInfo Ctor2 = Reflector<LocalVariable>.Ctor<Symbol, iObject>();
        }

        public static class Expressions
        {
            public static MemberExpression Value(Expression localVariable) =>
                Property(localVariable, Reflection.Value);

            public static NewExpression New(Expression name) =>
                Expression.New(Reflection.Ctor1, name);

            public static NewExpression New(Expression name, Expression value) =>
                Expression.New(Reflection.Ctor2, name, value ?? Constant(null, typeof(iObject)));
        }
    }
}
