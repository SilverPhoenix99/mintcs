namespace Mint.Reflection.Parameters
{
    public partial class ParameterCounter
    {
        private class OptionalState : BaseParameterState
        {
            public OptionalState(ParameterCounter parameterCounter) : base(parameterCounter) { }

            public override ParameterState Parse(ParameterMetadata parameter)
            {
                switch(parameter.Kind)
                {
                    case ParameterKind.Required:    return ParseInfoWith<RequiredSuffixState>(parameter);
                    case ParameterKind.Optional:    ParameterCounter.Optional++; return this;
                    case ParameterKind.Rest:        return ParseInfoWith<RestState>(parameter);
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
