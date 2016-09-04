using Mint.MethodBinding.Arguments;
using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Compilation
{
    internal static class TokenTypeExtensions
    {
        public static ArgumentKind GetArgumentKind(this TokenType type)
        {
            switch(type)
            {
                case kSTAR:
                    return ArgumentKind.Rest;

                case tLABEL_END: goto case kASSOC;
                case tLABEL: goto case kASSOC;
                case kASSOC:
                    return ArgumentKind.Key;

                case kDSTAR:
                    return ArgumentKind.KeyRest;

                case kDO: goto case kAMPER;
                case kLBRACE2: goto case kAMPER;
                case kAMPER:
                    return ArgumentKind.Block;

                default:
                    return ArgumentKind.Simple;
            }
        }
    }
}
