using System;

namespace Mint.Lex.States
{
    [Flags]
    internal enum LiteralFeatures
    {
        None          = 0x0,
        Label         = 0x1,
        Interpolation = 0x2,
        Words         = 0x4,
        Regexp        = 0x8 | Interpolation
    }
}
