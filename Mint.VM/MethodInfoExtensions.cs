using Mint.MethodBinding.Parameters;
using Mint.Reflection;
using Mint.Reflection.Parameters;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Mint
{
    internal static class MethodInfoExtensions
    {
        public static IEnumerable<ParameterBinder> GetParameterBinders(this MethodInfo methodInfo)
        {
            var descriptor = TypeDescriptor.GetAttributes(methodInfo);
            var binders = descriptor[typeof(ParameterBinderCollection)] as ParameterBinderCollection;

            if(binders == null)
            {
                binders = new ParameterBinderCollection(CreateParameterBinders(methodInfo).ToArray());
                TypeDescriptor.AddAttributes(methodInfo, binders);
            }

            return binders.Binders;
        }

        private static IEnumerable<ParameterBinder> CreateParameterBinders(MethodInfo methodInfo)
        {
            var parameterInfos = methodInfo.GetParameterEnumerator();
            var parameterInformation = methodInfo.GetParameterCounter();
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
