using Mint.Binding.Arguments;
using Mint.Reflection.Parameters;
using System.Reflection;

namespace Mint.Binding.Parameters
{
    internal class OptionalParameterBinder : ParameterBinder
    {
        public OptionalParameterBinder(ParameterInfo parameter, ParameterInformation counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            if(Parameter.Position < bundle.Splat.Count)
            {
                return bundle.Splat[Parameter.Position];
            }

            var defaultValue = Parameter.HasDefaultValue ? Parameter.DefaultValue : null;
            return Object.Box(defaultValue);
        }
    }
}