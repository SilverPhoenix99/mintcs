using Mint.Binding.Arguments;
using Mint.Reflection.Parameters;
using System.Reflection;

namespace Mint.Binding.Parameters
{
    internal class BlockParameterBinder : ParameterBinder
    {
        public BlockParameterBinder(ParameterInfo parameter, ParameterInformation counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle) => bundle.Block ?? new NilClass();
    }
}