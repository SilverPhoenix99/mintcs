using Mint.MethodBinding.Arguments;
using Mint.MethodBinding.Compilation;
using Mint.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Mint.MethodBinding
{
    public delegate iObject Function(iObject instance, params iObject[] arguments);

    public sealed class CallSite
    {
        public delegate iObject Stub(iObject instance, ArgumentBundle bundle);

        public Visibility Visibility { get; }
        public Symbol MethodName { get; }
        public IList<ArgumentKind> ArgumentKinds { get; }

        private Arity arity;
        public Arity Arity
        {
            get
            {
                if(arity == null)
                {
                    var min = ArgumentKinds.Count(a => a == ArgumentKind.Simple);
                    if(ArgumentKinds.Any(a => a == ArgumentKind.Key || a == ArgumentKind.KeyRest))
                    {
                        min++;
                    }
                    var max = ArgumentKinds.Any(a => a == ArgumentKind.Rest) ? int.MaxValue : min;
                    arity = new Arity(min, max);
                }
                return arity;
            }
        }

        public CallCompiler CallCompiler { get; set; }
        public Function Call { get; set; }

        public CallSite(Symbol methodName, Visibility visibility, params ArgumentKind[] argumentKinds)
        {
            MethodName = methodName;
            Visibility = visibility;
            ArgumentKinds = System.Array.AsReadOnly(argumentKinds);
            Call = DefaultCall;
        }

        public CallSite(Symbol methodName, Visibility visibility, IEnumerable<ArgumentKind> argumentKinds)
            : this(methodName, visibility, argumentKinds?.ToArray() ?? System.Array.Empty<ArgumentKind>())
        { }

        public CallSite(Symbol methodName, params ArgumentKind[] argumentKinds)
            : this(methodName, Visibility.Public, argumentKinds)
        { }

        public CallSite(Symbol methodName, IEnumerable<ArgumentKind> argumentKinds)
            : this(methodName, argumentKinds?.ToArray() ?? System.Array.Empty<ArgumentKind>())
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
            var argumentKinds = string.Join(", ", ArgumentKinds.Select(_ => _.Description));
            return $"CallSite<\"{MethodName}\"<{Arity}>({argumentKinds})>";
        }

        public ArgumentBundle CreateBundle(params iObject[] arguments) => new ArgumentBundle(ArgumentKinds, arguments);
    }
}
