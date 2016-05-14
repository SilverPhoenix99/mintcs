using System.Reflection;

namespace Mint.MethodBinding.Parameters
{
    public partial class ParameterInformation
    {
        private class RequiredPrefixState : BaseParameterState
        {
            public RequiredPrefixState(ParameterInformation parameterInformation) : base(parameterInformation) { }

            public override ParameterState Parse(ParameterInfo info)
            {
                switch(info.GetParameterKind())
                {
                    case ParameterKind.Required:    ParameterInformation.PrefixRequired++; return this;
                    case ParameterKind.Optional:    return ParseInfoWith<OptionalState>(info);
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
