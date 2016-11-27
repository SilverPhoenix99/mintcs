using Mint.Reflection;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Mint
{
    public class TypeError : Exception
    {
        public TypeError()
        { }

        public TypeError(string message) : base(message)
        { }

        public TypeError(string message, Exception innerException) : base(message, innerException)
        { }

        public static class Reflection
        {
            public static readonly ConstructorInfo Ctor = Reflector<TypeError>.Ctor<string>();
        }

        public static class Expressions
        {
            public static NewExpression New(Expression message) => Expression.New(Reflection.Ctor, message);
        }
    }
}