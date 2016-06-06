using System;
using System.Linq;
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

        private State currentState;
        private string data;
        private int[] lines;
        private readonly Queue<Token> tokens;
        internal BitStack Cmdarg;
        internal BitStack Cond;
        private readonly Stack<iLiteral> literals;

        public string Filename { get; }
        public int DataLength { get; internal set; }
        public int Position { get; internal set; }
        internal int LineJump { get; set; }
        internal bool CommandStart { get; set; }
        internal bool InKwarg { get; set; }
        public bool Eof => Position >= DataLength;
        public int ParenNest { get; set; }
        public bool CanLabel { get; set; }
        internal iLiteral CurrentLiteral => literals.Count == 0 ? null : literals.Peek();
        internal int LeftParenCounter;
        internal Stack<Stack<ISet<string>>> Variables { get; }

        internal State CurrentState
        {
            get { return currentState; }
            set
            {
                currentState = value;
                CanLabel = false;
            }
        }

        private State MainState { get; }
        internal State SharedState { get; }
        internal State ArgState { get; }
        internal State ArgLabeledState { get; }
        internal State BegState { get; }
        internal State ClassState { get; }
        internal State CmdargState { get; }
        internal State DotState { get; }
        internal State EndState { get; }
        internal State EndargState { get; }
        internal State EndfnState { get; }
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
                DataLength = CalculateDataLength(data);
                lines = ResetLines(data, DataLength);
            }
        }

        internal char CurrentChar
        {
            get
            {
                // discount 1 char from length (the virtual eof char)
                if(Position < 0 || Position >= DataLength - 1)
                {
                    return '\0';
                }

                return Data[Position];
            }
        }

        public Lexer(string filename)
        {
            Filename = filename;
            tokens = new Queue<Token>();
            Data = string.Empty;
            literals = new Stack<iLiteral>();
            Variables = new Stack<Stack<ISet<string>>>();

            MainState = new Main(this);
            SharedState = new Shared(this);
            ArgState = new Arg(this);
            ArgLabeledState = new ArgLabeled(this);
            BegState = new Beg(this);
            ClassState = new Class(this);
            CmdargState = new Cmdarg(this);
            DotState = new Dot(this);
            EndState = new End(this);
            EndargState = new Endarg(this);
            EndfnState = new Endfn(this);
            FnameState = new Fname(this);
            FnameFitemState = new FnameFitem(this);
            MidState = new Mid(this);
        }

        private void Reset()
        {
            Position = 0;
            tokens.Clear();
            CurrentState = MainState;
            LineJump = -1;
            InKwarg = false;
            Cond = new BitStack();
            Cmdarg = new BitStack();
            CanLabel = false;
            literals.Clear();
            LeftParenCounter = 0;

            Variables.Clear();
            PushClosedScope();
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
            if(tokens.Count == 0 && !Eof)
            {
                Advance();
            }

            return tokens.Count == 0 ? CreateEofToken() : tokens.Dequeue();
        }

        private void Advance()
        {
            CommandStart = false;

            for(;;)
            {
                var nextState = CurrentState.Advance(null);
                if(nextState == null)
                {
                    break;
                }

                CurrentState = nextState;
            }
        }

        private Token CreateEofToken() => new Token(TokenType.EOF, "$eof", LocationFor(DataLength, 0));

        public void EmitToken(TokenType type, int ts, int te)
        {
            var length = te - ts;
            var text = Data.Substring(ts, length);
            var location = LocationFor(ts, length);
            var token = new Token(type, text, location);
            tokens.Enqueue(token);
        }

        internal LexLocation LocationFor(int position, int length)
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

        public void EmitIntegerToken(int ts, int te, int numBase, bool isRational, bool isImaginary)
        {
            throw new NotImplementedException();
        }

        public void EmitFloatToken(int ts, int te, bool isRational, bool isImaginary)
        {
            throw new NotImplementedException();
        }

        internal void IncrementBraceNest()
        {
            throw new NotImplementedException();
        }

        internal void PushClosedScope()
        {
            Variables.Push(new Stack<ISet<string>>());
            PushOpenScope();
        }

        internal void PopClosedScope()
        {
            Variables.Pop();
        }

        internal void PushOpenScope()
        {
            Variables.Peek().Push(new HashSet<string>());
        }

        internal void PopOpenScope()
        {
            Variables.Peek().Pop();
        }

        internal void DefineVariable(Token token)
        {
            var name = token.Value;
            if(token.Type == TokenType.tLABEL)
            {
                name = name.Substring(0, name.Length - 1);
            }

            if(!IsVariableName(name))
            {
                throw new ArgumentException($"{name} is not a valid local variable name.");
            }

            switch(name)
            {
                case "nil": goto case "self";
                case "true": goto case "self";
                case "false": goto case "self";
                case "__FILE__": goto case "self";
                case "__LINE__": goto case "self";
                case "__ENCODING__": goto case "self";
                case "self":
                    throw new SyntaxError(Filename, token.Location.StartLine, $"Can't assign to {name}");
            }

            if(IsVariableDefined(name))
            {
                return;
            }

            Variables.Peek().Peek().Add(name);
        }

        internal bool IsVariableName(string name) => "$@ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(name[0]) < 0;

        internal bool IsVariableDefined(string name) => Variables.Peek().Any(_ => _.Contains(name));

        internal void DefineVariable(Ast<Token> node)
        {
            if(!node.IsList)
            {
                DefineVariable(node.Value);
                return;
            }

            foreach(var child in node)
            {
                DefineVariable(child);
            }
        }

        internal void DefineArgument(Token token)
        {
            var isDefined = Variables.Peek().Peek().Contains(token.Value);

            if(isDefined)
            {
                throw new SyntaxError(Filename, token.Location.StartLine, "duplicated argument name");
            }

            DefineVariable(token);
        }

        internal void DefineArgument(Ast<Token> node)
        {
            if(!node.IsList)
            {
                DefineArgument(node.Value);
                return;
            }

            foreach(var child in node)
            {
                DefineArgument(child);
            }
        }
    }
}
