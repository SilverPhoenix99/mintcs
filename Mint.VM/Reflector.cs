using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mint
{
    public static class Reflector
    {
        public static ConstructorInfo Ctor<T>(params Type[] argTypes) => typeof(T).GetConstructor(argTypes);

        public static MethodInfo Method<TResult>(Expression<Func<TResult>> lambda) => Method((LambdaExpression) lambda);

        public static MethodInfo Operator<TResult>(Expression<Func<TResult>> lambda) => Operator((LambdaExpression) lambda);

        public static MethodInfo Convert<TResult>(Expression<Func<TResult>> lambda) => Convert((LambdaExpression) lambda);

        public static PropertyInfo Property<TResult>(Expression<Func<TResult>> lambda) => Property((LambdaExpression) lambda);

        internal static MethodInfo Method(LambdaExpression lambda)
        {
            var body = (MethodCallExpression) Body(lambda);
            var type = body.Object.Type;
            var method = body.Method;
            return DeclaringMethod(method, type);
        }

        internal static MethodInfo Operator(LambdaExpression lambda)
        {
            var body = Body(lambda);

            Type type;
            MethodInfo method;
            var bin = body as BinaryExpression;
            if(bin != null)
            {
                type = bin.Type;
                method = bin.Method;
            }
            else
            {
                var unary = body as UnaryExpression;
                type = unary.Type;
                method = unary.Method;
            }

            return DeclaringMethod(method, type);
        }

        internal static MethodInfo Convert(LambdaExpression lambda)
        {
            var body = (UnaryExpression) lambda.Body;
            var type = body.Type;
            var method = body.Method;
            return DeclaringMethod(method, type);
        }

        internal static PropertyInfo Property(LambdaExpression lambda)
        {
            var body = Body(lambda);

            var member = body as MemberExpression;
            if(member != null)
            {
                var type = member.Type;

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

        internal static Expression Body(LambdaExpression lambda)
        {
            var body = lambda.Body;
            var convert = body as UnaryExpression;
            return convert?.NodeType == ExpressionType.Convert
                ? convert.Operand
                : body;
        }

        internal static MethodInfo DeclaringMethod(MethodInfo method, Type declaringType)
        {
            var flags = BindingFlags.Default;
            flags |= method.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
            flags |= method.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic;
            var parameters = method.GetParameters().Select(_ => _.ParameterType).ToArray();
            return declaringType.GetMethod(method.Name, flags, null, parameters, null);
        }
    }

    public static class Reflector<T>
    {
        public static MethodInfo Method<TResult>(Expression<Func<T, TResult>> lambda) => Reflector.Method(lambda);

        public static MethodInfo Operator<TResult>(Expression<Func<T, TResult>> lambda) => Reflector.Operator(lambda);

        public static MethodInfo Convert<TResult>(Expression<Func<T, TResult>> lambda) => Reflector.Convert(lambda);

        public static PropertyInfo Property<TResult>(Expression<Func<T, TResult>> lambda) => Reflector.Property(lambda);
    }
}
