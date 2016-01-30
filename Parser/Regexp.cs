using System;
using System.Text.RegularExpressions;

namespace Mint.Parser
{
    public class Regexp
    {
        [Flags]
        public enum Flags : uint
        {
            None = RegexOptions.None,
            IgnoreCase = RegexOptions.IgnoreCase,
            Multiline = RegexOptions.Singleline,
            Extend = RegexOptions.IgnorePatternWhitespace,
            Once = 1 << 10,

            EncodingMask = 0x7800,
            ASCII8 = 1 << 11,
            EUC_JP = 1 << 12,
            Windows_31J = 1 << 13,
            UTF_8 = 1 << 14,
        }
    }
}
