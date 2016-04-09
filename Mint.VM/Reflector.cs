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
        public static MethodInfo Method<TResult>(Expression<Func<T, TResult>> expr) => ((MethodCallExpression) expr.Body).Method;

        public static PropertyInfo Property<TResult>(Expression<Func<T, TResult>> expr) => (PropertyInfo) ((MemberExpression) expr.Body).Member;

        public static MethodInfo Operator<TResult>(Expression<Func<T, TResult>> expr)
            => expr.Body is BinaryExpression ? ((BinaryExpression) expr.Body).Method
               : ((UnaryExpression) expr.Body).Method;
    }
}
