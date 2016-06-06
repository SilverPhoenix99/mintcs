using QUT.Gppg;
using System.Collections.Generic;
using Mint.Lex;
using static Mint.Parse.TokenType;

namespace Mint.Parse
{
    public partial class Parser
    {
        private bool in_def;
        private bool in_single;

        private readonly Stack<BitStack> cmdarg_stack = new Stack<BitStack>();
        private readonly Stack<BitStack> cond_stack = new Stack<BitStack>();
        private BitStack in_def_stack = new BitStack();
        private BitStack in_single_stack = new BitStack();
        private readonly Stack<int> lpar_beg_stack = new Stack<int>();
        private BitStack in_kwarg_stack = new BitStack();

        public Parser(Lexer lexer) : base(new LexerAdapter(lexer)) { }

        public Parser(string filename, string data) : this(new Lexer(filename, data)) { }

        public Lexer Lexer => ((LexerAdapter) Scanner).Lexer;

        public Ast<Token> Result => CurrentSemanticValue;

        public string Filename => Lexer.Filename;

        private void PushCmdarg() { cmdarg_stack.Push(Lexer.Cmdarg); }
        private void PopCmdarg()  { Lexer.Cmdarg = cmdarg_stack.Pop(); }

        private void PushCond() { cond_stack.Push(Lexer.Cond); }
        private void PopCond()  { Lexer.Cond = cond_stack.Pop(); }

        private void PushDef() { in_def_stack.Push(in_def); }
        private void PopDef()  { in_def = in_def_stack.Pop(); }

        private void PushSingle() { in_single_stack.Push(in_single); }
        private void PopSingle()  { in_single = in_single_stack.Pop(); }

        private void PushLParBeg() { lpar_beg_stack.Push(Lexer.LeftParenCounter); }
        private void PopLParBeg()  { Lexer.LeftParenCounter = lpar_beg_stack.Pop(); }

        private void PushKwarg() { in_kwarg_stack.Push(Lexer.InKwarg); }
        private void PopKwarg()  { Lexer.InKwarg = in_kwarg_stack.Pop(); }

        public new Ast<Token> Parse()
        {
            if(!((ShiftReduceParser<Ast<Token>, LexLocation>) this).Parse())
            {
                var token = Scanner.yylval.Value;
                throw new SyntaxError(Filename, token.Location.StartLine, $"unexpected {token.Type}");
            }
            return Result;
        }

        // TODO: Assigns (kASSIGN and tOP_ASGN) will have to know if they are calling into a kASET ('[]=').
        //       They are: kLBRACK2

        protected static Ast<Token> EnsureNode() =>
            (Ast<Token>) new Token(kENSURE, "ensure", new LexLocation(-1, -1, -1, -1));

        protected static Ast<Token> CallNode() =>
            (Ast<Token>) new Token(kDOT, ".", new LexLocation(-1, -1, -1, -1));

        private static Ast<Token> sexp() => new Ast<Token>();

        private static Ast<Token> sexp(params Ast<Token>[] nodes) => new Ast<Token>(null, nodes);

        public static Ast<Token> Parse(string filename, string data)
        {
            return new Parser(filename, data).Parse();
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
