using Mint.MethodBinding.Arguments;
using Mint.Reflection;

namespace Mint.MethodBinding.Parameters
{
    internal class KeyRequiredParameterBinder : ParameterBinder
    {
        public KeyRequiredParameterBinder(MethodMetadata method, ParameterMetadata parameter)
            : base(method, parameter)
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