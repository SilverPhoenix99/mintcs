using System.Collections.Generic;
using System.Linq;
using static Mint.MethodBinding.ParameterKind;

namespace Mint.MethodBinding
{
    public class CallInfo
    {
        private Range arity;

        public CallInfo(Symbol methodName, Visibility visibility = Visibility.Public, IEnumerable<ParameterKind> parameters = null)
        {
            MethodName = methodName;
            Visibility = visibility;
            Parameters = parameters?.ToArray() ?? new ParameterKind[0];
        }

        public Visibility Visibility { get; }
        public Symbol MethodName { get; }
        public ParameterKind[] Parameters { get; }
        public Range Arity => arity ?? (arity = CalculateArity());

        private Range CalculateArity()
        {
            var numRequiredParameters = Parameters.Count(p => p == Required || p == KeyRequired || p == Block);
            var numOptionalParameters = Parameters.Count(p => p == Optional || p == KeyOptional);
            var isVarArgs = Parameters.Contains(Rest) || Parameters.Contains(KeyRest);

            var min = numRequiredParameters;
            var max = isVarArgs ? long.MaxValue : numRequiredParameters + numOptionalParameters;

            return new Range(min, max);
        }

        public override string ToString()
        {
            var arityString = (Fixnum) Arity.End == long.MaxValue ? $"{Arity.Begin}+" : Arity.ToString();
            var parameters = string.Join(", ", Parameters);
            return $"\"{MethodName}\"<{arityString}>({parameters})";
        }
    }
}
