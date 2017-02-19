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
            var parameters = method.Parameters.GetEnumerator();
            var counter = method.ParameterCounter;
            for(var i = 0; i < counter.PrefixRequired; i++)
            {
                var parameter = parameters.Next();
                yield return new PrefixRequiredParameterBinder(method, parameter);
            }

            for(var i = 0; i < counter.Optional; i++)
            {
                var parameter = parameters.Next();
                yield return new OptionalParameterBinder(method, parameter);
            }

            if(counter.HasRest)
            {
                var parameter = parameters.Next();
                yield return new RestParameterBinder(method, parameter);
            }

            for(var i = 0; i < counter.SuffixRequired; i++)
            {
                var parameter = parameters.Next();
                yield return new SuffixRequiredParameterBinder(method, parameter);
            }

            var numKeys = counter.KeyRequired + counter.KeyOptional;
            for(var i = 0; i < numKeys; i++)
            {
                var parameter = parameters.Next();
                yield return parameter.IsOptional
                    ? (ParameterBinder) new KeyOptionalParameterBinder(method, parameter)
                    : new KeyRequiredParameterBinder(method, parameter);
            }

            if(counter.HasKeyRest)
            {
                var parameter = parameters.Next();
                yield return new KeyRestParameterBinder(method, parameter);
            }

            if(counter.HasBlock)
            {
                var parameter = parameters.Next();
                yield return new BlockParameterBinder(method, parameter);
            }
        }

        private static T Next<T>(this IEnumerator<T> enumerator) =>
            enumerator.MoveNext() ? enumerator.Current : default(T);
    }
}
