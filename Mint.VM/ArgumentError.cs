using Mint.Reflection;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Mint
{
    public class ArgumentError : Exception
    {
        public ArgumentError()
        { }

        public ArgumentError(string message) : base(message)
        { }

        public ArgumentError(string message, Exception innerException) : base(message, innerException)
        { }

        public static class Reflection
        {
            public static readonly ConstructorInfo Ctor_ArgumentError = Reflector.Ctor<ArgumentError>(typeof(string));
        }

        public static class Expressions
        {
            public static NewExpression New(Expression message) =>
                Expression.New(Reflection.Ctor_ArgumentError, message);
        }
    }
}
