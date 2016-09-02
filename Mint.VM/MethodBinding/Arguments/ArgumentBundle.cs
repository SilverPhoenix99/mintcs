using Mint.MethodBinding.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;

namespace Mint.MethodBinding.Arguments
{
    public class ArgumentBundle
    {
        public IList<ArgumentKind> ArgumentKinds { get; }

        public IList<iObject> Splat { get; set; }

        private Hash keywords;
        public Hash Keywords
        {
            get { return keywords ?? (keywords = GetOrCreateKeysArgument()); }
            set { keywords = value; }
        }

        public iObject Block { get; set; }

        public int Arity => Splat.Count;

        public bool HasKeyArguments =>
            ArgumentKinds.Any(kind => kind == ArgumentKind.Key || kind == ArgumentKind.KeyRest);

        public ArgumentBundle(IList<ArgumentKind> kinds, params iObject[] arguments)
        {
            ArgumentKinds = kinds;
            Splat = new List<iObject>();
            Keywords = new Hash();
            Block = null;

            if(arguments.Length == 0)
            {
                return;
            }

            if(arguments.Length != ArgumentKinds.Count)
            {
                throw new ArgumentException("number of arguments does not match.");
            }

            var zippedArgs = ArgumentKinds.Zip(arguments, (kind, value) => new { Kind = kind, Value = value });

            foreach(var argument in zippedArgs)
            {
                argument.Kind.Bundle(argument.Value, this);
            }
        }

        private Hash GetOrCreateKeysArgument()
        {
            if(!HasKeyArguments)
            {
                // create empty stub if needed,
                // but don't add it to Splat, since it affects Arity.
                return Splat.LastOrDefault() as Hash ?? new Hash();
            }

            var keys = new Hash();
            Splat.Add(keys);
            return keys;
        }

        public iObject[] Bind(MethodInfo methodInfo)
        {
            var validator = new ArityValidator(this, methodInfo);
            if(!validator.IsValid())
            {
                return null;
            }

            var binders = methodInfo.GetParameterBinders();
            return binders.Select(binder => binder.Bind(this)).ToArray();
        }

        public static class Reflection
        {
            public static readonly MethodInfo Bind = Reflector<ArgumentBundle>.Method(
                _ => _.Bind(default(MethodInfo))
            );
        }

        public static class Expressions
        {
            public static MethodCallExpression Bind(Expression argumentBundle, Expression methodInfo) =>
                Expression.Call(argumentBundle, Reflection.Bind, methodInfo);
        }
    }
}