using Mint.MethodBinding.Arguments;
using Mint.Reflection;
using Mint.Reflection.Parameters;

namespace Mint.MethodBinding.Parameters
{
    internal class KeyRequiredParameterBinder : ParameterBinder
    {
        public KeyRequiredParameterBinder(ParameterMetadata parameter, ParameterCounter counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            var value = bundle.Keywords[new Symbol(Parameter.Name)];

            if(value != null)
            {
                return value;
            }

            throw new ArgumentError(
                $"required keyword parameter `{Parameter.Name}' with index {Parameter.Position} was not passed");
        }
    }
}