using System.Linq;
using System.Reflection;

namespace Mint.MethodBinding
{
    public sealed partial class ClrMethodBinder
    {
        private class Info
        {
            // parameters, if specified, will follow this order:
            // Required, Optional, Rest, Required, (KeyRequired | KeyOptional), KeyRest, Block

            public readonly MethodInfo Method;
            public readonly Range Arity;

            public Info(MethodInfo method)
            {
                Method = method;
                Arity = CalculateArity();
            }

            private Range CalculateArity()
            {
                var parameters = Method.GetParameters();
                var min = parameters.Count(_ => !_.IsOptional);
                var max = parameters.Length;

                if(Method.IsStatic)
                {
                    min--;
                    max--;
                }

                return new Range(min, max);
            }
        }
    }
}
