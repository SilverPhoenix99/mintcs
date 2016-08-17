using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Mint.Lex.States;
using Mint.Parse;

namespace Mint.Lex
{
    internal class Literal : iLiteral
    {
        private static readonly Regex INTERPOLATES = new Regex("^(/|`|:?\"|%[^qwis])", RegexOptions.Compiled);

        private static readonly IReadOnlyDictionary<string, TokenType> STRING_BEG =
            new ReadOnlyDictionary<string, TokenType>(new SortedList<string, TokenType>(11)
            {
                { "%W",  TokenType.tWORDS_BEG    },
                { "%w",  TokenType.tQWORDS_BEG   },
                { "%I",  TokenType.tSYMBOLS_BEG  },
                { "%i",  TokenType.tQSYMBOLS_BEG },
                { "%x",  TokenType.tXSTRING_BEG  },
                { "`",   TokenType.tXSTRING_BEG  },
                { "%r",  TokenType.tREGEXP_BEG   },
                { "/",   TokenType.tREGEXP_BEG   },
                { "%s",  TokenType.tSYMBEG       },
                { ":'",  TokenType.tSYMBEG       },
                { ":\"", TokenType.tSYMBEG       },
            });

        private static readonly IReadOnlyDictionary<char, string> STRING_END =
            new ReadOnlyDictionary<char, string>(new SortedList<char, string>(4)
            {
                { '{', "}" },
                { '<', ">" },
                { '[', "]" },
                { '(', ")" },
            });

        private int nesting;

        public uint BraceCount { get; set; }
        public bool CanLabel { get; }
        public int ContentStart { get; set; }
        public bool Dedents => false;
        public int Indent => 0; // Do nothing
        public bool Interpolates => INTERPOLATES.IsMatch(Delimiter);
        public bool IsRegexp => Delimiter[0] == '/' || Delimiter.StartsWith("%r");
        public bool IsWords => Delimiter[0] == '%' && "WwIi".IndexOf(Delimiter[1]) >= 0;
        public string EofErrorMessage => "unterminated string meets end of file";
        public bool WasContent { get; set; }
        public bool IsNested => STRING_END.ContainsKey(BeginDelimiter) && nesting > 0;

        public State State //=> Interpolates ? STRING_INTERPOLATION : STRING_LITERAL;
        {
            get { throw new NotImplementedException(); }
        }

        // Not inherited
        private string Delimiter { get; }
        private string EndDelimiter { get; }

        // Returns the last character from the begin delimiter
        public char BeginDelimiter => Delimiter[Delimiter.Length - 1];

        public TokenType Type
        {
            get
            {
                var delim = Delimiter[0] == '%' ? Delimiter.Substring(0, 2) : Delimiter;
                TokenType type;
                return STRING_BEG.TryGetValue(delim, out type) ? type : TokenType.tSTRING_BEG;
            }
        }

        public Literal(string delimiter, int contentStart, bool canLabel)
        {
            Delimiter = delimiter;
            ContentStart = contentStart;
            CanLabel = canLabel;
            nesting = 0;

            EndDelimiter = Delimiter.Substring(Delimiter.Length - 1);
            string endDelimiter;

            if(STRING_END.TryGetValue(EndDelimiter[0], out endDelimiter))
            {
                EndDelimiter = endDelimiter;
            }
        }

        public void CommitIndent() { } // Do nothing

        public bool IsDelimiter(string delimiter) => EndDelimiter == delimiter;

        // use ^D, since it isn't used anywhere (trimmed at Lexer.Reset())
        public uint TranslateDelimiter(char delimiter) => EndDelimiter[0] == delimiter ? 0x4u : delimiter;
    }
}
