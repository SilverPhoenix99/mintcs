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
        public SyntaxNode Result => CurrentSemanticValue;
        public string Filename => Lexer.Filename;


        private void PushCmdarg()
        {
            cmdargStack.Push(Lexer.Cmdarg);
        }


        private void PopCmdarg()
        {
            Lexer.Cmdarg = cmdargStack.Pop();
        }


        private void PushCond()
        {
            condStack.Push(Lexer.Cond);
        }


        private void PopCond()
        {
            Lexer.Cond = condStack.Pop();
        }


        private void PushDef()
        {
            inDefStack.Push(inDef);
        }


        private void PopDef()
        {
            inDef = inDefStack.Pop();
        }
        

        private void PushSingle()
        {
            inSingleStack.Push(inSingle);
        }


        private void PopSingle()
        {
            inSingle = inSingleStack.Pop();
        }


        private void PushLParBeg()
        {
            leftParenCounterStack.Push(Lexer.LeftParenCounter);
        }


        private void PopLParBeg()
        {
            Lexer.LeftParenCounter = leftParenCounterStack.Pop();
        }


        private void PushKwarg()
        {
            inKwargStack.Push(Lexer.InKwarg);
        }


        private void PopKwarg()
        {
            Lexer.InKwarg = inKwargStack.Pop();
        }


        public new SyntaxNode Parse()
        {
            if(base.Parse())
            {
                return Result;
            }

            var token = Scanner.yylval.Token;
            if(token.Type != TokenType.EOF)
            {
                throw new SyntaxError(Filename, token.Location.StartLine, $"unexpected {token.Type}");
            }

            return null;
        }

        
        private void VerifyFormalArgument(Token token)
        {
            var name = token.Text;

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

        
        protected static SyntaxNode EnsureNode(SyntaxNode body, SyntaxNode rescues)
            => new SyntaxNode(
                new Token(kENSURE, "ensure", DefaultLocation()),
                body,
                rescues,
                /*ensureBody: */ new SyntaxNode()
            );


        protected static SyntaxNode CallNode(SyntaxNode methodName, SyntaxNode args)
            => new SyntaxNode(
                new Token(kDOT, ".", DefaultLocation()),
                    /*instance: */ new SyntaxNode(),
                    methodName, args);


        private static LexLocation DefaultLocation() =>
            new LexLocation(-1, -1, -1, -1);

        public static SyntaxNode ParseFile(string filename, string data)
            => new Parser(filename, data, isFile: true).Parse();


        public static SyntaxNode ParseString(string filename, string data)
            => new Parser(filename, data, isFile: false).Parse();


        private class LexerAdapter : AbstractScanner<SyntaxNode, LexLocation>
        {
            public LexerAdapter(Lexer lexer)
            {
                Lexer = lexer;
            }

            public Lexer Lexer { get; }
            
            public override int yylex()
            {
                var token = Lexer.NextToken();
                yylval = new SyntaxNode(token);
                return (int) token.Type;
            }
        }
    }
}
