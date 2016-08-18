using QUT.Gppg;
using System.Collections.Generic;
using Mint.Lex;
using static Mint.Parse.TokenType;

namespace Mint.Parse
{
    public partial class Parser
    {
        private bool inDef;
        private bool inSingle;

        private readonly Stack<BitStack> cmdargStack = new Stack<BitStack>();
        private readonly Stack<BitStack> condStack = new Stack<BitStack>();
        private BitStack inDefStack = new BitStack();
        private BitStack inSingleStack = new BitStack();
        private readonly Stack<int> leftParenCounterStack = new Stack<int>();
        private BitStack inKwargStack = new BitStack();

        public Parser(Lexer lexer)
            : base(new LexerAdapter(lexer))
        { }

        public Parser(string filename, string data, bool isFile = false)
            : this(new Lexer(filename, data, isFile))
        { }

        public Lexer Lexer => ((LexerAdapter) Scanner).Lexer;

        public Ast<Token> Result => CurrentSemanticValue;

        public string Filename => Lexer.Filename;

        private void PushCmdarg() { cmdargStack.Push(Lexer.Cmdarg); }
        private void PopCmdarg()  { Lexer.Cmdarg = cmdargStack.Pop(); }

        private void PushCond() { condStack.Push(Lexer.Cond); }
        private void PopCond()  { Lexer.Cond = condStack.Pop(); }

        private void PushDef() { inDefStack.Push(inDef); }
        private void PopDef()  { inDef = inDefStack.Pop(); }

        private void PushSingle() { inSingleStack.Push(inSingle); }
        private void PopSingle()  { inSingle = inSingleStack.Pop(); }

        private void PushLParBeg() { leftParenCounterStack.Push(Lexer.LeftParenCounter); }
        private void PopLParBeg()  { Lexer.LeftParenCounter = leftParenCounterStack.Pop(); }

        private void PushKwarg() { inKwargStack.Push(Lexer.InKwarg); }
        private void PopKwarg()  { Lexer.InKwarg = inKwargStack.Pop(); }

        public new Ast<Token> Parse()
        {
            if(((ShiftReduceParser<Ast<Token>, LexLocation>) this).Parse())
            {
                return Result;
            }

            var token = Scanner.yylval.Value;
            throw new SyntaxError(Filename, token.Location.StartLine, $"unexpected {token.Type}");
        }

        // TODO: Assigns (kASSIGN and tOP_ASGN) will have to know if they are calling into a kASET ('[]=').
        //       They are: kLBRACK2

        protected static Ast<Token> EnsureNode() =>
            (Ast<Token>) new Token(kENSURE, "ensure", new LexLocation(-1, -1, -1, -1));

        protected static Ast<Token> CallNode() =>
            (Ast<Token>) new Token(kDOT, ".", new LexLocation(-1, -1, -1, -1));

        private static Ast<Token> sexp() => new Ast<Token>();

        private static Ast<Token> sexp(params Ast<Token>[] nodes) => new Ast<Token>(null, nodes);

        public static Ast<Token> ParseFile(string filename, string data)
        {
            return new Parser(filename, data, isFile: true).Parse();
        }

        public static Ast<Token> ParseString(string filename, string data)
        {
            return new Parser(filename, data, isFile: false).Parse();
        }

        private void VerifyFormalArgument(Token token)
        {
            var name = token.Value;

            if(name.StartsWith("@@"))
            {
                throw new SyntaxError(Filename, token.Location.StartLine, "formal argument cannot be a class variable");
            }

            if(name.StartsWith("@"))
            {
                throw new SyntaxError(Filename, token.Location.StartLine, "formal argument cannot be an instance variable");
            }

            if(name.StartsWith("$"))
            {
                throw new SyntaxError(Filename, token.Location.StartLine, "formal argument cannot be a global variable");
            }

            if("ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(name[0]) >= 0)
            {
                throw new SyntaxError(Filename, token.Location.StartLine, "formal argument cannot be a constant");
            }
        }

        private class LexerAdapter : AbstractScanner<Ast<Token>, LexLocation>
        {
            public Lexer Lexer { get; }

            public LexerAdapter(Lexer lexer)
            {
                Lexer = lexer;
            }

            public override int yylex()
            {
                var token = Lexer.NextToken();
                yylval = (Ast<Token>) token;
                return (int) token.Type;
            }
        }
    }
}
