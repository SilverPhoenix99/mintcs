namespace Mint.Reflection.Parameters
{
    public partial class ParameterCounter
    {
        private class RequiredSuffixState : BaseParameterState
        {
            public RequiredSuffixState(ParameterCounter parameterCounter)
                : base(parameterCounter)
            { }


            public override ParameterState Parse(ParameterMetadata parameter)
            {
                switch(parameter.Kind)
                {
                    case ParameterKind.Required:    ParameterCounter.SuffixRequired++; return this;
                    case ParameterKind.KeyRequired: goto case ParameterKind.KeyOptional;
                    case ParameterKind.KeyOptional: return ParseInfoWith<KeyState>(parameter);
                    case ParameterKind.KeyRest:     return ParseInfoWith<KeyRestState>(parameter);
                    case ParameterKind.Block:       return ParseInfoWith<BlockState>(parameter);

                    default: return InvalidParameterError(parameter);
                }
            }
        }
    }
}