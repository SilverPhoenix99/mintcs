using System.Collections.Generic;
using System.Linq;
using static Mint.MethodBinding.ParameterKind;

namespace Mint.MethodBinding
{
    public class CallInfo
    {
        public Visibility Visibility { get; } // Private: "f", Protected: "self.f", Public: "anything.f"
        public Symbol MethodName { get; }
        public ParameterKind[] Parameters { get; }

        public CallInfo(Symbol methodName, Visibility visibility, IEnumerable<ParameterKind> parameters)
        {
            MethodName = methodName;
            Visibility = visibility;
            Parameters = parameters.ToArray();
        }

        public bool IsVarArgs => Parameters.Contains(Rest) || Parameters.Contains(KeyRest);

        public Range Arity
        {
            get
            {
                long max;
                var min = max = Parameters.Count(p => p == Required || p == Block || p == KeyRequired);

                if(IsVarArgs)
                {
                    max = long.MaxValue;
                }

                return new Range(min, max);
            }
        }

        public override string ToString()
        {
            var arity = (Fixnum) Arity.End == long.MaxValue ? $"{Arity.Begin}+" : Arity.ToString();
            var parameters = string.Join(", ", Parameters);
            return $"\"{MethodName}\"<{arity}>({parameters})";
        }
    }
}
