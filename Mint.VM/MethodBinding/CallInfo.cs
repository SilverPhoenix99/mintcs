using System;
using System.Collections.Generic;
using System.Linq;

namespace Mint.MethodBinding
{
    public class CallInfo
    {
        public Visibility Visibility { get; }
        public Symbol MethodName { get; }
        public ArgumentKind[] Arguments { get; }
        public int Arity => Arguments.Length;

        public CallInfo(Symbol methodName, Visibility visibility = Visibility.Public, IEnumerable<ArgumentKind> arguments = null)
        {
            MethodName = methodName;
            Visibility = visibility;
            Arguments = arguments?.ToArray() ?? new ArgumentKind[0];
        }

        public override string ToString()
        {
            var parameters = string.Join(", ", Arguments.Select(_ => _.Description));
            return $"\"{MethodName}\"<{Arity}>({parameters})";
        }

        public ArgumentBundle Bundle(params iObject[] arguments)
        {
            if(arguments.Length != Arguments.Length) throw new ArgumentException();

            var bundle = new ArgumentBundle();

            for(var i = 0; i < arguments.Length; i++)
            {
                Arguments[i].Bundle(arguments[i], bundle);
            }

            return bundle;
        }
    }
}
