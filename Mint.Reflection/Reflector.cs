using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mint.Reflection
{
    public static class Reflector
    {
        public static ConstructorInfo Ctor<T>(params Type[] argTypes) => typeof(T).GetConstructor(argTypes);

        public static MethodInfo Method<TResult>(Expression<Func<TResult>> lambda) => Method((LambdaExpression) lambda);

        public static MethodInfo Operator<TResult>(Expression<Func<TResult>> lambda) => Operator((LambdaExpression) lambda);

        public static MethodInfo Convert<TResult>(Expression<Func<TResult>> lambda) => Convert((LambdaExpression) lambda);

        public static PropertyInfo Property<TResult>(Expression<Func<TResult>> lambda) => Property((LambdaExpression) lambda);

        public static MethodInfo Getter<TResult>(Expression<Func<TResult>> lambda) => Getter((LambdaExpression) lambda);

        public static MethodInfo Setter<TResult>(Expression<Func<TResult>> lambda) => Setter((LambdaExpression) lambda);

        internal static MethodInfo Method(LambdaExpression lambda)
        {
            var body = (MethodCallExpression) Body(lambda);
            var type = body.Object?.Type;
            var method = body.Method;
            return type == null ? method : (DeclaringMethod(method, type) ?? method);
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
                var property = (PropertyInfo) member.Member;
                return DeclaringProperty(property, member.Expression.Type);
            }

            // probably an indexer

            var call = (MethodCallExpression) body;
            var method = DeclaringMethod(call.Method, call.Object.Type);
            var properties = method.DeclaringType.GetProperties(
                BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            return properties.Single(p => p.GetMethod == method || p.SetMethod == method);
        }

        internal static MethodInfo Getter(LambdaExpression lambda) => Property(lambda).GetMethod;

        internal static MethodInfo Setter(LambdaExpression lambda) => Property(lambda).SetMethod;

        private static Expression Body(LambdaExpression lambda)
        {
            var body = lambda.Body;
            var convert = body as UnaryExpression;
            return convert?.NodeType == ExpressionType.Convert
                ? convert.Operand
                : body;
        }

        private static MethodInfo DeclaringMethod(MethodInfo method, Type declaringType)
        {
            var flags = BindingFlags.Default;
            flags |= method.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
            flags |= method.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic;
            var parameters = method.GetParameters().Select(_ => _.ParameterType).ToArray();
            return declaringType.GetMethod(method.Name, flags, null, parameters, null);
        }

        private static PropertyInfo DeclaringProperty(PropertyInfo property, Type declaringType)
        {
            var method = property.GetMethod ?? property.SetMethod;
            var flags = BindingFlags.Default;
            flags |= method.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
            flags |= method.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic;
            var parameters = property.GetIndexParameters().Select(_ => _.ParameterType).ToArray();
            return declaringType.GetProperty(property.Name, flags, null, property.PropertyType, parameters, null);
        }
    }

    public static class Reflector<T>
    {
        public static MethodInfo Method<TResult>(Expression<Func<T, TResult>> lambda) => Reflector.Method(lambda);

        public static MethodInfo Method(Expression<Action<T>> lambda) => Reflector.Method(lambda);

        public static MethodInfo Operator<TResult>(Expression<Func<T, TResult>> lambda) => Reflector.Operator(lambda);

        public static MethodInfo Convert<TResult>(Expression<Func<T, TResult>> lambda) => Reflector.Convert(lambda);

        public static PropertyInfo Property<TResult>(Expression<Func<T, TResult>> lambda) => Reflector.Property(lambda);

        public static MethodInfo Getter<TResult>(Expression<Func<T, TResult>> lambda) => Reflector.Getter(lambda);

        public static MethodInfo Setter<TResult>(Expression<Func<T, TResult>> lambda) => Reflector.Setter(lambda);
    }
}
