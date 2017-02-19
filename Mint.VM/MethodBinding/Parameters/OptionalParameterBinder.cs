using Mint.MethodBinding.Arguments;
using Mint.Reflection;
using Mint.Reflection.Parameters;

namespace Mint.MethodBinding.Parameters
{
    internal class OptionalParameterBinder : ParameterBinder
    {
        public OptionalParameterBinder(ParameterMetadata parameter, ParameterCounter counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            var available = bundle.Splat.Count - CountRequired();

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