using QUT.Gppg;
using System;
using System.Collections.Generic;
using static Mint.Parser.TokenType;

namespace Mint.Parser
{
    public partial class Parser
    {
        bool in_def,
             in_single,
             in_defined;

        Stack<BitStack> cmdarg_stack { get; } = new Stack<BitStack>();
        Stack<BitStack> cond_stack { get; } = new Stack<BitStack>();
        BitStack in_def_stack = new BitStack();
        BitStack in_single_stack = new BitStack();
        Stack<int> lpar_beg_stack = new Stack<int>();
        BitStack in_kwarg_stack = new BitStack();

        public Parser(Lexer lexer) : base(new LexerAdapter(lexer)) { }

        public Parser(string data) : this(new Lexer(data)) { }

        public Lexer Lexer => ((LexerAdapter) Scanner).Lexer;

        public Ast<Token> Result => CurrentSemanticValue;
        
        private void PushCmdarg() { cmdarg_stack.Push(Lexer.Cmdarg); }
        private void PopCmdarg()  { Lexer.Cmdarg = cmdarg_stack.Pop(); }

        private void PushCond() { cond_stack.Push(Lexer.Cond); }
        private void PopCond()  { Lexer.Cond = cond_stack.Pop(); }

        private void PushDef() { in_def_stack.Push(in_def); }
        private void PopDef()  { in_def = in_def_stack.Pop(); }

        private void PushSingle() { in_single_stack.Push(in_single); }
        private void PopSingle()  { in_single = in_single_stack.Pop(); }

        private void PushLParBeg() { lpar_beg_stack.Push(Lexer.LParBeg); }
        private void PopLParBeg()  { Lexer.LParBeg = lpar_beg_stack.Pop(); }

        private void PushKwarg() { in_kwarg_stack.Push(Lexer.InKwarg); }
        private void PopKwarg()  { Lexer.InKwarg = in_kwarg_stack.Pop(); }

        public new Ast<Token> Parse()
        {
            if(!((ShiftReduceParser<Ast<Token>, LexLocation>) this).Parse())
            {
                var token = ValueStack[ValueStack.Depth-1];
                throw new SyntaxError(token.Value.Location.Item1, $"unexpected {token.Value.Type}");
            }
            return Result;
        }

        // TODO: Assigns (kASSIGN and tOP_ASGN) will have to know if they are calling into a kASET ('[]=').
        //       They are: kLBRACK2

        protected static readonly Ast<Token> ENSURE_NODE =
            (Ast<Token>) new Token(kENSURE, "ensure", new Tuple<int, int>(-1, -1));

        protected static readonly Ast<Token> CALL_NODE =
            (Ast<Token>) new Token(kDOT, ".", new Tuple<int, int>(-1, -1));

        protected static readonly Ast<Token> EMPTY = new Ast<Token>();

        private static Ast<Token> sexp() => EMPTY;

        private static Ast<Token> sexp(params Ast<Token>[] nodes) => new Ast<Token>(null, nodes);

        public static Ast<Token> Parse(string data)
        {
            return new Parser(data).Parse();
        }

        class LexerAdapter : AbstractScanner<Ast<Token>, LexLocation>
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

            public override void yyerror(string format, params object[] args)
            {
                base.yyerror(format, args);
            }
        }
    }
}
