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

    internal abstract class Literal : StateBase
    {
        public uint BraceCount { get; set; }

        public abstract Token BeginToken { get; }

        public abstract TokenType EndTokenType { get; }

        public Literal(Lexer lexer) : base(lexer)
        { }
    }

    internal partial class StringLiteral : Literal
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
        private readonly LiteralFeatures features;
        private int contentStart;
        private RegexpFlags regexpOptions = RegexpFlags.None;

        protected Delimiter Delimiter { get; }

        public State EndState { get; }

        protected override char CurrentChar => Delimiter.CurrentChar;

        protected override bool CanLabel => features.HasFlag(Label);

        protected bool HasInterpolation => features.HasFlag(Interpolation);

        protected bool IsWords => features.HasFlag(Words);

        protected bool IsRegexp => features.HasFlag(Regexp);

        private string DelimiterString => BeginToken.Value;

        private char OpenDelimiter => DelimiterString[DelimiterString.Length - 1];

        public override TokenType EndTokenType => IsRegexp ? tREGEXP_END : tSTRING_END;

        public override Token BeginToken { get; }

        public StringLiteral(Lexer lexer, int ts, int te, bool canLabel = false, State endState = null) : base(lexer)
        {
            var text = lexer.TextAt(ts, te);
            var type = CalculateBeginTokenType(text);
            BeginToken = lexer.EmitToken(type, ts, te);

            contentStart = te;
            EndState = endState ?? lexer.EndState;

            features = CalculateFeatures();
            if(canLabel)
            {
                features |= Label;
            }

            Delimiter = CreateDelimiter(OpenDelimiter);
        }

        private static TokenType CalculateBeginTokenType(string delimiter)
        {
            var length = Math.Min(delimiter.Length, 2);
            var key = delimiter.Substring(0, length);
            TokenType type;
            return OPEN_DELIMITERS.TryGetValue(key, out type) ? type : tSTRING_BEG;
        }

        private Delimiter CreateDelimiter(char openDelimiter)
        {
            if(openDelimiter == '\n')
            {
                return new NewLineDelimiter(this);
            }

            char closeDelimiter;
            return NESTING_DELIMITERS.TryGetValue(openDelimiter, out closeDelimiter)
                ? new NestingDelimiter(this, openDelimiter, closeDelimiter)
                : new SimpleDelimiter(this, openDelimiter);
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
            var effectiveDelimiter = DelimiterString[0];

            if(effectiveDelimiter == '%')
            {
                switch(DelimiterString[1])
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
                effectiveDelimiter = DelimiterString[1];
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