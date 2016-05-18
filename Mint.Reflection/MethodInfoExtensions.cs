using Mint.Reflection.Parameters;
using System;
using System.Linq;
using System.Reflection;

namespace Mint.Reflection
{
    public static class MethodInfoExtensions
    {
        public static Tuple<long, long> CalculateArity(this MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            var numOptionalParameters = parameters.Count(p => p.IsOptional());
            var numVarArgs = parameters.Count(p => p.IsRest() || p.IsKeyRest());

            var min = parameters.LongLength - numOptionalParameters - numVarArgs;
            var max = parameters.LongLength;

            if(methodInfo.IsStatic)
            {
                min--;
                max--;
            }

            if(numVarArgs != 0)
            {
                max = long.MaxValue;
            }

            return new Tuple<long, long>(min, max);
        }
    }
}
