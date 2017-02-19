using Mint.MethodBinding.Arguments;
using Mint.Reflection;
using Mint.Reflection.Parameters;

namespace Mint.MethodBinding.Parameters
{
    internal class KeyRestParameterBinder : ParameterBinder
    {
        public KeyRestParameterBinder(ParameterMetadata parameter, ParameterCounter counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle)
        {
            throw new System.NotImplementedException();
        }
    }
}