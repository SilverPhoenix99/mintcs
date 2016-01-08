using System;
using System.Text.RegularExpressions;

namespace mint.types
{
    class Regexp
    {
        [Flags]
        public enum Flags
        {
            None       = RegexOptions.None,
            IgnoreCase = RegexOptions.IgnoreCase,
            Multiline  = RegexOptions.Singleline,
            Extend     = RegexOptions.IgnorePatternWhitespace,
            Once       = 1 << 10,
        }
    }
}
