using Mint.MethodBinding.Arguments;
using Mint.Reflection.Parameters;
using System.Reflection;

namespace Mint.MethodBinding.Parameters
{
    internal class SuffixRequiredParameterBinder : ParameterBinder
    {
        public SuffixRequiredParameterBinder(ParameterInfo parameter, ParameterCounter counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            var numParameters = CountParameters();
            var splatPosition = bundle.Splat.Count + numParameters - Parameter.Position - 2;

            if(splatPosition < 0 || splatPosition >= bundle.Splat.Count)
            {
                throw new ArgumentError(
                    $"required parameter `{Parameter.Name}' with index {Parameter.Position} not passed");
            }

            return bundle.Splat[splatPosition];
        }

        private int CountParameters() =>
            CountRequired() + ParameterCounter.Optional + (ParameterCounter.HasRest ? 1 : 0);
    }
}