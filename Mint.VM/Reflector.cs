using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mint
{
    public static class Reflector
    {
        public static ConstructorInfo Ctor<T>(params Type[] argTypes) => typeof(T).GetConstructor(argTypes);
    }

    public static class Reflector<T>
    {
        public static MethodInfo Method<TResult>(Expression<Func<T, TResult>> expr) => (expr.Body as MethodCallExpression)?.Method;

        public static PropertyInfo Property<TResult>(Expression<Func<T, TResult>> expr)
            => (expr.Body as MemberExpression)?.Member as PropertyInfo;
    }
}
