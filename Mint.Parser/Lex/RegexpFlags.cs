using System;
using System.Text.RegularExpressions;

namespace Mint.Lex
{
    [Flags]
    public enum RegexpFlags : uint
    {
        None       = RegexOptions.None,
        IgnoreCase = RegexOptions.IgnoreCase,
        Multiline  = RegexOptions.Singleline,
        Extend     = RegexOptions.IgnorePatternWhitespace,
        Once       = 1 << 10,

        EncodingMask = 0x7800,
        Ascii8       = 1 << 11,
        EucJp        = 1 << 12,
        Windows31J   = 1 << 13,
        Utf8         = 1 << 14,
    }
}
