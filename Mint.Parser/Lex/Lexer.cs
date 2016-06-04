using System;
using System.Collections.Generic;
using Mint.Lex.States;
using Mint.Parse;
using QUT.Gppg;
using State = Mint.Lex.States.State;

namespace Mint.Lex
{
    public class Lexer
    {
        private static readonly char[] EOF_CHARS = { '\0', '\x4', '\x1a' };

        private string data;
        private int[] lines;
        private readonly Queue<Token> tokens;

        public string Filename { get; }
        public int Length { get; private set; }
        public int Position { get; internal set; }
        internal int LineJump { get; set; }
        internal bool CommandStart { get; set; }
        internal bool InKwarg { get; set; }
        internal LexLocation CurrentLocation => LocationFor(Position, 0);
        internal int CurrentLine => CurrentLocation.StartLine;

        internal State CurrentState { get; set; }

        private State MainState { get; }
        internal Shared SharedState { get; }
        internal State ArgState { get; }
        internal State ArgLabeledState { get; }
        internal State BegState { get; }
        internal State BegLabelState { get; }
        internal State ClassState { get; }
        internal State CmdargState { get; }
        internal State DotState { get; }
        internal State EndState { get; }
        internal State EndLabelState { get; }
        internal State EndargState { get; }
        internal State EndfnState { get; }
        internal State EndfnLabelState { get; }
        internal State FnameState { get; }
        internal State FnameFitemState { get; }
        internal State MidState { get; }

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
                Reset();
                Length = CalculateDataLength(data);
                lines = ResetLines(data, Length);
            }
        }

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

        public Lexer(string filename)
        {
            Filename = filename;
            data = string.Empty;
            tokens = new Queue<Token>();

            MainState = new Main(this);
            SharedState = new Shared(this);
            ArgState = new Arg(this);
            ArgLabeledState = new ArgLabeled(this);
            BegState = new Beg(this);
            BegLabelState = new BegLabel(this);
            ClassState = new Class(this);
            CmdargState = new Cmdarg(this);
            DotState = new Dot(this);
            EndState = new End(this);
            EndLabelState = new EndLabel(this);
            EndargState = new Endarg(this);
            EndfnState = new Endfn(this);
            EndfnLabelState = new EndfnLabel(this);
            FnameState = new Fname(this);
            FnameFitemState = new FnameFitem(this);
            MidState = new Mid(this);

            Reset();
        }

        private void Reset()
        {
            Position = 0;
            tokens.Clear();
            CurrentState = MainState;
            LineJump = -1;
            InKwarg = false;
        }

        private static int[] ResetLines(string data, int dataLength)
        {
            var lines = new List<int> { 0 };

            for(var i = 0; i < dataLength; i++)
            {
                var c = data[i];
                if(c == 0 || c == 0x4 || c == 0x1a)
                {
                    break;
                }

                if(c == '\n')
                {
                    lines.Add(i + 1);
                }
            }

            return lines.ToArray();
        }

        private static int CalculateDataLength(string data)
        {
            var index = data.IndexOfAny(EOF_CHARS);

            if(index >= 0)
            {
                return index;
            }

            // 1 char offset to virtually append a '\0' (eof char)
            return data.Length + 1;
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
            CommandStart = false;

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
        public void SetArgState() => CurrentState = ArgState;
        public void SetArgLabeledState() => CurrentState = ArgLabeledState;
        public void SetBegState() => CurrentState = BegState;
        public void SetBegLabelState() => CurrentState = BegLabelState;
        public void SetClassState() => CurrentState = ClassState;
        public void SetCmdargState() => CurrentState = CmdargState;
        public void SetDotState() => CurrentState = DotState;
        public void SetEndState() => CurrentState = EndState;
        public void SetEndLabelState() => CurrentState = EndLabelState;
        public void SetEndargState() => CurrentState = EndargState;
        public void SetEndfnState() => CurrentState = EndfnState;
        public void SetEndfnLabelState() => CurrentState = EndfnLabelState;
        public void SetFnameState() => CurrentState = FnameState;
        public void SetFnameFitemState() => CurrentState = FnameFitemState;
        public void SetMidState() => CurrentState = MidState;

        public void EmitToken(TokenType type, int ts, int te)
        {
            var length = te - ts;
            var text = Data.Substring(ts, length);
            var location = LocationFor(ts, length);
            var token = new Token(type, text, location);
            tokens.Enqueue(token);
        }

        private LexLocation LocationFor(int position, int length)
        {
            var line = Array.BinarySearch(lines, position) + 1;
            line = Math.Abs(line < 0 ? -line : line);
            var column = position - lines[line - 1] + 1;
            return new LexLocation(line, column, line, column + length);
        }

        public void EmitStringToken(int ts, int te)
        {
            throw new NotImplementedException();
        }

        public void EmitLabelableStringToken(int ts, int te)
        {
            throw new NotImplementedException();
        }

        public void EmitHeredocToken(int ts, int te)
        {
            throw new NotImplementedException();
        }
    }
}
