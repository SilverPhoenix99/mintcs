using System.Reflection;

namespace Mint.Reflection.Parameters
{
    public partial class ParameterCounter
    {
        internal class KeyRestState : BaseParameterState
        {
            public KeyRestState(ParameterCounter parameterCounter) : base(parameterCounter) { }

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
                if(ParameterCounter.HasKeyRest)
                {
                    DuplicateParameterError("keywords", info);
                }

                ParameterCounter.HasKeyRest = true;
            }
        }
    }
}