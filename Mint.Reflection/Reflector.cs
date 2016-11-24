using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Reflection.BindingFlags;

namespace Mint.Reflection
{
    public static class Reflector
    {
        public static ConstructorInfo Ctor<T>(params Type[] argTypes) => typeof(T).GetConstructor(argTypes);

        public static MethodInfo Method<TResult>(Expression<Func<TResult>> lambda) =>
            Method((LambdaExpression) lambda);

        public static MethodInfo Method(Expression<Action> lambda) => Method((LambdaExpression) lambda);

        public static MethodInfo Operator<TResult>(Expression<Func<TResult>> lambda) =>
            Operator((LambdaExpression) lambda);

        public static MethodInfo Convert<TResult>(Expression<Func<TResult>> lambda) =>
            Convert((LambdaExpression) lambda);

        public static PropertyInfo Property<TResult>(Expression<Func<TResult>> lambda) =>
            Property((LambdaExpression) lambda);

        public static MethodInfo Getter<TResult>(Expression<Func<TResult>> lambda) =>
            Getter((LambdaExpression) lambda);

        public static MethodInfo Setter<TResult>(Expression<Func<TResult>> lambda) =>
            Setter((LambdaExpression) lambda);

        public static FieldInfo Field<TResult>(Expression<Func<TResult>> lambda) =>
            Field((LambdaExpression) lambda);

        public static MethodInfo Method(LambdaExpression lambda)
        {
            var body = (MethodCallExpression) Body(lambda);
            var type = body.Object?.Type;
            var method = body.Method;
            return type == null ? method : (DeclaringMethod(method, type) ?? method);
        }

        public static MethodInfo Operator(LambdaExpression lambda)
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

        public static MethodInfo Convert(LambdaExpression lambda)
        {
            var body = (UnaryExpression) lambda.Body;
            var type = body.Type;
            var method = body.Method;
            return DeclaringMethod(method, type);
        }

        public static PropertyInfo Property(LambdaExpression lambda)
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
                Static | Instance | Public | NonPublic);

            return properties.Single(p => p.GetMethod == method || p.SetMethod == method);
        }

        public static MethodInfo Getter(LambdaExpression lambda) => Property(lambda).GetMethod;

        public static MethodInfo Setter(LambdaExpression lambda) => Property(lambda).SetMethod;

        public static FieldInfo Field(LambdaExpression lambda)
        {
            var body = Body(lambda);

            var member = body as MemberExpression;
            return member?.Member as FieldInfo;
        }

        private static Expression Body(LambdaExpression lambda)
        {
            var body = lambda.Body;
            return body.NodeType == ExpressionType.Convert
                ? ((UnaryExpression) body).Operand
                : body;
        }

        private static MethodInfo DeclaringMethod(MethodInfo method, IReflect declaringType)
        {
            var flags = Default;
            flags |= method.IsStatic ? Static : Instance;
            flags |= method.IsPublic ? Public : NonPublic;
            var parameters = method.GetParameters().Select(_ => _.ParameterType).ToArray();
            return declaringType.GetMethod(method.Name, flags, null, parameters, null) ?? method;

        }

        private static PropertyInfo DeclaringProperty(PropertyInfo property, IReflect declaringType)
        {
            var method = property.GetMethod ?? property.SetMethod;
            var flags = Default;
            flags |= method.IsStatic ? Static : Instance;
            flags |= method.IsPublic ? Public : NonPublic;
            var parameters = property.GetIndexParameters().Select(_ => _.ParameterType).ToArray();
            return declaringType.GetProperty(property.Name, flags, null, property.PropertyType, parameters, null) ?? property;
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

        public static FieldInfo Field<TResult>(Expression<Func<T, TResult>> lambda) => Reflector.Field(lambda);
    }
}
