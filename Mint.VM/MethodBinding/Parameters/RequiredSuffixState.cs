using System.Reflection;

namespace Mint.MethodBinding.Parameters
{
    public partial class ParameterInformation
    {
        internal class RequiredSuffixState : BaseParameterState
        {
            public RequiredSuffixState(ParameterInformation parameterInformation) : base(parameterInformation) { }

            public override ParameterState Parse(ParameterInfo info)
            {
                return Match(info.IsBlock(),
                                typeof(BlockState))

                    ?? Match(info.IsKeyRest(),
                                typeof(KeyRestState))

                    ?? Match(info.IsKey(),
                                typeof(KeyState))

                    ?? Match(info.IsOptional() || info.IsRest(),
                                () => InvalidParameter(info))

                    ?? Match(true,
                                () => { ParameterInformation.SuffixRequired++; })
                ;
            }
        }
    }
}