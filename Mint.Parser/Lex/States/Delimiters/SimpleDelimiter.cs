using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mint.Parse;
using static Mint.Parse.TokenType;
using static Mint.Lex.States.LiteralFeatures;

namespace Mint.Lex.States.Delimiters
{
    internal class SimpleDelimiter : Delimiter
    {
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


        public SimpleDelimiter(StringLiteral literal, string delimiterText)
        {
            Literal = literal;
            Text = delimiterText;
            OpenDelimiter = CloseDelimiter = Text[Text.Length - 1];

            BeginType = CalculateBeginTokenType(Text);
            Features = CalculateFeatures(Text);
        }


        public virtual bool IsNested => false;
        public char CloseDelimiter { get; protected set; }
        public TokenType BeginType { get; }
        public LiteralFeatures Features { get; set; }
        public bool CanLabel => Features.HasFlag(Label);
        public bool HasInterpolation => Features.HasFlag(Interpolation);
        public bool IsWords => Features.HasFlag(Words);
        public bool IsRegexp => Features.HasFlag(Regexp);
        protected StringLiteral Literal { get; }
        protected char OpenDelimiter { get; }
        private string Text { get; }


        public virtual void IncrementNesting()
        { }


        public virtual void DecrementNesting()
        { }


        private static TokenType CalculateBeginTokenType(string text)
        {
            var length = Math.Min(text.Length, 2);
            var key = text.Substring(0, length);
            return OPEN_DELIMITERS.TryGetValue(key, out var type) ? type : tSTRING_BEG;
        }


        private static LiteralFeatures CalculateFeatures(string text)
        {
            var effectiveDelimiter = text[0];

            if(effectiveDelimiter == '%')
            {
                switch(text[1])
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
                effectiveDelimiter = text[1];
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