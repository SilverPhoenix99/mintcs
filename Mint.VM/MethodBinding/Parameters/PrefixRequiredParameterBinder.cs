using Mint.MethodBinding.Arguments;
using Mint.Reflection;
using Mint.Reflection.Parameters;

namespace Mint.MethodBinding.Parameters
{
    internal class PrefixRequiredParameterBinder : ParameterBinder
    {
        public PrefixRequiredParameterBinder(ParameterMetadata parameter, ParameterCounter counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            if(Parameter.Position >= bundle.Splat.Count)
            {
                throw new ArgumentError(
                    $"required parameter `{Parameter.Name}' with index {Parameter.Position} was not passed");
            }

            return bundle.Splat[Parameter.Position];
        }
    }
}