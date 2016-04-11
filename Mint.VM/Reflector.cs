using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mint
{
    public static class Reflector
    {
        public static ConstructorInfo Ctor<T>(params Type[] argTypes) => typeof(T).GetConstructor(argTypes);

        public static MethodInfo Method<TResult>(Expression<Func<TResult>> lambda) =>
            ((MethodCallExpression) Body(lambda)).Method;
        
        public static MethodInfo Operator<TResult>(Expression<Func<TResult>> lambda)
        {
            var body = Body(lambda);
            return (body as BinaryExpression)?.Method ?? ((UnaryExpression) body).Method;
        }

        public static MethodInfo Convert<TResult>(Expression<Func<TResult>> lambda) => ((UnaryExpression) lambda.Body).Method;

        public static PropertyInfo Property<TResult>(Expression<Func<TResult>> lambda)
        {
            var body = Body(lambda);

            var member = body as MemberExpression;
            if(member != null)
            {
                return (PropertyInfo) member.Member;
            }

            // probably indexer

            var method = (body as MethodCallExpression)?.Method;
            var properties = method?.DeclaringType?.GetProperties(
                BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            return (from p in properties
                    where p.GetMethod == method || p.SetMethod == method
                    select p).Single();
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
        
        public static MethodInfo Operator<TResult>(Expression<Func<T, TResult>> lambda)
        {
            var body = Body(lambda);
            return (body as BinaryExpression)?.Method ?? ((UnaryExpression) body).Method;
        }

        public static MethodInfo Convert<TResult>(Expression<Func<T, TResult>> lambda) => ((UnaryExpression) lambda.Body).Method;

        public static PropertyInfo Property<TResult>(Expression<Func<T, TResult>> lambda)
        {
            var body = Body(lambda);

            var member = body as MemberExpression;
            if(member != null)
            {
                return (PropertyInfo) member.Member;
            }

            // probably indexer

            var method = (body as MethodCallExpression)?.Method;
            var properties = typeof(T).GetProperties(
                BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            return (from p in properties
                    where p.GetMethod == method || p.SetMethod == method
                    select p).Single();
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
