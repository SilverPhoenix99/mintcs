using Mint.MethodBinding.Arguments;
using Mint.Reflection;
using Mint.Reflection.Parameters;

namespace Mint.MethodBinding.Parameters
{
    internal class KeyOptionalParameterBinder : ParameterBinder
    {
        public KeyOptionalParameterBinder(ParameterMetadata parameter, ParameterCounter counter)
            : base(parameter, counter)
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