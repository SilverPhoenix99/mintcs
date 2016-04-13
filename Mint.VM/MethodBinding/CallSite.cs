using System.Linq;
using System.Collections.Generic;
using static Mint.MethodBinding.ParameterKind;

namespace Mint.MethodBinding
{
    public delegate iObject Function(iObject instance, params iObject[] args);

    public sealed class CallSite
    {
        public CallSite(Symbol methodName, IEnumerable<ParameterKind> parameters, CallSiteBinder binder = null)
        {
            MethodName = methodName;
            Parameters = parameters.ToArray();
            Arity = CalculateArity();
            Binder = binder;
            Call = Binder != null ? Binder.Compile(this) : DefaultCall;
        }

        public Symbol          MethodName { get; }
        public ParameterKind[] Parameters { get; }
        public Range           Arity      { get; }
        public CallSiteBinder  Binder     { get; set; }
        public Function        Call       { get; set; }
        //public Visibility Visibility { get; } // TODO (private: "f", protected: "self.f", public: "anything.f")

        private iObject DefaultCall(iObject instance, iObject[] args)
        {
            Binder = new PolymorphicSiteBinder();
            Call = Binder.Compile(this);
            return Call(instance, args);
        }

        private Range CalculateArity()
        {
            long max;
            var min = max = Parameters.Count(p => p == Req || p == Block || p == KeyReq);

            if(Parameters.Contains(Rest) || Parameters.Contains(KeyRest))
            {
                max = long.MaxValue;
            }

            return new Range(min, max);
        }

        public override string ToString()
        {
            var arity = (Fixnum) Arity.End == long.MaxValue ? $"{Arity.Begin}+" : Arity.ToString();
            return $"CallSite<{arity}>({string.Join(", ", Parameters)})";
        }
    }
}
