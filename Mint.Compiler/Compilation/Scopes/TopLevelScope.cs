using Mint.Compilation.Scopes.Variables;
using Mint.MethodBinding.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Scopes
{
    public class TopLevelScope : BaseScope
    {
        private readonly IDictionary<Symbol, LocalVariable> initialValues;
        private readonly IList<ScopeVariable> variablesToInitialize;

        public override Scope Parent => this;

        public override Expression Nesting => CompilerUtils.EmptyArray<Module>();

        public TopLevelScope(Compiler compiler, CallFrame callFrame)
            : base(compiler, callFrame == null ? null : Constant(callFrame))
        {
            if(callFrame == null) throw new ArgumentNullException(nameof(callFrame));

            initialValues = callFrame.Locals;
            variablesToInitialize = new List<ScopeVariable>();
        }

        protected override ScopeVariable FindVariable(Symbol name, bool isCurrentScope)
        {
            return TryFindVariableLocally(name)
                ?? TryFindVariableByIndex(name, isCurrentScope)
                ?? TryFindVariableInParent(name, isCurrentScope);
        }

        private ScopeVariable TryFindVariableByIndex(Symbol name, bool isCurrentScope)
        {
            if(!initialValues.ContainsKey(name))
            {
                return null;
            }

            var variable = AddIndexedVariable(name);
            if(!isCurrentScope)
            {
                variablesToInitialize.Add(variable);
            }
            return variable;
        }

        public override Expression CompileBody(Expression body)
        {
            if(variablesToInitialize.Count != 0)
            {
                body = Block(
                    variablesToInitialize.Select(v => v.Local),
                    Block(variablesToInitialize.Select(v => v.VariableExpression())),
                    body
                );
            }

            return base.CompileBody(body);
        }
    }
}
