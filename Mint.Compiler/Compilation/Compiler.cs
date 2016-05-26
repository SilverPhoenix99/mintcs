using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Mint.Compilation.Components;
using Mint.Parse;
using static System.Linq.Expressions.Expression;
using static Mint.Parse.TokenType;

namespace Mint.Compilation
{
    using Ast = Ast<Token>;
    using InputState = Tuple<Ast<Token>, Stack<Ast<Token>>>;
    using ReduceState = Tuple<Ast<Token>, int, CompilerComponent, Queue<Expression>>;

    public class Compiler
    {
        internal static readonly Expression NIL = Constant(new NilClass(), typeof(iObject));
        internal static readonly Expression FALSE = Constant(new FalseClass(), typeof(iObject));
        internal static readonly Expression TRUE = Constant(new TrueClass(), typeof(iObject));

        private readonly Stack<InputState> inputs;
        private readonly Stack<ReduceState> reducing;
        private readonly Queue<Expression> outputs;
        private readonly IDictionary<TokenType, CompilerComponent> components;
        private InputState currentShifting;
        private ReduceState currentReducing;

        public string Filename { get; }
        public Scope CurrentScope { get; private set; }
        public CompilerComponent ListComponent { get; set; }
        public Ast CurrentNode { get; private set; }

        public Compiler(string filename, Closure binding, Ast root)
        {
            inputs = new Stack<InputState>();
            reducing = new Stack<ReduceState>();
            outputs = new Queue<Expression>();
            components = new Dictionary<TokenType, CompilerComponent>();

            Filename = filename;
            CurrentScope = new Scope(ScopeType.Method, binding);

            inputs.Push(new InputState(root, new Stack<Ast>()));

            RegisterDefaultComponents();
        }

        private void RegisterDefaultComponents()
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
            components[type] = component;
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

                if(inputs.Count == 0)
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
            var canReduce = reducing.Count != 0 && reducing.Peek().Item4.Count == reducing.Peek().Item2;
            if(!canReduce)
            {
                return false;
            }

            try
            {
                currentReducing = reducing.Pop();
                CurrentNode = currentReducing.Item1;

                var component = currentReducing.Item3;
                var expression = component.Reduce();

                if(currentReducing.Item4.Count != 0)
                {
                    throw new IncompleteCompilationError("More nodes were shifted than needed.");
                }

                if(reducing.Count == 0)
                {
                    outputs.Enqueue(expression);
                }
                else
                {
                    reducing.Peek().Item4.Enqueue(expression);
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
                currentShifting = inputs.Pop();
                CurrentNode = currentShifting.Item1;

                var component = GetComponentOrThrow();
                component.Shift();

                PushChildInputs(currentShifting.Item2);
                
                var buildingState = CreateBuildingState(currentShifting, component);
                reducing.Push(buildingState);
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
            CompilerComponent component;
            if(components.TryGetValue(type, out component))
            {
                return component;
            }

            throw new UnregisteredTokenError(type.ToString());
        }

        private void PushChildInputs(IEnumerable<Ast> items)
        {
            foreach(var child in items)
            {
                inputs.Push(new InputState(child, new Stack<Ast>()));
            }
        }

        private static ReduceState CreateBuildingState(InputState input, CompilerComponent component)
        {
            var node = input.Item1;
            var count = input.Item2.Count;
            return new ReduceState(node, count, component, new Queue<Expression>());
        }

        public void Push(Ast node) => currentShifting.Item2.Push(node);

        public Expression Pop() => currentReducing.Item4.Dequeue();
    }
}
