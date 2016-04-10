using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Mint
{
    public static class Reflector
    {
        public static ConstructorInfo Ctor<T>(params Type[] argTypes) => typeof(T).GetConstructor(argTypes);

        public static MethodInfo Method<TResult>(Expression<Func<TResult>> lambda) =>
            ((MethodCallExpression) Body(lambda)).Method;

        public static PropertyInfo Property<TResult>(Expression<Func<TResult>> lambda) =>
            (PropertyInfo) ((MemberExpression) Body(lambda)).Member;

        public static MethodInfo Operator<TResult>(Expression<Func<TResult>> lambda)
        {
            var body = Body(lambda);
            return (body as BinaryExpression)?.Method ?? ((UnaryExpression) body).Method;
        }

        private static Expression Body<TResult>(Expression<Func<TResult>> lambda)
        {
            var body = lambda.Body;
            var convert = body as UnaryExpression;
            return convert?.NodeType == ExpressionType.Convert
                ? convert.Operand
                : body;
        }
    }

    public static class Reflector<T>
    {
        public static MethodInfo Method<TResult>(Expression<Func<T,TResult>> lambda) =>
            ((MethodCallExpression) Body(lambda)).Method;

        public static PropertyInfo Property<TResult>(Expression<Func<T, TResult>> lambda) =>
            (PropertyInfo) ((MemberExpression) Body(lambda)).Member;

        public static MethodInfo Operator<TResult>(Expression<Func<T, TResult>> lambda)
        {
            var body = Body(lambda);
            return (body as BinaryExpression)?.Method ?? ((UnaryExpression) body).Method;
        }

        private static Expression Body<TResult>(Expression<Func<T, TResult>> lambda)
        {
            var body = lambda.Body;
            var convert = body as UnaryExpression;
            return convert?.NodeType == ExpressionType.Convert
                ? convert.Operand
                : body;
        }
    }
}
