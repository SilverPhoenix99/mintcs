using System.Reflection;

namespace Mint.Reflection.Parameters
{
    public partial class ParameterCounter
    {
        internal class KeyState : BaseParameterState
        {
            public KeyState(ParameterCounter parameterCounter) : base(parameterCounter) { }

            public override ParameterState Parse(ParameterInfo info)
            {
                switch(info.GetParameterKind())
                {
                    case ParameterKind.KeyRequired: ParameterCounter.KeyRequired++; return this;
                    case ParameterKind.KeyOptional: ParameterCounter.KeyOptional++; return this;
                    case ParameterKind.KeyRest:     return ParseInfoWith<KeyRestState>(info);
                    case ParameterKind.Block:       return ParseInfoWith<BlockState>(info);

                    default: return InvalidParameterError(info);
                }
            }
        }
    }
}