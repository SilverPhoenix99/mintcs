using Mint.Binding.Arguments;
using Mint.Reflection.Parameters;
using System.Reflection;

namespace Mint.Binding.Parameters
{
    internal class SuffixRequiredParameterBinder : ParameterBinder
    {
        public SuffixRequiredParameterBinder(ParameterInfo parameter, ParameterInformation counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            var numParameters = CountParameters();
            var splatPositionFromEnd = Parameter.Position + 1 - numParameters;
            var splatPositionFromStart = bundle.Splat.Count - splatPositionFromEnd;

            if(splatPositionFromStart >= bundle.Splat.Count)
            {
                throw new ArgumentError(
                    "required parameter `{Parameter.Name}' with index {Parameter.Position} not passed");
            }

            return bundle.Splat[splatPositionFromStart];
        }

        private int CountParameters() =>
            ParameterCounter.PrefixRequired
            + ParameterCounter.Optional
            + (ParameterCounter.HasRest ? 1 : 0)
            + ParameterCounter.SuffixRequired;
    }
}