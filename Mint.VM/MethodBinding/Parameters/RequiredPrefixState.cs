using System.Reflection;

namespace Mint.MethodBinding.Parameters
{
    public partial class ParameterInformation
    {
        private class RequiredPrefixState : BaseParameterState
        {
            public RequiredPrefixState(ParameterInformation parameterInformation) : base(parameterInformation) { }

            public override ParameterState Parse(ParameterInfo info)
            {
                return Match(info.IsBlock(),
                                typeof(BlockState))

                    ?? Match(info.IsKeyRest(),
                                typeof(KeyRestState))

                    ?? Match(info.IsKey(),
                                typeof(KeyState))

                    ?? Match(info.IsRest(),
                                typeof(RestState))

                    ?? Match(info.IsOptional(),
                                typeof(OptionalState))

                    ?? Match(true,
                                () => { ParameterInformation.RequiredPrefix++; })
                ;
            }
        }
    }
}
