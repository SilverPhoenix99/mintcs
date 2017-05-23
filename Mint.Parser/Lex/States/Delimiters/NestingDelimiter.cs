using System;

namespace Mint.Lex.States.Delimiters
{
    internal class NestingDelimiter : SimpleDelimiter
    {
        private const string OPEN_DELIMITERS  = "(<[{";
        private const string CLOSE_DELIMITERS = ")>]}";


        private int nesting;


        private NestingDelimiter(StringLiteral literal, string delimiterText)
            : base(literal, delimiterText)
        {
            var index = OPEN_DELIMITERS.IndexOf(OpenDelimiter);
            CloseDelimiter = index >= 0 ? CLOSE_DELIMITERS[index] : throw new ArgumentException(nameof(delimiterText));
        }


        public override bool IsNested => nesting > 0;


        public override void IncrementNesting()
        {
            if(Literal.Lexer.CurrentChar == OpenDelimiter)
            {
                nesting++;
            }
        }


        public override void DecrementNesting()
        {
            nesting--;
        }


        public static NestingDelimiter TryCreate(StringLiteral literal, string delimiterText)
        {
            var openDelimiter = delimiterText[delimiterText.Length - 1];
            return IsValidOpenDelimiter(openDelimiter) ? new NestingDelimiter(literal, delimiterText) : null;
        }


        private static bool IsValidOpenDelimiter(char openDelimiter)
            => OPEN_DELIMITERS.IndexOf(openDelimiter) >= 0;
    }
}