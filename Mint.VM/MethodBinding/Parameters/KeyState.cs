using System.Reflection;

namespace Mint.MethodBinding.Parameters
{
    public partial class ParameterInformation
    {
        internal class KeyState : BaseParameterState
        {
            public KeyState(ParameterInformation parameterInformation) : base(parameterInformation) { }

            public override ParameterState Parse(ParameterInfo info)
            {
                switch(info.GetParameterKind())
                {
                    case ParameterKind.KeyRequired: ParameterInformation.KeyRequired++; return this;
                    case ParameterKind.KeyOptional: ParameterInformation.KeyOptional++; return this;
                    case ParameterKind.KeyRest:     return ParseInfoWith<KeyRestState>(info);
                    case ParameterKind.Block:       return ParseInfoWith<BlockState>(info);

                    default: return InvalidParameterError(info);
                }
            }
        }
    }
}