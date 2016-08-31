using System.Reflection;

namespace Mint.Reflection.Parameters
{
    public partial class ParameterCounter
    {
        private class RestState : BaseParameterState
        {
            public RestState(ParameterCounter parameterCounter) : base(parameterCounter) { }

            public override ParameterState Parse(ParameterInfo info)
            {
                switch(info.GetParameterKind())
                {
                    case ParameterKind.Required:    return ParseInfoWith<RequiredSuffixState>(info);
                    case ParameterKind.Rest:        UpdateWith(info); return this;
                    case ParameterKind.KeyRequired: goto case ParameterKind.KeyOptional;
                    case ParameterKind.KeyOptional: return ParseInfoWith<KeyState>(info);
                    case ParameterKind.KeyRest:     return ParseInfoWith<KeyRestState>(info);
                    case ParameterKind.Block:       return ParseInfoWith<BlockState>(info);

                    default: return InvalidParameterError(info);
                }
            }

            private void UpdateWith(ParameterInfo info)
            {
                if(ParameterCounter.HasRest)
                {
                    DuplicateParameterError("rest", info);
                }

                ParameterCounter.HasRest = true;
            }
        }
    }
}