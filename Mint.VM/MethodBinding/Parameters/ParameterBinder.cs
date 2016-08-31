using Mint.MethodBinding.Arguments;
using Mint.Reflection.Parameters;
using System.Reflection;

namespace Mint.MethodBinding.Parameters
{
    public abstract class ParameterBinder
    {
        public ParameterInfo Parameter { get; }
        public ParameterCounter ParameterCounter { get; }

        public ParameterBinder(ParameterInfo parameter, ParameterCounter counter)
        {
            Parameter = parameter;
            ParameterCounter = counter;
        }

        public abstract iObject Bind(ArgumentBundle bundle);
    }
}