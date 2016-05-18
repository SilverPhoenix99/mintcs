using System.Reflection;

namespace Mint.Reflection.Parameters
{
    internal interface ParameterState
    {
        ParameterState Parse(ParameterInfo info);
    }
}