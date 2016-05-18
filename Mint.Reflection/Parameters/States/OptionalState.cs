using System.Reflection;

namespace Mint.Reflection.Parameters
{
    public partial class ParameterInformation
    {
        private class OptionalState : BaseParameterState
        {
            public OptionalState(ParameterInformation parameterInformation) : base(parameterInformation) { }

            public override ParameterState Parse(ParameterInfo info)
            {
                switch(info.GetParameterKind())
                {
                    case ParameterKind.Required:    return ParseInfoWith<RequiredSuffixState>(info);
                    case ParameterKind.Optional:    ParameterInformation.Optional++; return this;
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
