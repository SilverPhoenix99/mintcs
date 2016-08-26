using Mint.MethodBinding.Arguments;
using Mint.MethodBinding.Compilation;
using System.Collections.Generic;
using System.Linq;

namespace Mint.MethodBinding
{
    public delegate iObject Function(iObject instance, params iObject[] arguments);

    public sealed class CallSite
    {
        public Visibility Visibility { get; }
        public Symbol MethodName { get; }
        public IList<ArgumentKind> Arguments { get; }
        public int Arity => Arguments.Count;
        public CallCompiler CallCompiler { get; set; }
        public Function Call { get; set; }

        public CallSite(Symbol methodName, Visibility visibility, params ArgumentKind[] arguments)
        {
            MethodName = methodName;
            Visibility = visibility;
            Arguments = System.Array.AsReadOnly(arguments);
            Call = DefaultCall;
        }

        public CallSite(Symbol methodName, Visibility visibility, IEnumerable<ArgumentKind> arguments)
            : this(methodName, visibility, arguments?.ToArray() ?? System.Array.Empty<ArgumentKind>())
        { }

        public CallSite(Symbol methodName, params ArgumentKind[] arguments)
            : this(methodName, Visibility.Public, arguments)
        { }

        public CallSite(Symbol methodName, IEnumerable<ArgumentKind> arguments)
            : this(methodName, arguments?.ToArray() ?? System.Array.Empty<ArgumentKind>())
        { }

        private iObject DefaultCall(iObject instance, iObject[] arguments)
        {
            if(CallCompiler == null)
            {
                CallCompiler = new PolymorphicCallCompiler(this);
            }
            Call = CallCompiler.Compile();
            return Call(instance, arguments);
        }

        public override string ToString()
        {
            var parameters = string.Join(", ", Arguments.Select(_ => _.Description));
            return $"CallSite<\"{MethodName}\"<{Arity}>({parameters})>";
        }

        public ArgumentBundle CreateBundle(params iObject[] arguments)
        {
            var bundle = new ArgumentBundle(Arguments);
            bundle.AddAll(arguments);
            return bundle;
        }
    }
}
