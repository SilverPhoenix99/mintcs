namespace Mint.Reflection.Parameters
{
    public partial class ParameterCounter
    {
        private class KeyState : BaseParameterState
        {
            public KeyState(ParameterCounter parameterCounter)
                : base(parameterCounter)
            { }


            public override ParameterState Parse(ParameterMetadata parameter)
            {
                switch(parameter.Kind)
                {
                    case ParameterKind.KeyRequired: ParameterCounter.KeyRequired++; return this;
                    case ParameterKind.KeyOptional: ParameterCounter.KeyOptional++; return this;
                    case ParameterKind.KeyRest:     return ParseInfoWith<KeyRestState>(parameter);
                    case ParameterKind.Block:       return ParseInfoWith<BlockState>(parameter);

                    default: return InvalidParameterError(parameter);
                }
            }
        }
    }
}