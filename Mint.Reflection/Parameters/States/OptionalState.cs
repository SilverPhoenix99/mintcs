using System.Reflection;

namespace Mint.Reflection.Parameters
{
    public partial class ParameterCounter
    {
        private class OptionalState : BaseParameterState
        {
            public OptionalState(ParameterCounter parameterCounter) : base(parameterCounter) { }

            public override ParameterState Parse(ParameterInfo info)
            {
                switch(info.GetParameterKind())
                {
                    case ParameterKind.Required:    return ParseInfoWith<RequiredSuffixState>(info);
                    case ParameterKind.Optional:    ParameterCounter.Optional++; return this;
                    case ParameterKind.Rest:        return ParseInfoWith<RestState>(info);
                    case ParameterKind.KeyRequired: goto case ParameterKind.KeyOptional;
                    case ParameterKind.KeyOptional: return ParseInfoWith<KeyState>(info);
                    case ParameterKind.KeyRest:     return ParseInfoWith<KeyRestState>(info);
                    case ParameterKind.Block:       return ParseInfoWith<BlockState>(info);

                    default: return InvalidParameterError(info);
                }
            }
        }
    }
}
