using System;
using System.Linq;
using System.Collections.Generic;
using Mint.Lex.States;
using Mint.Parse;
using QUT.Gppg;
using static Mint.Parse.TokenType;
using State = Mint.Lex.States.State;

namespace Mint.Lex
{
    public class Lexer
    {
        private static readonly char[] EOF_CHARS = { '\0', '\x4', '\x1a' };
        public static int TabWidth = 8;


        private readonly State initialState;
        private State currentState;
        private string data;
        private int[] lines;
        private readonly Queue<Token> tokens;
        internal BitStack Cmdarg;
        internal BitStack Cond;
        private readonly Stack<Literal> literals;


        public Lexer(string filename, string data = "", bool isFile = false) : this()
        {
            Filename = filename;
            initialState = isFile ? MainState : BegState;
            Data = data;
        }


        public string Filename { get; }
        public int DataLength { get; internal set; }
        public int Position { get; internal set; }
        internal int LineJump { get; set; }
        internal bool CommandStart { get; set; }
        internal bool InKwarg { get; set; }
        private bool Eof => Position >= DataLength;
        public int ParenNest { get; set; }
        public bool CanLabel { get; set; }
        internal Literal CurrentLiteral => literals.Count == 0 ? null : literals.Peek();
        internal int LeftParenCounter;
        private Stack<Stack<ISet<string>>> Variables { get; }
        internal bool Retry { get; set; }
        internal char CurrentChar => Position < 0 || Position >= DataLength ? '\0' : Data[Position];
        private State MainState { get; }
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


        internal State CurrentState
        {
            get => currentState;
            set
            {
                currentState = value;
                CanLabel = false;
                CommandStart = false;
            }
        }


        public string Data
        {
            get => data;
            set
            {
                data = value ?? throw new ArgumentNullException(nameof(value));
                Reset();
                DataLength = CalculateDataLength(data);
                lines = ResetLines(data, DataLength);
            }
        }


        private Lexer()
        {
            tokens = new Queue<Token>();
            literals = new Stack<Literal>();
            Variables = new Stack<Stack<ISet<string>>>();
            MainState = new Main(this);
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
            CurrentState = initialState;
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

            if(dataLength > data.Length)
            {
                dataLength = data.Length;
            }

            for(var i = 0; i < dataLength; i++)
            {
                var c = data[i];
                if(Array.IndexOf(EOF_CHARS, c) >= 0)
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
            return index >= 0 ? index : data.Length;
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
            do
            {
                Retry = false;
                CurrentState.Advance();
            } while(Retry);
        }


        private Token CreateEofToken()
            => new Token(EOF, "$eof", LocationFor(DataLength, 0));


        public Token EmitToken(TokenType type, int ts, int te)
        {
            var token = GenerateToken(type, ts, te);
            EmitToken(token);
            return token;
        }


        public Token GenerateToken(TokenType type, int ts, int te)
        {
            var text = TextAt(ts, te);
            var length = te - ts;
            var location = LocationFor(ts, length);
            return new Token(type, text, location);
        }


        private void EmitToken(Token token)
        {
            tokens.Enqueue(token);
        }


        internal string TextAt(int ts, int te)
        {
            var length = te - ts;
            return Data.Substring(ts, length);
        }


        private LexLocation LocationFor(int position, int length)
        {
            var start = LocationFor(position);
            var end = LocationFor(position + length);
            return new LexLocation(start.Item1, start.Item2, end.Item1, end.Item2);
        }


        private Tuple<int, int> LocationFor(int position)
        {
            var line = LineAt(position);
            var column = position - lines[line - 1] + 1;
            return new Tuple<int, int>(line, column);
        }


        internal int LineAt(int position)
        {
            var line = Array.BinarySearch(lines, position) + 1;
            return line < 0 ? -line : line;
        }


        public void EmitStringBeginToken(int ts, int te, bool canLabel = false, State endState = null)
        {
            var literal = new StringLiteral(this, ts, te, canLabel, endState);
            EmitLiteralBeginToken(literal);
        }

        public void EmitStringContentToken(int ts, int te)
        {
            EmitToken(tSTRING_CONTENT, ts, te);
        }


        public void EmitHeredocToken(int ts, int te)
        {
            var literal = new Heredoc(this, ts, te);
            EmitLiteralBeginToken(literal);
        }


        private void EmitLiteralBeginToken(Literal literal)
        {
            literals.Push(literal);
            CurrentState = literal;

            EmitToken(literal.BeginToken);
        }


        public void EmitIntegerToken(int ts, int te, int numBase, bool isRational, bool isImaginary)
        {
            var type = isImaginary ? tIMAGINARY
                     : isRational ? tRATIONAL
                     : tINTEGER;

            var token = EmitToken(type, ts, te);
            token.Properties["num_base"] = numBase;
        }


        public void EmitFloatToken(int ts, int te, bool isRational, bool isImaginary)
        {
            var type = isImaginary ? tIMAGINARY
                     : isRational ? tRATIONAL
                     : tFLOAT;

            EmitToken(type, ts, te);
        }


        public int NextLinePosition()
        {
            if(LineJump > Position)
            {
                return LineJump;
            }

            var line = Array.BinarySearch(lines, Position);

            if(line < 0)
            {
                line = ~line;
            }

            return line < lines.Length ? lines[line] : DataLength;
        }


        internal void IncrementBraceCount()
        {
            if(literals.Count > 0)
            {
                literals.Peek().BraceCount++;
            }
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


        internal void PopLiteral()
            => literals.Pop();


        private void DefineVariable(Token token)
        {
            if(token.Type == tIVAR
            || token.Type == tCVAR
            || token.Type == tGVAR
            || token.Type == tNTH_REF
            || token.Type == tBACK_REF
            || token.Type == tCONSTANT)
            {
                return;
            }

            var name = token.Text;
            if(token.Type == tLABEL)
            {
                name = name.Substring(0, name.Length - 1);
            }

            if(!IsVariableName(name))
            {
                throw new ArgumentException($"{name} is not a valid local variable name.");
            }

            CheckAssignable(token, name);

            if(IsVariableDefined(name))
            {
                return;
            }

            Variables.Peek().Peek().Add(name);
        }


        private static bool IsVariableName(string name)
            => "$@ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(name[0]) < 0;


        private void CheckAssignable(Token token, string name)
        {
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
        }


        internal bool IsVariableDefined(string name)
            => Variables.Peek().Any(_ => _.Contains(name));


        internal void DefineVariable(SyntaxNode node)
        {
            if(!node.IsList)
            {
                DefineVariable(node.Token);
                return;
            }

            foreach(var child in node)
            {
                DefineVariable(child);
            }
        }


        private void DefineArgument(Token token)
        {
            var isDefined = Variables.Peek().Peek().Contains(token.Text);

            if(isDefined)
            {
                throw new SyntaxError(Filename, token.Location.StartLine, "duplicated argument name");
            }

            DefineVariable(token);
        }


        internal void DefineArgument(SyntaxNode node)
        {
            if(!node.IsList)
            {
                DefineArgument(node.Token);
                return;
            }

            foreach(var child in node)
            {
                DefineArgument(child);
            }
        }
    }
}
