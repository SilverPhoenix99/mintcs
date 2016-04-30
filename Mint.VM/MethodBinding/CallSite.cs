using System.Linq;
using System.Collections.Generic;
using static Mint.MethodBinding.ParameterKind;

namespace Mint.MethodBinding
{
    public delegate iObject Function(iObject instance, params iObject[] args);

    public sealed class CallSite
    {
        public CallSite(Symbol methodName, Visibility visibility, IEnumerable<ParameterKind> parameters, CallSiteBinder binder = null)
        {
            MethodName = methodName;
            Visibility = visibility;
            Parameters = parameters.ToArray();
            Arity      = CalculateArity();
            Binder     = binder;
            Call       = DefaultCall;
        }

        public Symbol          MethodName { get; }
        public ParameterKind[] Parameters { get; }
        public Range           Arity      { get; }
        public CallSiteBinder  Binder     { get; set; }
        public Function        Call       { get; set; }
        public Visibility      Visibility { get; } // Private: "f", Protected: "self.f", Public: "anything.f"

        private iObject DefaultCall(iObject instance, iObject[] args)
        {
            if(Binder == null)
            {
                Binder = new PolymorphicSiteBinder();
            }
            Call = Binder.Compile(this);
            return Call(instance, args);
        }

        private Range CalculateArity()
        {
            long max;
            var min = max = Parameters.Count(p => p == Required || p == Block || p == KeyRequired);

            if(Parameters.Contains(Rest) || Parameters.Contains(KeyRest))
            {
                max = long.MaxValue;
            }

            return new Range(min, max);
        }

        public override string ToString()
        {
            var arity = (Fixnum) Arity.End == long.MaxValue ? $"{Arity.Begin}+" : Arity.ToString();
            return $"CallSite<\"{MethodName}\" : {arity} : {string.Join(", ", Parameters)}>";
        }
    }
}
