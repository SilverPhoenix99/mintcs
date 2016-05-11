using System.Linq;
using System.Collections.Generic;
using static Mint.MethodBinding.ParameterKind;

namespace Mint.MethodBinding
{
    public delegate iObject Function(iObject instance, params iObject[] arguments);

    public sealed class CallSite
    {
        public Symbol MethodName { get; }
        public ParameterKind[] Parameters { get; }
        public Range Arity { get; }
        public CallSiteBinder CallCompiler { get; set; }
        public Function Call { get; set; }
        public Visibility Visibility { get; } // Private: "f", Protected: "self.f", Public: "anything.f"

        public CallSite(Symbol methodName, Visibility visibility, IEnumerable<ParameterKind> parameters)
        {
            MethodName = methodName;
            Visibility = visibility;
            Parameters = parameters.ToArray();
            Arity = CalculateArity(Parameters);
            Call = DefaultCall;
        }

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
            var arity = (Fixnum) Arity.End == long.MaxValue ? $"{Arity.Begin}+" : Arity.ToString();
            var parameters = string.Join(", ", Parameters);
            return $"CallSite<\"{MethodName}\" : {arity} : {parameters}>";
        }

        private static Range CalculateArity(IEnumerable<ParameterKind> parameters)
        {
            long max;
            var min = max = parameters.Count(p => p == Required || p == Block || p == KeyRequired);

            if(parameters.Contains(Rest) || parameters.Contains(KeyRest))
            {
                max = long.MaxValue;
            }

            return new Range(min, max);
        }
    }
}
