using System.Collections.Generic;
using System.Linq.Expressions;
using Mint.Compilation.Components;
using Mint.Compilation.Selectors;
using Mint.Parse;
using static System.Linq.Expressions.Expression;
using static Mint.Parse.TokenType;

namespace Mint.Compilation
{
    public partial class Compiler
    {
        internal static readonly Expression NIL = Constant(new NilClass(), typeof(iObject));
        internal static readonly Expression FALSE = Constant(new FalseClass(), typeof(iObject));
        internal static readonly Expression TRUE = Constant(new TrueClass(), typeof(iObject));

        private readonly Stack<ShiftState> shifting;
        private readonly Stack<ReduceState> reducing;
        private readonly Queue<Expression> outputs;
        private readonly IDictionary<TokenType, ComponentSelector> selectors;
        private ShiftState currentShifting;
        private ReduceState currentReducing;

        public string Filename { get; }
        public Scope CurrentScope { get; private set; }
        public CompilerComponent ListComponent { get; set; }
        public Ast<Token> CurrentNode { get; private set; }

        public Compiler(string filename, Closure binding, Ast<Token> root)
        {
            shifting = new Stack<ShiftState>();
            reducing = new Stack<ReduceState>();
            outputs = new Queue<Expression>();
            selectors = new Dictionary<TokenType, ComponentSelector>();

            Filename = filename;
            CurrentScope = new Scope(ScopeType.Method, binding);

            shifting.Push(new ShiftState(root));

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            ListComponent = new ListCompiler(this);

            Register(ListComponent, kBEGIN, kELSE, tSTRING_DBEG);
            Register(new ConstantCompiler(this), kFALSE, kNIL, kTRUE);
            Register(new IntegerCompiler(this), tINTEGER);
            Register(new FloatCompiler(this), tFLOAT);
            Register(new StringContentCompiler(this), tSTRING_CONTENT);
            Register(new StringCompiler(this), tSTRING_BEG);
            Register(new CharCompiler(this), tCHAR);
            Register(new SymbolCompiler(this), tSYMBEG);
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
            Register(new NotEqualCompiler(this), kNEQ);
            Register(new SelfCompiler(this), kSELF);
            Register(new IdentifierCompiler(this), tIDENTIFIER);
            Register(new LabelCompiler(this), tLABEL);
            
            Register(new AssignSelector(this), kASSIGN);
        }

        public void Register(CompilerComponent component, TokenType type)
        {
            selectors[type] = new UnconditionalSelector(component);
        }

        public void Register(CompilerComponent component, params TokenType[] types)
        {
            foreach(var type in types)
            {
                Register(component, type);
            }
        }

        public void Register(ComponentSelector selector, TokenType type)
        {
            selectors[type] = selector;
        }

        public void Register(ComponentSelector selector, params TokenType[] types)
        {
            foreach(var type in types)
            {
                Register(selector, type);
            }
        }

        public Expression Compile()
        {
            if(ListComponent == null)
            {
                throw new UnregisteredTokenError("ListComponent is null");
            }

            for(;;)
            {
                if(Reduce())
                {
                    continue;
                }

                if(shifting.Count == 0)
                {
                    break;
                }

                Shift();
            }

            if(reducing.Count != 0)
            {
                throw new IncompleteCompilationError("Compilation finished but some nodes were not reduced.");
            }

            return outputs.Count == 0 ? NIL
                 : outputs.Count > 1 ? Block(outputs)
                 : outputs.Dequeue();
        }

        private bool Reduce()
        {
            var canReduce = reducing.Count != 0 && reducing.Peek().CanReduce;
            if(!canReduce)
            {
                return false;
            }

            try
            {
                currentReducing = reducing.Pop();
                CurrentNode = currentReducing.Node;

                var expression = currentReducing.Component.Reduce();

                if(currentReducing.Children.Count != 0)
                {
                    throw new IncompleteCompilationError("More nodes were shifted than needed.");
                }

                if(reducing.Count == 0)
                {
                    outputs.Enqueue(expression);
                }
                else
                {
                    reducing.Peek().Children.Enqueue(expression);
                }

                return true;
            }
            finally
            {
                currentReducing = null;
                CurrentNode = null;
            }
        }

        private void Shift()
        {
            try
            {
                currentShifting = shifting.Pop();
                CurrentNode = currentShifting.Node;

                var component = GetComponentOrThrow();
                component.Shift();

                ShiftChildren(currentShifting.Children);

                var childCount = currentShifting.Children.Count;
                var reducingState = new ReduceState(CurrentNode, component, childCount);
                reducing.Push(reducingState);
            }
            finally
            {
                currentShifting = null;
                CurrentNode = null;
            }
        }

        private CompilerComponent GetComponentOrThrow()
        {
            if(CurrentNode.IsList)
            {
                return ListComponent;
            }

            var type = CurrentNode.Value.Type;
            ComponentSelector selector;
            if(selectors.TryGetValue(type, out selector))
            {
                return selector.Select();
            }

            throw new UnregisteredTokenError(type.ToString());
        }

        private void ShiftChildren(IEnumerable<Ast<Token>> items)
        {
            foreach(var child in items)
            {
                shifting.Push(new ShiftState(child));
            }
        }

        public void Push(Ast<Token> node) => currentShifting.Children.Push(node);

        public Expression Pop() => currentReducing.Children.Dequeue();
    }
}
