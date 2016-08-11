using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mint.Lex.States.Delimiters;
using Mint.Parse;
using static Mint.Parse.TokenType;
using static Mint.Lex.States.LiteralFeatures;

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
        private readonly LiteralFeatures features;
        private int contentStart;
        private RegexpFlags regexpOptions;

        protected override char CurrentChar => Delimiter.CurrentChar;

        protected override bool CanLabel => features.HasFlag(LiteralFeatures.Label);

        protected bool HasInterpolation => features.HasFlag(LiteralFeatures.Interpolation);

        protected bool IsWords => features.HasFlag(LiteralFeatures.Words);

        protected bool IsRegexp => features.HasFlag(LiteralFeatures.Regexp);

        private char OpenDelimiter => delimiter[delimiter.Length - 1];

        protected Delimiter Delimiter { get; }

        public State EndState { get; }

        public StringLiteral(
                Lexer lexer, string delimiter, int contentStart, bool canLabel = false, State endState = null)
            : base(lexer)
        {
            this.delimiter = delimiter;
            this.contentStart = contentStart;
            EndState = endState ?? lexer.EndState;

            this.features = CalculateFeatures();
            if(canLabel)
            {
                this.features |= LiteralFeatures.Label;
            }

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

        protected void EmitDBeg()
        {
            throw new NotImplementedException(nameof(EmitDBeg));
        }

        protected void EmitDVar(TokenType type)
        {
            throw new NotImplementedException(nameof(EmitDVar));
        }

        private LiteralFeatures CalculateFeatures()
        {
            char effectiveDelimiter = delimiter[0];

            if(effectiveDelimiter == '%')
            {
                switch(delimiter[1])
                {
                    case 'q': return None;
                    case 'r': return Regexp;
                    case 'w': return Words;
                    case 'i': return Words;
                    case 'W': return Words | Interpolation;
                    case 'I': return Words | Interpolation;
                    default:  return Interpolation;
                }
            }

            if(effectiveDelimiter == ':')
            {
                effectiveDelimiter = delimiter[1];
            }

            switch(effectiveDelimiter)
            {
                case '/': return Regexp;
                case '"': return Interpolation;
                case '`': return Interpolation;
                default:  return None;
            }
        }
    }
}