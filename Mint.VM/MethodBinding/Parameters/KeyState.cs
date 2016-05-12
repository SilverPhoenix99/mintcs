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
                return Match(info.IsBlock(),
                                typeof(BlockState))

                    ?? Match(info.IsKeyRest(),
                                typeof(KeyRestState))

                    ?? Match(info.IsKey() && info.IsOptional(),
                                () => { ParameterInformation.KeyOptional++; })

                    ?? Match(info.IsKey(),
                                () => { ParameterInformation.KeyRequired++; })

                    ?? InvalidParameter(info)
                ;
            }
        }
    }
}