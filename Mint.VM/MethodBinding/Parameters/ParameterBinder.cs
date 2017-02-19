using Mint.MethodBinding.Arguments;
using Mint.Reflection;

namespace Mint.MethodBinding.Parameters
{
    public abstract class ParameterBinder
    {
        public MethodMetadata Method { get; }

        public ParameterMetadata Parameter { get; }

        public ParameterBinder(MethodMetadata method, ParameterMetadata parameter)
        {
            Method = method;
            Parameter = parameter;
        }

        public abstract iObject Bind(ArgumentBundle bundle);

        protected int CountRequired() =>
            Method.ParameterCounter.PrefixRequired + Method.ParameterCounter.SuffixRequired;
    }
}