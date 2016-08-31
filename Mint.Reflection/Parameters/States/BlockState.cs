using System.Reflection;

namespace Mint.Reflection.Parameters
{
    public partial class ParameterCounter
    {
        private class BlockState : BaseParameterState
        {
            public BlockState(ParameterCounter parameterCounter) : base(parameterCounter) { }

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
                if(ParameterCounter.HasBlock)
                {
                    DuplicateParameterError("block", info);
                }

                ParameterCounter.HasBlock = true;
            }
        }
    }
}