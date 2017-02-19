using Mint.MethodBinding.Arguments;
using Mint.Reflection;

namespace Mint.MethodBinding.Parameters
{
    internal class SuffixRequiredParameterBinder : ParameterBinder
    {
        public SuffixRequiredParameterBinder(MethodMetadata method, ParameterMetadata parameter)
            : base(method, parameter)
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
            CountRequired() + Method.ParameterCounter.Optional + (Method.ParameterCounter.HasRest ? 1 : 0);
    }
}