using System;
using System.Linq;
using System.Reflection;

namespace Mint.Reflection
{
    public static class ParameterInfoExtensions
    {
        public static bool IsAssignableFrom(this ParameterInfo info, Type instanceType)
        {
            if(!info.ParameterType.IsGenericParameter)
            {
                // return : info.ParameterType is == or superclass of declaringType?
                var matches = info.ParameterType.IsAssignableFrom(instanceType);
                return matches;
            }

            var constraints = info.ParameterType.GetGenericParameterConstraints();
            return constraints.Length == 0 || constraints.Any(type => type.IsAssignableFrom(instanceType));
        }
    }
}