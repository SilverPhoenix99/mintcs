using Mint.Binding.Arguments;
using Mint.Reflection.Parameters;
using System.Reflection;

namespace Mint.Binding.Parameters
{
    internal class KeyOptionalParameterBinder : ParameterBinder
    {
        public KeyOptionalParameterBinder(ParameterInfo parameter, ParameterInformation counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            throw new System.NotImplementedException();
        }
    }
}