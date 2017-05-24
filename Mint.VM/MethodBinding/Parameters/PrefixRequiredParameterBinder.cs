using Mint.MethodBinding.Arguments;
using Mint.Reflection;

namespace Mint.MethodBinding.Parameters
{
    internal class PrefixRequiredParameterBinder : ParameterBinder
    {
        public PrefixRequiredParameterBinder(MethodMetadata method, ParameterMetadata parameter)
            : base(method, parameter)
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