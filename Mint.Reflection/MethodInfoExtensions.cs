using Mint.Reflection.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

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

        public static IEnumerable<MethodInfo> GetExtensionOverloads(this MethodInfo method)
        {
            return
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where type.IsSealed
                   && !type.IsGenericType
                   && !type.IsNested
                   && type.IsDefined(typeof(ExtensionAttribute), false)
                from m in type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                where m.IsDefined(typeof(ExtensionAttribute), false)
                   && m.Name == method.Name
                   && Matches(m.GetParameters()[0], method.DeclaringType)
                select m
            ;
        }

        private static bool Matches(ParameterInfo info, Type declaringType)
        {
            if(!info.ParameterType.IsGenericParameter)
            {
                // return : info.ParameterType is == or superclass of declaringType?
                var matches = info.ParameterType.IsAssignableFrom(declaringType);
                return matches;
            }

            var constraints = info.ParameterType.GetGenericParameterConstraints();
            return constraints.Length == 0 || constraints.Any(type => type.IsAssignableFrom(declaringType));
        }
    }
}
