using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mint.Lex.States.Delimiters;
using Mint.Parse;
using static Mint.Parse.TokenType;
using static Mint.Lex.States.LiteralFeatures;

namespace Mint.Lex.States
{
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
        private RegexpFlags regexpOptions = RegexpFlags.None;
        private bool emittedSpace;

        private Delimiter Delimiter { get; }

        private State EndState { get; }

        protected override char CurrentChar => Lexer.CurrentChar;

        protected override bool CanLabel => features.HasFlag(Label);

        private bool HasInterpolation => features.HasFlag(Interpolation);

        private bool IsWords => features.HasFlag(Words);

        private bool IsRegexp => features.HasFlag(Regexp);

        private string DelimiterString => BeginToken.Value;

        private char OpenDelimiter => DelimiterString[DelimiterString.Length - 1];

        private bool IsDelimiter => Lexer.CurrentChar == Delimiter.CloseDelimiter;

        public StringLiteral(Lexer lexer, int ts, int te, bool canLabel = false, State endState = null)
            : base(lexer, lexer.Position + 1)
        {
            var text = lexer.TextAt(ts, te);
            var type = CalculateBeginTokenType(text);
            BeginToken = lexer.GenerateToken(type, ts, te);

            EndState = endState ?? lexer.EndState;

            features = CalculateFeatures();
            if(canLabel)
            {
                features |= Label;
            }

            BeginToken.Properties["has_interpolation"] = HasInterpolation;

            Delimiter = CreateDelimiter(OpenDelimiter);
        }

        private static TokenType CalculateBeginTokenType(string delimiter)
        {
            var length = Math.Min(delimiter.Length, 2);
            var key = delimiter.Substring(0, length);
            TokenType type;
            return OPEN_DELIMITERS.TryGetValue(key, out type) ? type : tSTRING_BEG;
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

        private Delimiter CreateDelimiter(char openDelimiter)
        {
            char closeDelimiter;
            return NESTING_DELIMITERS.TryGetValue(openDelimiter, out closeDelimiter)
                ? new NestingDelimiter(this, openDelimiter, closeDelimiter)
                : new SimpleDelimiter(this, openDelimiter);
        }

        protected override void EmitContent(int te)
        {

            if(contentStart == te)
            {
                return;
            }

            base.EmitContent(te);
            emittedSpace = false;
        }

        protected override void EmitDBeg()
        {
            base.EmitDBeg();
            emittedSpace = false;
        }

        protected override void EmitDVar(TokenType type)
        {
            base.EmitDVar(type);
            emittedSpace = false;
        }

        private void EmitEndToken(int ts, int te)
        {
            EmitSpace(ts, ts);

            var type = Lexer.Data[te - 1] == ':' ? tLABEL_END
                     : IsRegexp ? tREGEXP_END
                     : tSTRING_END;

            Lexer.EmitToken(type, ts, te);
        }

        private void EmitSpace(int ts, int te)
        {
            if(!IsWords || emittedSpace)
            {
                return;
            }

            Lexer.EmitToken(tSPACE, ts, te);
            emittedSpace = true;
            contentStart = this.te;
        }
    }
}