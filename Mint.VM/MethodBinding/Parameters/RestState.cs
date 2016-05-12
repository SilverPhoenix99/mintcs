using System.Reflection;

namespace Mint.MethodBinding.Parameters
{
    public partial class ParameterInformation
    {
        private class RestState : BaseParameterState
        {
            public RestState(ParameterInformation parameterInformation) : base(parameterInformation) { }

            public override ParameterState Parse(ParameterInfo info)
            {
                return Match(info.IsBlock(),
                                typeof(BlockState))

                    ?? Match(info.IsKeyRest(),
                                typeof(KeyRestState))

                    ?? Match(info.IsKey(),
                                typeof(KeyState))

                    ?? Match(info.IsOptional(),
                                () => InvalidParameter(info))

                    ?? Match(!info.IsRest(),
                                typeof(RequiredSuffixState))

                    ?? Match(ParameterInformation.HasRest,
                                () => DuplicateParameter("rest", info))

                    ?? Match(true,
                                () => { ParameterInformation.HasRest = true; })
                ;
            }
        }
    }
}