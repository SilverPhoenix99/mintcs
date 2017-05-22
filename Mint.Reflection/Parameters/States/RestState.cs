namespace Mint.Reflection.Parameters
{
    public partial class ParameterCounter
    {
        private class RestState : BaseParameterState
        {
            public RestState(ParameterCounter parameterCounter)
                : base(parameterCounter)
            { }


            public override ParameterState Parse(ParameterMetadata parameter)
            {
                switch(parameter.Kind)
                {
                    case ParameterKind.Required:    return ParseInfoWith<RequiredSuffixState>(parameter);
                    case ParameterKind.Rest:        UpdateWith(parameter); return this;
                    case ParameterKind.KeyRequired: goto case ParameterKind.KeyOptional;
                    case ParameterKind.KeyOptional: return ParseInfoWith<KeyState>(parameter);
                    case ParameterKind.KeyRest:     return ParseInfoWith<KeyRestState>(parameter);
                    case ParameterKind.Block:       return ParseInfoWith<BlockState>(parameter);

                    default: return InvalidParameterError(parameter);
                }
            }


            private void UpdateWith(ParameterMetadata parameter)
            {
                if(ParameterCounter.HasRest)
                {
                    DuplicateParameterError("rest", parameter);
                }

                ParameterCounter.HasRest = true;
            }
        }
    }
}