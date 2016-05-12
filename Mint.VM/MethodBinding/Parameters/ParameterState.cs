using System.Reflection;

namespace Mint.MethodBinding.Parameters
{
    internal interface ParameterState
    {
        ParameterState Parse(ParameterInfo info);
    }
}