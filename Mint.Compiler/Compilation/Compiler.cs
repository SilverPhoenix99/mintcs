using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mint.Compilation.Components;
using Mint.Compilation.Scopes;
using Mint.Compilation.Selectors;
using Mint.MethodBinding.Methods;
using Mint.Parse;
using static Mint.Parse.TokenType;
using System;

namespace Mint.Compilation
{
    public partial class Compiler : AstVisitor<Token, Expression>
    {
        private readonly IDictionary<TokenType, ComponentSelector> selectors;
        private readonly Stack<Scope> scopes;

        public string Filename { get; }

        public CompilerComponent ListComponent { get; set; }

        public Scope CurrentScope => scopes.Peek();

        public Ast<Token> CurrentNode { get; private set; }

        public Compiler(string filename, CallFrame topLevelFrame)
        {
            if(topLevelFrame == null) throw new ArgumentNullException(nameof(topLevelFrame));

            selectors = new Dictionary<TokenType, ComponentSelector>();
            scopes = new Stack<Scope>();
            Filename = filename;
            InitializeComponents();
            scopes.Push(new TopLevelScope(this, topLevelFrame));
        }

        public Compiler(string filename, iObject instance)
            : this(filename, new CallFrame(instance))
        { }

        private void InitializeComponents()
        {
            ListComponent = new ListCompiler(this);

            Register(ListComponent, kBEGIN, kELSE, tSTRING_DBEG);
            Register(new NilCompiler(this), kNIL);
            Register(new TrueCompiler(this), kTRUE);
            Register(new FalseCompiler(this), kFALSE);
            Register(new IntegerCompiler(this), tINTEGER);
            Register(new FloatCompiler(this), tFLOAT);
            Register(new StringContentCompiler(this), tSTRING_CONTENT);
            Register(new StringCompiler(this), tSTRING_BEG);
            Register(new CharCompiler(this), tCHAR);
            Register(new WordsCompiler(this), tWORDS_BEG, tQWORDS_BEG);
            Register(new SymbolWordsCompiler(this), tSYMBOLS_BEG, tQSYMBOLS_BEG);
            Register(new IfCompiler(this), kIF, kIF_MOD, kELSIF, kUNLESS, kUNLESS_MOD, kQMARK);
            Register(new LineNumberCompiler(this), k__LINE__);
            Register(new FileNameCompiler(this), k__FILE__);
            Register(new RangeCompiler(this), kDOT2, kDOT3);
            Register(new ArrayCompiler(this), kLBRACK);
            Register(new UMinusNumCompiler(this), kUMINUS_NUM);
            Register(new NotCompiler(this), kNOT);
            Register(new NotOperatorCompiler(this), kNOTOP);
            Register(new OrCompiler(this), kOR, kOROP);
            Register(new AndCompiler(this), kAND, kANDOP);
            Register(new EqualCompiler(this), kEQ);
            Register(new CaseEqualsCompiler(this), kEQQ);
            Register(new NotEqualCompiler(this), kNEQ);
            Register(new CompareCompiler(this), kCMP);
            Register(new SelfCompiler(this), kSELF);
            Register(new IdentifierCompiler(this), tIDENTIFIER);
            Register(new LabelCompiler(this), tLABEL);
            Register(new AssocCompiler(this), kASSOC);
            Register(new LabelEndCompiler(this), tLABEL_END);
            Register(new IndexerCompiler(this), kLBRACK2);
            Register(new HashCompiler(this), kLBRACE);
            Register(new SplatCompiler(this), kSTAR);
            Register(new KeySplatCompiler(this), kDSTAR);
            Register(new AdditionCompiler(this), kPLUS);
            Register(new SubtractionCompiler(this), kMINUS);
            Register(new MultiplationCompiler(this), kMUL);
            Register(new PowerCompiler(this), kPOW);
            Register(new DivisionCompiler(this), kDIV);
            Register(new ModulusCompiler(this), kPERCENT);
            Register(new GreaterThanCompiler(this), kGREATER);
            Register(new GreaterOrEqualCompiler(this), kGEQ);
            Register(new LessThanCompiler(this), kLESS);
            Register(new LessOrEqualCompiler(this), kLEQ);
            Register(new LeftShiftCompiler(this), kLSHIFT);
            Register(new RightShiftCompiler(this), kRSHIFT);
            Register(new BinaryAndCompiler(this), kBIN_AND);
            Register(new BinaryOrCompiler(this), kPIPE);
            Register(new NegationCompiler(this), kNEG);
            Register(new XorCompiler(this), kXOR);
            Register(new UnaryPlusCompiler(this), kUPLUS);
            Register(new UnaryMinusCompiler(this), kUMINUS);
            Register(new InstanceVariableCompiler(this), tIVAR);
            Register(new SafeMethodCallCompiler(this), kANDDOT);
            Register(new WhileCompiler(this), kWHILE, kUNTIL);
            Register(new ConstantCompiler(this), tCONSTANT);
            Register(new AbsoluteResolutionCompiler(this), kCOLON3);

            Register(new SymbolSelector(this), tSYMBEG);
            Register(new MethodCallSelector(this), kDOT);
            Register(new AssignSelector(this), kASSIGN, tOP_ASGN);
            Register(new WhileModSelector(this), kWHILE_MOD, kUNTIL_MOD);
            Register(new ConstantResolutionSelector(this), kCOLON2);
            Register(new ModuleSelector(this), kMODULE);
        }

        public void Register(CompilerComponent component, params TokenType[] types)
        {
            foreach(var type in types)
            {
                Register(component, type);
            }
        }

        public void Register(CompilerComponent component, TokenType type)
        {
            Register(new UnconditionalSelector(component), type);
        }

        public void Register(ComponentSelector selector, params TokenType[] types)
        {
            foreach(var type in types)
            {
                Register(selector, type);
            }
        }

        public void Register(ComponentSelector selector, TokenType type)
        {
            selectors[type] = selector;
        }

        public void StartScope(Scope scope) => scopes.Push(scope);

        public Scope EndScope() => scopes.Pop();

        public Expression Compile(Ast<Token> node)
        {
            var scope = CurrentScope;
            var body = node.Accept(this);
            body = scope.CompileBody(body);

            return Expression.Block(
                Expression.Assign(CallFrame.Expressions.Current(), scope.CallFrame),
                Expression.TryFinally(body, CallFrame.Expressions.Pop())
            );
        }

        public Expression Visit(Ast<Token> node)
        {
            var previousNode = CurrentNode;
            CurrentNode = node;
            var component = GetComponentOrThrow(node);
            var expression = component.Compile();
            CurrentNode = previousNode;
            return expression;
        }

        private CompilerComponent GetComponentOrThrow(Ast<Token> node)
        {
            if(node.IsList)
            {
                return ListComponent;
            }

            var type = node.Value.Type;
            ComponentSelector selector;
            if(selectors.TryGetValue(type, out selector))
            {
                return selector.Select();
            }

            throw new UnregisteredTokenError(type.ToString());
        }

        public Expression BuildNesting()
        {
            var nestingExpressions = scopes.Select(_ => _.Nesting).Where(_ => _ != null).ToArray();

            return nestingExpressions.Length == 0
                 ? Expression.Constant(System.Array.Empty<Module>())
                 : Expression.NewArrayInit(typeof(Module), nestingExpressions).Cast<IEnumerable<Module>>();
        }
    }
}
