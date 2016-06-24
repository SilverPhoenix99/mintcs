using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mint.Lex.States.Delimiters;
using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Lex.States
{
    internal partial class StringLiteral : StateBase
    {
        private static readonly IReadOnlyDictionary<char, char> NESTING_DELIMITERS =
            new ReadOnlyDictionary<char, char>(new SortedList<char, char>(4)
            {
                { '{', '}' },
                { '<', '>' },
                { '[', ']' },
                { '(', ')' },
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
        private int contentStart;

        protected override char CurrentChar => Delimiter.CurrentChar;

        protected override bool CanLabel { get; }

        private char OpenDelimiter => delimiter[delimiter.Length - 1];

        protected Delimiter Delimiter { get; }

        protected State EndState
        {
            get { throw new System.NotImplementedException(nameof(EndState)); }
        }

        public StringLiteral(Lexer lexer, string delimiter, int contentStart, bool canLabel) : base(lexer)
        {
            this.delimiter = delimiter;
            this.contentStart = contentStart;
            CanLabel = canLabel;
            Delimiter = CreateDelimiter(OpenDelimiter);
        }

        private Delimiter CreateDelimiter(char openDelimiter)
        {
            if(openDelimiter == '\n')
            {
                return new NewLineDelimiter(this);
            }

            char closeDelimiter;
            if(NESTING_DELIMITERS.TryGetValue(openDelimiter, out closeDelimiter))
            {
                return new NestingDelimiter(this, openDelimiter, closeDelimiter);
            }

            return new SimpleDelimiter(this, openDelimiter);
        }
    }
}