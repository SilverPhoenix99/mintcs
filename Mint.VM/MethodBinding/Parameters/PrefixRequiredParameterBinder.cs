using Mint.MethodBinding.Arguments;
using Mint.Reflection.Parameters;
using System.Reflection;

namespace Mint.MethodBinding.Parameters
{
    internal class PrefixRequiredParameterBinder : ParameterBinder
    {
        public PrefixRequiredParameterBinder(ParameterInfo parameter, ParameterInformation counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            if(Parameter.Position >= bundle.Splat.Count)
            {
                throw new ArgumentError(
                    "required parameter `{Parameter.Name}' with index {Parameter.Position} not passed");
            }

            return bundle.Splat[Parameter.Position];
        }
    }
}