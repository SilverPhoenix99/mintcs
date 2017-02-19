using Mint.MethodBinding.Arguments;
using Mint.Reflection;
using Mint.Reflection.Parameters;

namespace Mint.MethodBinding.Parameters
{
    public abstract class ParameterBinder
    {
        public ParameterMetadata Parameter { get; }
        public ParameterCounter ParameterCounter { get; }

        public ParameterBinder(ParameterMetadata parameter, ParameterCounter counter)
        {
            Parameter = parameter;
            ParameterCounter = counter;
        }

        public abstract iObject Bind(ArgumentBundle bundle);

        protected int CountRequired() => ParameterCounter.PrefixRequired + ParameterCounter.SuffixRequired;
    }
}