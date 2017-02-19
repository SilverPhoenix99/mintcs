using Mint.MethodBinding.Arguments;
using Mint.Reflection;
using Mint.Reflection.Parameters;

namespace Mint.MethodBinding.Parameters
{
    internal class BlockParameterBinder : ParameterBinder
    {
        public BlockParameterBinder(ParameterMetadata parameter, ParameterCounter counter)
            : base(parameter, counter)
        { }

        public override iObject Bind(ArgumentBundle bundle) => bundle.Block ?? new NilClass();
    }
}