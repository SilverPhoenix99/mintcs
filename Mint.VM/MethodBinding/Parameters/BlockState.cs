using System.Reflection;

namespace Mint.MethodBinding.Parameters
{
    public partial class ParameterInformation
    {
        private class BlockState : BaseParameterState
        {
            public BlockState(ParameterInformation parameterInformation) : base(parameterInformation) { }

            public override ParameterState Parse(ParameterInfo info)
            {
                return Match(!info.IsBlock(),
                                () => InvalidParameter(info))

                    ?? Match(ParameterInformation.HasBlock,
                                () => DuplicateParameter("block", info))

                    ?? Match(true,
                                () => { ParameterInformation.HasBlock = true; })
                ;
            }
        }
    }
}