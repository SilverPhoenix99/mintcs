using Mint.MethodBinding.Arguments;
using Mint.Reflection.Parameters;
using System.Reflection;

namespace Mint.MethodBinding.Parameters
{
    public abstract class ParameterBinder
    {
        public ParameterInfo Parameter { get; }
        public ParameterInformation ParameterCounter { get; }

        public ParameterBinder(ParameterInfo parameter, ParameterInformation counter)
        {
            Parameter = parameter;
            ParameterCounter = counter;
        }

        public abstract iObject Bind(ArgumentBundle bundle);
    }
}