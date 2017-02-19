using Mint.MethodBinding.Parameters;
using Mint.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Mint
{
    internal static class MethodInfoExtensions
    {
        public static IEnumerable<ParameterBinder> GetParameterBinders(this MethodMetadata method)
        {
            var binders = method.Attributes.OfType<ParameterBinderCollection>().FirstOrDefault();

            if(binders == null)
            {
                binders = new ParameterBinderCollection(CreateParameterBinders(method).ToArray());
                method.Attributes.Add(binders);
            }

            return binders.Binders;
        }

        private static IEnumerable<ParameterBinder> CreateParameterBinders(MethodMetadata method)
        {
            var parameterMetadatas = method.Parameters.GetEnumerator();
            var parameterCounter = method.ParameterCounter;
            for(var i = 0; i < parameterCounter.PrefixRequired; i++)
            {
                var parameterMetadata = parameterMetadatas.Next();
                yield return new PrefixRequiredParameterBinder(parameterMetadata, parameterCounter);
            }

            for(var i = 0; i < parameterCounter.Optional; i++)
            {
                var parameterMetadata = parameterMetadatas.Next();
                yield return new OptionalParameterBinder(parameterMetadata, parameterCounter);
            }

            if(parameterCounter.HasRest)
            {
                var parameterMetadata = parameterMetadatas.Next();
                yield return new RestParameterBinder(parameterMetadata, parameterCounter);
            }

            for(var i = 0; i < parameterCounter.SuffixRequired; i++)
            {
                var parameterMetadata = parameterMetadatas.Next();
                yield return new SuffixRequiredParameterBinder(parameterMetadata, parameterCounter);
            }

            var numKeys = parameterCounter.KeyRequired + parameterCounter.KeyOptional;
            for(var i = 0; i < numKeys; i++)
            {
                var parameterMetadata = parameterMetadatas.Next();
                yield return parameterMetadata.IsOptional
                    ? (ParameterBinder) new KeyOptionalParameterBinder(parameterMetadata, parameterCounter)
                    : new KeyRequiredParameterBinder(parameterMetadata, parameterCounter);
            }

            if(parameterCounter.HasKeyRest)
            {
                var parameterMetadata = parameterMetadatas.Next();
                yield return new KeyRestParameterBinder(parameterMetadata, parameterCounter);
            }

            if(parameterCounter.HasBlock)
            {
                var parameterMetadata = parameterMetadatas.Next();
                yield return new BlockParameterBinder(parameterMetadata, parameterCounter);
            }
        }

        private static T Next<T>(this IEnumerator<T> enumerator) =>
            enumerator.MoveNext() ? enumerator.Current : default(T);
    }
}
