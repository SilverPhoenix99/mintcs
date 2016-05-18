using System.Collections.Generic;
using System.Linq;
using Mint.Reflection;
using Mint.Reflection.Parameters;
using static Mint.Reflection.Parameters.ParameterKind;

namespace Mint.MethodBinding
{
    public class CallInfo
    {
        private Arity arity;

        public Visibility Visibility { get; }
        public Symbol MethodName { get; }
        public ParameterKind[] Parameters { get; }
        public Arity Arity => arity ?? (arity = CalculateArity());

        public CallInfo(Symbol methodName, Visibility visibility = Visibility.Public, IEnumerable<ParameterKind> parameters = null)
        {
            MethodName = methodName;
            Visibility = visibility;
            Parameters = parameters?.ToArray() ?? new ParameterKind[0];
        }

        private Arity CalculateArity()
        {
            var numRequiredParameters = Parameters.Count(p => p == Required || p == KeyRequired || p == Block);
            var numOptionalParameters = Parameters.Count(p => p == Optional || p == KeyOptional);
            var isVarArgs = Parameters.Contains(Rest) || Parameters.Contains(KeyRest);

            var min = numRequiredParameters;
            var max = isVarArgs ? int.MaxValue : numRequiredParameters + numOptionalParameters;

            return new Arity(min, max);
        }

        public override string ToString()
        {
            var parameters = string.Join(", ", Parameters);
            return $"\"{MethodName}\"<{Arity}>({parameters})";
        }
    }
}
