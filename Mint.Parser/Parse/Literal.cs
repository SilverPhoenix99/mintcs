using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Mint.Lex.States;
using static Mint.Parse.TokenType;

namespace Mint.Parse
{
    internal class Literal : iLiteral
    {
        private static readonly Regex INTERPOLATES = new Regex("^(/|`|:?\"|%[^qwis])", RegexOptions.Compiled);

        private static readonly IReadOnlyDictionary<string, TokenType> STRING_BEG =
            new ReadOnlyDictionary<string, TokenType>(new SortedList<string, TokenType>(11)
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
            });

        private static readonly IReadOnlyDictionary<string, string> STRING_END =
            new ReadOnlyDictionary<string, string>(new SortedList<string, string>(4)
            {
                { "{", "}" },
                { "<", ">" },
                { "[", "]" },
                { "(", ")" },
            });

        public Literal(string delimiter, int contentStart, bool canLabel)
        {
            Delimiter = delimiter;
            ContentStart = contentStart;
            CanLabel = canLabel;

            EndDelimiter = Delimiter.Substring(Delimiter.Length - 1);
            string endDelimiter;

            if(STRING_END.TryGetValue(EndDelimiter, out endDelimiter))
            {
                EndDelimiter = endDelimiter;
            }
        }

        public uint         BraceCount          { get; set; }
        public bool         CanLabel            { get; }
        public int          ContentStart        { get; set; }
        public bool         Dedents             => false;
        public int          Indent              => 0; // Do nothing
        public bool         Interpolates        => INTERPOLATES.IsMatch(Delimiter);
        public bool         IsRegexp            => Delimiter[0] == '/' || Delimiter.StartsWith("%r");
        public bool         IsWords             => Delimiter[0] == '%' && "WwIi".IndexOf(Delimiter[1]) >= 0;
        public int          LineIndent          { get { return 0; } set { } } // Do nothing
        public State        State               //=> Interpolates ? STRING_INTERPOLATION : STRING_LITERAL;
        {
            get { throw new NotImplementedException(); }
        }
        public string       UnterminatedMessage => "unterminated string meets end of file";
        public bool         WasContent          { get; set; }
        public int          Nesting             { get; set; }
        public bool         IsNested            => STRING_END.ContainsKey(BeginDelimiter) && Nesting > 0;

        // Not inherited
        private string      Delimiter           { get; }
        private string      EndDelimiter        { get; }

        // Returns the last character from the begin delimiter
        public string BeginDelimiter => Delimiter.Substring(Delimiter.Length - 1);

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

        // use ^D, since it isn't used anywhere (trimmed at Lexer.Reset())
        public uint TranslateDelimiter(char delimiter) => EndDelimiter[0] == delimiter ? 0x4u : delimiter;
    }
}
