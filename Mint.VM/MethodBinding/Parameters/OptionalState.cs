using System.Reflection;

namespace Mint.MethodBinding.Parameters
{
    public partial class ParameterInformation
    {
        private class OptionalState : BaseParameterState
        {
            public OptionalState(ParameterInformation parameterInformation) : base(parameterInformation) { }

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
                                () => { ParameterInformation.Optional++; })

                    ?? Match(true,
                                typeof(RequiredSuffixState))
                ;
            }
        }
    }
}
