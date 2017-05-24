using Mint.MethodBinding.Arguments;
using Mint.Reflection;

namespace Mint.MethodBinding.Parameters
{
    internal class BlockParameterBinder : ParameterBinder
    {
        public BlockParameterBinder(MethodMetadata method, ParameterMetadata parameter)
            : base(method, parameter)
        { }


        public override iObject Bind(ArgumentBundle bundle)
            => bundle.Block ?? new NilClass();
    }
}