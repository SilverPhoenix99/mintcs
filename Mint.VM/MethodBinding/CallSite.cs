using Mint.MethodBinding.Arguments;
using Mint.MethodBinding.Compilation;
using Mint.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mint.MethodBinding
{
    public sealed class CallSite
    {
        public delegate iObject Function(iObject instance, ArgumentBundle bundle);


        private Arity arity;


        public CallSite(Symbol methodName, Visibility visibility, params ArgumentKind[] argumentKinds)
        {
            MethodName = methodName;
            Visibility = visibility;
            ArgumentKinds = System.Array.AsReadOnly(argumentKinds);
            BundledCall = DefaultCall;
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


        public Visibility Visibility { get; }
        public Symbol MethodName { get; }
        public IList<ArgumentKind> ArgumentKinds { get; }
        public CallCompiler CallCompiler { get; set; }
        public Function BundledCall { get; set; }
        public Arity Arity => arity ?? (arity = CalculateArity());


        private iObject DefaultCall(iObject instance, ArgumentBundle bundle)
        {
            Compile();
            return BundledCall(instance, bundle);
        }

        private void Compile()
        {
            if(CallCompiler == null)
            {
                CallCompiler = new PolymorphicCallCompiler(this);
            }
            BundledCall = CallCompiler.Compile();
        }


        public iObject Call(iObject instance, params iObject[] arguments)
        {
            var bundle = new ArgumentBundle(ArgumentKinds, arguments);
            return BundledCall(instance, bundle);
        }


        private Arity CalculateArity()
        {
            var min = ArgumentKinds.Count(a => a == ArgumentKind.Simple);
            if(ArgumentKinds.Any(a => a == ArgumentKind.Key || a == ArgumentKind.KeyRest))
            {
                min++;
            }
            var max = ArgumentKinds.Any(a => a == ArgumentKind.Rest) ? int.MaxValue : min;
            return new Arity(min, max);
        }


        public override string ToString()
        {
            var argumentKinds = string.Join(", ", ArgumentKinds.Select(_ => _.Description));
            return $"CallSite<\"{MethodName}\"<{Arity}>({argumentKinds})>";
        }


        public static class Reflection
        {
            public static readonly MethodInfo Call = Reflector<CallSite>.Method(
                _ => _.Call(default(iObject), default(iObject[]))
            );
        }


        public static class Expressions
        {
            public static MethodCallExpression Call(Expression callSite, Expression instance, Expression arguments)
                => Expression.Call(callSite, Reflection.Call, instance, arguments);
        }
    }
}
