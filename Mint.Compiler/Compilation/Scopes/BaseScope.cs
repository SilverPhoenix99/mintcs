using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mint.Compilation.Scopes.Variables;
using static System.Linq.Expressions.Expression;
using CallFrame_Expressions = Mint.MethodBinding.Methods.CallFrame.Expressions;

namespace Mint.Compilation.Scopes
{
    public abstract class BaseScope : Scope
    {
        protected readonly IDictionary<Symbol, ScopeVariable> variables;

        public Compiler Compiler { get; }

        public abstract Scope Parent { get; }

        public abstract Expression Nesting { get; }

        public Expression CallFrame { get; }

        public virtual Expression Instance => CallFrame_Expressions.Instance(CallFrame);

        public virtual Expression Module => CallFrame_Expressions.Module(CallFrame);

        protected BaseScope(Compiler compiler, Expression callFrame = null)
        {
            Compiler = compiler;
            CallFrame = callFrame ?? CallFrame_Expressions.Current();
            variables = new LinkedDictionary<Symbol, ScopeVariable>();
        }

        public ScopeVariable AddNewVariable(Symbol name, ParameterExpression local = null) =>
            AddVariable(new NewScopeVariable(this, name, local));

        public ScopeVariable AddReferencedVariable(ScopeVariable baseVariable) =>
            AddVariable(new ReferencedScopeVariable(this, baseVariable));

        public ScopeVariable AddIndexedVariable(Symbol name) =>
            AddVariable(new IndexedScopeVariable(this, name));

        public ScopeVariable AddPreInitializedVariable(Symbol name, ParameterExpression local) =>
            AddVariable(new PreInitializedScopeVariable(this, name, local));

        private ScopeVariable AddVariable(ScopeVariable scopeVariable)
        {
            variables.Add(scopeVariable.Name, scopeVariable);
            return scopeVariable;
        }

        public ScopeVariable FindVariable(Symbol name) => FindVariable(name, true);

        protected virtual ScopeVariable FindVariable(Symbol name, bool isCurrentScope)
        {
            return TryFindVariableLocally(name)
                ?? TryFindVariableInParent(name, isCurrentScope);
        }

        protected ScopeVariable TryFindVariableLocally(Symbol name)
        {
            ScopeVariable variable;
            variables.TryGetValue(name, out variable);
            return variable;
        }

        protected ScopeVariable TryFindVariableInParent(Symbol name, bool isCurrentScope)
        {
            if(Parent == null || Parent == this)
            {
                return null;
            }

            var variable = ((BaseScope) Parent).FindVariable(name, false);

            if(variable != null && isCurrentScope)
            {
                variable = AddReferencedVariable(variable);
            }

            return variable;
        }

        public Expression AddLocal(Expression localVariable) =>
            // $callFrame.AddLocal(...)
            CallFrame_Expressions.AddLocal(CallFrame, localVariable);

        public virtual Expression CompileBody(Expression body)
        {
            if(variables.Count == 0)
            {
                return body;
            }

            return Block(
                variables.Select(v => v.Value.Local),
                body
            );
        }
    }
}
