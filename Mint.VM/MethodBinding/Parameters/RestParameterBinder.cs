using Mint.MethodBinding.Arguments;
using Mint.Reflection.Parameters;
using System.Linq;
using System.Reflection;

namespace Mint.MethodBinding.Parameters
{
    internal class RestParameterBinder : ParameterBinder
    {
        public RestParameterBinder(ParameterInfo parameter, ParameterInformation counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            var beginPosition = ParameterCounter.PrefixRequired + ParameterCounter.Optional;
            var endPosition = bundle.Splat.Count - ParameterCounter.SuffixRequired;

            var result = new Array();

            for(var i = beginPosition; i < endPosition; i++)
            {
                result.Add(bundle.Splat[i]);
            }

            var hasKeyArguments = bundle.CallInfo.Arguments.Any(a => a == ArgumentKind.Key || a == ArgumentKind.KeyRest);
            var hasKeyRestParameter = ParameterCounter.HasKeyRest;
            var restIncludesKeyRest = hasKeyArguments != hasKeyRestParameter;

            if(restIncludesKeyRest)
            {
                result.Add(new Hash(bundle.Keys));
            }

            return result;
        }
    }
}