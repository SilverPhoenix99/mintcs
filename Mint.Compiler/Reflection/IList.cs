using System.Linq.Expressions;

namespace Mint.Reflection
{
    internal static class IList<T>
    {
        public static class Reflection
        {
            public static readonly System.Reflection.ConstructorInfo Ctor =
                Reflector.Ctor<System.Collections.Generic.IList<T>>();
        }

        public static class Expressions
        {
            public static NewExpression New() => Expression.New(Reflection.Ctor);
        }
    }
}
