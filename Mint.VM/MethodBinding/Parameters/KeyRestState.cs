using System.Reflection;

namespace Mint.MethodBinding.Parameters
{
    public partial class ParameterInformation
    {
        internal class KeyRestState : BaseParameterState
        {
            public KeyRestState(ParameterInformation parameterInformation) : base(parameterInformation) { }

            public override ParameterState Parse(ParameterInfo info)
            {
                return Match(info.IsBlock(),
                                typeof(BlockState))

                    ?? Match(!info.IsKeyRest(),
                                () => InvalidParameter(info))

                    ?? Match(ParameterInformation.HasKeyRest,
                                () => DuplicateParameter("keywords", info))

                    ?? Match(true,
                                () => { ParameterInformation.HasKeyRest = true; })
                ;
            }
        }
    }
}