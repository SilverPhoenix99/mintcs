using Mint.MethodBinding.Arguments;
using Mint.Reflection;
using System.Linq;

namespace Mint.MethodBinding.Parameters
{
    internal class RestParameterBinder : ParameterBinder
    {
        public RestParameterBinder(MethodMetadata method, ParameterMetadata parameter)
            : base(method, parameter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            var begin = Method.ParameterCounter.PrefixRequired + Method.ParameterCounter.Optional;
            var end = bundle.Splat.Count - Method.ParameterCounter.SuffixRequired;
            var count = end - begin;

            if(count <= 0)
            {
                return new Array();
            }

            var values = bundle.Splat.Skip(begin).Take(count);
            var result = new Array(values);

            var hasKeyRestParameter = Method.ParameterCounter.HasKeyRest;
            var restIncludesKeyRest = bundle.HasKeyArguments != hasKeyRestParameter;

            if(restIncludesKeyRest)
            {
                result.Add(bundle.Keywords.Duplicate());
            }

            return result;
        }
    }
}