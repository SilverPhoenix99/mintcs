using System.Collections.Generic;
using System.Linq.Expressions;
using Mint.MethodBinding.Methods;

namespace Mint.Compilation.Scopes
{
    using System;
    using CallFrame_Expressions = Mint.MethodBinding.Methods.CallFrame.Expressions;

    public abstract class BaseScope : Scope
    {
        public Compiler Compiler { get; }

        public abstract Scope Parent { get; }

        public abstract Expression Nesting { get; }

        public Expression CallFrame { get; set; }

        public MemberExpression Self => CallFrame_Expressions.Instance(CallFrame);

        public IDictionary<Symbol, ScopeVariable> Variables { get; }

        protected BaseScope(Compiler compiler)
        {
            Compiler = compiler;
            CallFrame = Expression.Variable(typeof(CallFrame), "frame");
            Variables = new LinkedDictionary<Symbol, ScopeVariable>();
        }
    }
}
