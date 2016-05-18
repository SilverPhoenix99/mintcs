using System.Reflection;
using Mint.MethodBinding.Parameters;

namespace Mint.MethodBinding.Binders
{
    internal class MethodInformation
    {
        public readonly MethodInfo MethodInfo;
        public readonly ParameterInformation ParameterInformation;
        public Range Arity => ParameterInformation.Arity;

        public MethodInformation(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            ParameterInformation = new ParameterInformation(MethodInfo.GetParameters());
        }
    }
}
