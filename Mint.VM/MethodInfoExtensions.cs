using Mint.MethodBinding.Parameters;
using Mint.Reflection;
using Mint.Reflection.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mint
{
    internal static class MethodInfoExtensions
    {
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

            var numKeys = parameterInformation.KeyRequired + parameterInformation.KeyOptional;
            for(var i = 0; i < numKeys; i++)
            {
                var parameterInfo = parameterInfos.Next();
                yield return parameterInfo.IsOptional()
                    ? (ParameterBinder) new KeyOptionalParameterBinder(parameterInfo, parameterInformation)
                    : new KeyRequiredParameterBinder(parameterInfo, parameterInformation);
            }

            if(parameterInformation.HasKeyRest)
            {
                var parameterInfo = parameterInfos.Next();
                yield return new KeyRestParameterBinder(parameterInfo, parameterInformation);
            }

            if(parameterInformation.HasBlock)
            {
                var parameterInfo = parameterInfos.Next();
                yield return new BlockParameterBinder(parameterInfo, parameterInformation);
            }
        }

        private static IEnumerator<ParameterInfo> GetParameterEnumerator(this MethodInfo methodInfo) =>
            methodInfo.GetParameters().AsEnumerable().GetEnumerator();

        private static T Next<T>(this IEnumerator<T> enumerator) =>
            enumerator.MoveNext() ? enumerator.Current : default(T);
    }
}
