using Mint.MethodBinding.Arguments;
using Mint.Reflection.Parameters;
using System.Linq;
using System.Reflection;

namespace Mint.MethodBinding.Parameters
{
    internal class RestParameterBinder : ParameterBinder
    {
        public RestParameterBinder(ParameterInfo parameter, ParameterCounter counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            var begin = ParameterCounter.PrefixRequired + ParameterCounter.Optional;
            var end = bundle.Splat.Count - ParameterCounter.SuffixRequired;
            var count = end - begin;

            var values = from i in Enumerable.Range(begin, count)
                         select bundle.Splat[i];

            var result = new Array(values);

            var hasKeyRestParameter = ParameterCounter.HasKeyRest;
            var restIncludesKeyRest = bundle.HasKeyArguments != hasKeyRestParameter;

            if(restIncludesKeyRest)
            {
                result.Add(bundle.Keywords.Duplicate());
            }

            return result;
        }
    }
}