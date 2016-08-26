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
        public IDictionary<iObject, iObject> Keys { get; set; }
        public iObject Block { get; set; }

        public ArgumentBundle(IList<ArgumentKind> kinds, params iObject[] arguments)
        {
            ArgumentKinds = kinds;
            Splat = new List<iObject>();
            Keys = new LinkedDictionary<iObject, iObject>();
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

        public iObject[] Unbundle(IEnumerable<ParameterBinder> binders) =>
            binders.Select(binder => binder.Bind(this)).ToArray();

        public class Expressions
        {
            private static readonly MethodInfo METHOD_UNBUNDLE = Reflector<ArgumentBundle>.Method(
                _ => _.Unbundle(default(IEnumerable<ParameterBinder>))
            );

            public static Expression CallUnbundle(Expression argumentBundle, Expression parameterBinders) =>
                Expression.Call(argumentBundle, METHOD_UNBUNDLE, parameterBinders);
        }
    }
}