using System.Reflection;

namespace Mint.Reflection.Parameters
{
    public partial class ParameterInformation
    {
        internal class KeyRestState : BaseParameterState
        {
            public KeyRestState(ParameterInformation parameterInformation) : base(parameterInformation) { }

            public override ParameterState Parse(ParameterInfo info)
            {
                switch(info.GetParameterKind())
                {
                    case ParameterKind.KeyRest: UpdateWith(info); return this;
                    case ParameterKind.Block:   return ParseInfoWith<BlockState>(info);

                    default: return InvalidParameterError(info);
                }
            }

            private void UpdateWith(ParameterInfo info)
            {
                if(ParameterInformation.HasKeyRest)
                {
                    DuplicateParameterError("keywords", info);
                }

                ParameterInformation.HasKeyRest = true;
            }
        }
    }
}