using System.Reflection;
using Mint.MethodBinding.Parameters;

namespace Mint.MethodBinding.Binders
{
    internal class MethodInformation
    {
        public readonly MethodInfo MethodInfo;
        public readonly ParameterInformation ParameterInformation;

        private Range arity;
        public Range Arity => arity ?? (arity = MethodInfo.CalculateArity());

        public MethodInformation(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            ParameterInformation = new ParameterInformation(MethodInfo.GetParameters());
        }
    }
}
