using Mint.MethodBinding.Arguments;
using Mint.Reflection;

namespace Mint.MethodBinding.Parameters
{
    public abstract class ParameterBinder
    {
        protected ParameterBinder(MethodMetadata method, ParameterMetadata parameter)
        {
            Method = method;
            Parameter = parameter;
        }


        protected MethodMetadata Method { get; }
        protected ParameterMetadata Parameter { get; }


        public abstract iObject Bind(ArgumentBundle bundle);
    }
}