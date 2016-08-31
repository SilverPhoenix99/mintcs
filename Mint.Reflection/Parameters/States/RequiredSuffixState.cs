using System.Reflection;

namespace Mint.Reflection.Parameters
{
    public partial class ParameterCounter
    {
        internal class RequiredSuffixState : BaseParameterState
        {
            public RequiredSuffixState(ParameterCounter parameterCounter) : base(parameterCounter) { }

            public override ParameterState Parse(ParameterInfo info)
            {
                switch(info.GetParameterKind())
                {
                    case ParameterKind.Required:    ParameterCounter.SuffixRequired++; return this;
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