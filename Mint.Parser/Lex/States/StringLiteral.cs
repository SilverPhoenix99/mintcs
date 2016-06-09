using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Lex.States
{
    internal partial class StringLiteral : StateBase
    {
        private static readonly IReadOnlyDictionary<char, string> CLOSE_DELIMITERS =
            new ReadOnlyDictionary<char, string>(new SortedList<char, string>(4)
            {
                { '{', "}" },
                { '<', ">" },
                { '[', "]" },
                { '(', ")" },
            });

        private static readonly IReadOnlyDictionary<string, TokenType> OPEN_DELIMITERS =
            new ReadOnlyDictionary<string, TokenType>(new SortedList<string, TokenType>(11)
            {
                { "%W",  tWORDS_BEG },
                { "%w",  tQWORDS_BEG },
                { "%I",  tSYMBOLS_BEG },
                { "%i",  tQSYMBOLS_BEG },
                { "%x",  tXSTRING_BEG },
                { "`",   tXSTRING_BEG },
                { "%r",  tREGEXP_BEG },
                { "/",   tREGEXP_BEG },
                { "%s",  tSYMBEG },
                { ":'",  tSYMBEG },
                { ":\"", tSYMBEG },
            });

        private readonly string delimiter;
        private readonly char closeDelimiter;
        private readonly bool canLabel;
        private int contentStart;

        private char OpenDelimiter => delimiter[delimiter.Length - 1];

        public StringLiteral(Lexer lexer, string delimiter, int contentStart, bool canLabel) : base(lexer)
        {
            this.delimiter = delimiter;
            this.contentStart = contentStart;
            this.canLabel = canLabel;

            if(!CLOSE_DELIMITERS.TryGetValue(OpenDelimiter, out closeDelimiter))
            {
                closeDelimiter = OpenDelimiter;
            }
        }
    }
}