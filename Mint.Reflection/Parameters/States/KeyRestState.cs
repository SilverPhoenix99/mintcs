namespace Mint.Reflection.Parameters
{
    public partial class ParameterCounter
    {
        private class KeyRestState : BaseParameterState
        {
            public KeyRestState(ParameterCounter parameterCounter)
                : base(parameterCounter)
            { }


            public override ParameterState Parse(ParameterMetadata parameter)
            {
                switch(parameter.Kind)
                {
                    case ParameterKind.KeyRest: UpdateWith(parameter); return this;
                    case ParameterKind.Block:   return ParseInfoWith<BlockState>(parameter);

                    default: return InvalidParameterError(parameter);
                }
            }


            private void UpdateWith(ParameterMetadata parameter)
            {
                if(ParameterCounter.HasKeyRest)
                {
                    DuplicateParameterError("keywords", parameter);
                }

                ParameterCounter.HasKeyRest = true;
            }
        }
    }
}