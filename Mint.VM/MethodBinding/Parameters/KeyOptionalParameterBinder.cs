using Mint.MethodBinding.Arguments;
using Mint.Reflection.Parameters;
using System.Reflection;

namespace Mint.MethodBinding.Parameters
{
    internal class KeyOptionalParameterBinder : ParameterBinder
    {
        public KeyOptionalParameterBinder(ParameterInfo parameter, ParameterCounter counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            throw new System.NotImplementedException();
        }
    }
}