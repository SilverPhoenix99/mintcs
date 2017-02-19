namespace Mint.Reflection.Parameters
{
    internal interface ParameterState
    {
        ParameterState Parse(ParameterMetadata parameter);
    }
}