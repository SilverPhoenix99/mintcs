using System.Collections.Generic;
using System.Linq.Expressions;
using Mint.MethodBinding.Methods;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Scopes
{
    using CallFrame_Expressions = Mint.MethodBinding.Methods.CallFrame.Expressions;

    public abstract class BaseScope : Scope
    {
        public Compiler Compiler { get; }

        public abstract Scope Parent { get; }

        public abstract Expression Nesting { get; }

        public Expression CallFrame { get; set; }

        public ParameterExpression Locals { get; }

        public MemberExpression Instance => CallFrame_Expressions.Instance(CallFrame);

        public IDictionary<Symbol, ScopeVariable> Variables { get; }

        protected BaseScope(Compiler compiler)
        {
            Compiler = compiler;
            CallFrame = Variable(typeof(CallFrame), "frame");
            Locals = Variable(typeof(IList<LocalVariable>), "locals");
            Variables = new LinkedDictionary<Symbol, ScopeVariable>();
        }

        public Expression LocalsAdd(Symbol variableName, ParameterExpression variable)
        {
            // new LocalVariable(@variableName)
            var localVariable = LocalVariable.Expressions.New(Constant(variableName));

            // $locals.Add($variable = ...)
            return CallFrame_Expressions.Locals_Add(Locals, Assign(variable, localVariable));
        }

        public Expression LocalsIndex(ParameterExpression variable, int index) =>
            // $variable = $locals[@index]
            Assign(variable, CallFrame_Expressions.Locals_Indexer(Locals, Constant(index)));

        public Expression CompileCallFrameInitialization()
        {
            if(CallFrame.NodeType == ExpressionType.Constant)
            {
                return CompileCallFrameConstantInitialization();
            }

            if(CallFrame.NodeType == ExpressionType.Parameter)
            {
                return CompileCallFrameVariableInitialization();
            }

            throw new System.NotImplementedException("CallFrame not a constant");
        }

        private Expression CompileCallFrameConstantInitialization()
        {
            return Block(
                new ParameterExpression[] { Locals },

                // CallFrame.Current = @__CallFrame_1;
                Assign(CallFrame_Expressions.Current(), CallFrame),

                // $locals = @__CallFrame_1.Locals;
                Assign(Locals, CallFrame_Expressions.Locals(CallFrame)),

                CompileLocalsInitialization()
            );
        }

        private Expression CompileCallFrameVariableInitialization()
        {
            throw new System.NotImplementedException(nameof(CompileCallFrameVariableInitialization));
        }

        private Expression CompileLocalsInitialization()
        {
             return Empty();
        }
    }
}
