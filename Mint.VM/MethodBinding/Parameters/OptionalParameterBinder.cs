using Mint.MethodBinding.Arguments;
using Mint.Reflection;

namespace Mint.MethodBinding.Parameters
{
    internal class OptionalParameterBinder : ParameterBinder
    {
        public OptionalParameterBinder(MethodMetadata method, ParameterMetadata parameter)
            : base(method, parameter)
        { }


        public override iObject Bind(ArgumentBundle bundle)
        {
            var available = bundle.Splat.Count - Method.ParameterCounter.Required;

            if(available > 0 && Parameter.Position < bundle.Splat.Count)
            {
                return bundle.Splat[Parameter.Position];
            }

            var defaultValue = Parameter.Parameter.HasDefaultValue
                ? Object.Box(Parameter.Parameter.DefaultValue)
                : null;

            return defaultValue;
        }
    }
}