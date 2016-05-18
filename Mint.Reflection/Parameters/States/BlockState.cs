using System.Reflection;

namespace Mint.Reflection.Parameters
{
    public partial class ParameterInformation
    {
        private class BlockState : BaseParameterState
        {
            public BlockState(ParameterInformation parameterInformation) : base(parameterInformation) { }

            public override ParameterState Parse(ParameterInfo info)
            {
                switch(info.GetParameterKind())
                {
                    case ParameterKind.Block: UpdateWith(info); return this;

                    default: return InvalidParameterError(info);
                }
            }

            private void UpdateWith(ParameterInfo info)
            {
                if(ParameterInformation.HasBlock)
                {
                    DuplicateParameterError("block", info);
                }

                ParameterInformation.HasBlock = true;
            }
        }
    }
}