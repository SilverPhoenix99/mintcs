using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;
using static Mint.MethodBinding.Arguments.ArgumentKind;

namespace Mint.MethodBinding.Arguments
{
    public class ArgumentBundle
    {
        private Hash keywords;

        
        public ArgumentBundle(IList<ArgumentKind> kinds, params iObject[] arguments)
        {
            ArgumentKinds = kinds;
            Splat = new List<iObject>();
            Block = null;

            if(arguments.Length == 0)
            {
                return;
            }

            if(arguments.Length != ArgumentKinds.Count)
            {
                throw new ArgumentException("number of arguments does not match.");
            }

            for(var i = 0; i < arguments.Length; i++)
            {
                var kind = kinds[i];
                var arg = arguments[i];
                kind.Bundle(arg, this);
            }
        }


        public IList<ArgumentKind> ArgumentKinds { get; }
        public IList<iObject> Splat { get; set; }
        public iObject Block { get; set; }
        public int Arity => Splat.Count;
        public bool HasKeyArguments => ArgumentKinds.Any(kind => kind == Key || kind == KeyRest);
        
        public Hash Keywords
        {
            get => keywords ?? (keywords = GetOrCreateKeysArgument());
            set => keywords = value;
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


        public iObject[] Bind(MethodMetadata method)
        {
            var validator = new ArityValidator(this, method);
            validator.Validate();
            var binders = method.GetParameterBinders();
            return binders.Select(binder => binder.Bind(this)).ToArray();
        }


        public iObject[] TryBind(MethodMetadata method)
        {
            var validator = new ArityValidator(this, method);
            if(!validator.IsValid())
            {
                return null;
            }

            var binders = method.GetParameterBinders();
            return binders.Select(binder => binder.Bind(this)).ToArray();
        }


        public static class Reflection
        {
            public static readonly MethodInfo Bind = Reflector<ArgumentBundle>.Method(
                _ => _.Bind(default(MethodMetadata))
            );

            public static readonly MethodInfo TryBind = Reflector<ArgumentBundle>.Method(
                _ => _.TryBind(default(MethodMetadata))
            );
        }


        public static class Expressions
        {
            public static MethodCallExpression Bind(Expression argumentBundle, Expression methodInfo)
                => Expression.Call(argumentBundle, Reflection.Bind, methodInfo);


            public static MethodCallExpression TryBind(Expression argumentBundle, Expression methodInfo)
                => Expression.Call(argumentBundle, Reflection.TryBind, methodInfo);
        }
    }
}