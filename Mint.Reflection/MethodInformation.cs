using System.Reflection;
using Mint.Reflection.Parameters;

namespace Mint.Reflection
{
    public class MethodInformation
    {
        public readonly MethodInfo MethodInfo;
        public readonly ParameterInformation ParameterInformation;

        public MethodInformation(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            ParameterInformation = new ParameterInformation(MethodInfo.GetParameters());
        }
    }
}
