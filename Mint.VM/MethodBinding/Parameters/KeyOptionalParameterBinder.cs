using Mint.MethodBinding.Arguments;
using Mint.Reflection;

namespace Mint.MethodBinding.Parameters
{
    internal class KeyOptionalParameterBinder : ParameterBinder
    {
        public KeyOptionalParameterBinder(MethodMetadata method, ParameterMetadata parameter)
            : base(method, parameter)
        { }


        public override iObject Bind(ArgumentBundle bundle)
        {
            var value = bundle.Keywords[new Symbol(Parameter.Name)];

            if(value != null)
            {
                return value;
            }

            var defaultValue = Parameter.Parameter.HasDefaultValue
                ? Object.Box(Parameter.Parameter.DefaultValue)
                : null;

            return defaultValue;
        }
    }
}