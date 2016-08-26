using Mint.MethodBinding.Arguments;
using Mint.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mint.MethodBinding
{
    public class CallInfo
    {
        public Visibility Visibility { get; }
        public Symbol MethodName { get; }
        public IList<ArgumentKind> Arguments { get; }
        public int Arity => Arguments.Count;

        public CallInfo(Symbol methodName, Visibility visibility, params ArgumentKind[] arguments)
        {
            MethodName = methodName;
            Visibility = visibility;
            Arguments = System.Array.AsReadOnly(arguments);
        }

        public CallInfo(Symbol methodName, params ArgumentKind[] arguments)
            : this(methodName, Visibility.Public, arguments)
        { }

        public CallInfo(Symbol methodName, Visibility visibility, IEnumerable<ArgumentKind> arguments = null)
            : this(methodName, visibility, arguments?.ToArray() ?? System.Array.Empty<ArgumentKind>())
        { }

        public CallSite CreateSite() => new CallSite(this);

        public override string ToString()
        {
            var parameters = string.Join(", ", Arguments.Select(_ => _.Description));
            return $"\"{MethodName}\"<{Arity}>({parameters})";
        }

        public ArgumentBundle Bundle(params iObject[] arguments)
        {
            if(arguments.Length != Arguments.Count) throw new ArgumentException();

            var bundle = new ArgumentBundle(Arguments);

            for(var i = 0; i < arguments.Length; i++)
            {
                Arguments[i].Bundle(arguments[i], bundle);
            }

            return bundle;
        }
    }
}
