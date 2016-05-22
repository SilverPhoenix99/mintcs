using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mint.MethodBinding.Binders;
using Mint.Reflection;

namespace Mint
{
    internal static class MethodInfoExtensions
    {
        private static IEnumerator<ParameterInfo> GetParameterEnumerator(this MethodInfo methodInfo) =>
            methodInfo.GetParameters().AsEnumerable().GetEnumerator();

        private static T Next<T>(this IEnumerator<T> enumerator) =>
            enumerator.MoveNext() ? enumerator.Current : default(T);

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

        public static IEnumerable<ParameterBinder> GetParameterBinders(this MethodInformation methodInformation)
        {
            var parameterInfos = methodInformation.MethodInfo.GetParameterEnumerator();
            var parameterInformation = methodInformation.ParameterInformation;
            for(var i = 0; i < parameterInformation.PrefixRequired; i++)
            {
                var parameterInfo = parameterInfos.Next();
                yield return new PrefixRequiredParameterBinder(parameterInfo, parameterInformation);
            }

            for(var i = 0; i < parameterInformation.Optional; i++)
            {
                var parameterInfo = parameterInfos.Next();
                yield return new OptionalParameterBinder(parameterInfo, parameterInformation);
            }
            
            if(parameterInformation.HasRest)
            {
                var parameterInfo = parameterInfos.Next();
                yield return new RestParameterBinder(parameterInfo, parameterInformation);
            }

            for(var i = 0; i < parameterInformation.SuffixRequired; i++)
            {
                var parameterInfo = parameterInfos.Next();
                yield return new SuffixRequiredParameterBinder(parameterInfo, parameterInformation);
            }

            // TODO: Key and KeyRest
            /*var numKeys = parameterInformation.KeyRequired + parameterInformation.KeyOptional;
            for(var i = 0; i < numKeys; i++)
            {
                var parameterInfo = parameterInfos.Next();
                yield return parameterInfo.IsOptional()
                    ? new KeyOptionalParameterBinder(parameterInfo, parameterInformation)
                    : new KeyRequiredParameterBinder(parameterInfo, parameterInformation);
            }*/

            /*if(parameterInformation.HasKeyRest)
            {
                var parameterInfo = parameterInfos.Next();
                yield return new KeyRestParameterBinder(parameterInfo, parameterInformation);
            }*/

            if(parameterInformation.HasBlock)
            {
                var parameterInfo = parameterInfos.Next();
                yield return new BlockParameterBinder(parameterInfo, parameterInformation);
            }
        }
    }
}
