using System;
using System.Collections.Generic;
using Mint.Lex.States;
using Mint.Parse;

namespace Mint.Lex
{
    public class Lexer
    {
        private static readonly char[] EOF_CHARS = { '\0', '\x4', '\x1a' };

        private string data;
        private readonly Queue<Token> tokens;

        public string Filename { get; }
        
        public string Data
        {
            get { return data; }
            set
            {
                if(data == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                data = value;
                Length = CalculateDataLength();
                Reset();
            }
        }

        public int Length { get; private set; }

        public int Position { get; internal set; }

        internal char CurrentChar
        {
            get
            {
                // discount 1 char from length (the virtual eof char)
                if(Position < 0 || Position >= Length - 1)
                {
                    return '\0';
                }

                return Data[Position];
            }
        }

        private State CurrentState { get; set; }

        private State MainState { get; }
        internal State BegState { get; }
        internal ExprShared SharedState { get; }

        internal int LineJump { get; set; }

        public Lexer(string filename)
        {
            Filename = filename;
            data = string.Empty;
            tokens = new Queue<Token>();
            MainState = new Main(this);
            BegState = new ExprBeg(this);
            Reset();
        }

        private void Reset()
        {
            Position = 0;
            tokens.Clear();
            CurrentState = MainState;
            LineJump = -1;
        }

        private int CalculateDataLength()
        {
            var index = Data.IndexOfAny(EOF_CHARS);

            if(index >= 0)
            {
                return index;
            }

            // 1 char offset to virtually append a '\0' (eof char)
            return Data.Length + 1;
        }

        public Token NextToken()
        {
            if(tokens.Count == 0)
            {
                Advance();
            }

            return tokens.Count == 0 ? null : tokens.Dequeue();
        }

        private void Advance()
        {
            for(;;)
            {
                var nextState = CurrentState.Advance();
                if(nextState == null)
                {
                    break;
                }

                CurrentState = nextState;
            }
        }

        public void SetMainState() => CurrentState = MainState;

        public void SetBegState() => CurrentState = BegState;
    }
}
