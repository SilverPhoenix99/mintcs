namespace Mint.Reflection.Parameters
{
    public partial class ParameterCounter
    {
        private class BlockState : BaseParameterState
        {
            public BlockState(ParameterCounter parameterCounter)
                : base(parameterCounter)
            { }


            public override ParameterState Parse(ParameterMetadata parameter)
            {
                switch(parameter.Kind)
                {
                    case ParameterKind.Block: UpdateWith(parameter); return this;

                    default: return InvalidParameterError(parameter);
                }
            }


            private void UpdateWith(ParameterMetadata parameter)
            {
                if(ParameterCounter.HasBlock)
                {
                    DuplicateParameterError("block", parameter);
                }

                ParameterCounter.HasBlock = true;
            }
        }
    }
}