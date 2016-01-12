using System.Collections.Generic;
using System.Text.RegularExpressions;
using static mint.Compiler.TokenType;

namespace mint.Compiler
{
    class Literal : iLiteral
    {
        public Literal(string delimiter, int content_start, bool can_label)
        {
            Delimiter = delimiter;
            ContentStart = content_start;
            CanLabel = can_label;

            string end_delimiter;
            EndDelimiter = STRING_END.TryGetValue(Delimiter, out end_delimiter) ? end_delimiter : Delimiter;
        }

        public uint         BraceCount          { get; set; }
        public bool         CanLabel            { get; }
        public int          ContentStart        { get; set; }
        public bool         Dedents             => false;
        public string       Delimiter           { get; }
        public string       EndDelimiter        { get; }
        public int          Indent              { get { return 0; } set { } } // Do nothing
        public bool         Interpolates        => INTERPOLATES.IsMatch(Delimiter);
        public bool         IsRegexp            => Delimiter[0] == '/' || Delimiter.StartsWith("%r");
        public bool         IsWords             => Delimiter[0] == '%' && "WwIi".IndexOf(Delimiter[1]) >= 0;
        public int          LineIndent          { get { return 0; } set { } } // Do nothing
        public Lexer.States State               => Lexer.States.STRING_DELIMITER;
        public string       UnterminatedMessage => "unterminated string meets end of file";

        public TokenType Type
        {
            get
            {
                var delim = Delimiter[0] == '%' ? Delimiter.Substring(0, 2) : Delimiter;
                TokenType type;
                return STRING_BEG.TryGetValue(delim, out type) ? type : tSTRING_BEG;
            }
        }

        public void CommitIndent() { } // Do nothing

        public bool IsDelimiter(string delimiter) => EndDelimiter == delimiter;

        private static readonly Regex INTERPOLATES = new Regex("^(/|`|:?\"|%[^qwis])", RegexOptions.Compiled);

        private static readonly IReadOnlyDictionary<string, TokenType> STRING_BEG =
            (IReadOnlyDictionary<string, TokenType>) new SortedList<string, TokenType>(11)
            {
                { "%W",  tWORDS_BEG    },
                { "%w",  tQWORDS_BEG   },
                { "%I",  tSYMBOLS_BEG  },
                { "%i",  tQSYMBOLS_BEG },
                { "%x",  tXSTRING_BEG  },
                { "`",   tXSTRING_BEG  },
                { "%r",  tREGEXP_BEG   },
                { "/",   tREGEXP_BEG   },
                { "%s",  tSYMBEG       },
                { ":'",  tSYMBEG       },
                { ":\"", tSYMBEG       },
            };

        private static readonly IReadOnlyDictionary<string, string> STRING_END =
            (IReadOnlyDictionary<string, string>) new SortedList<string, string>(4)
            {
                { "{", "}" },
                { "<", ">" },
                { "[", "]" },
                { "(", ")" },
            };
    }
}
