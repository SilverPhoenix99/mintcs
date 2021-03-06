﻿using System.Linq.Expressions;
using Mint.MethodBinding;

namespace Mint.Compilation.Scopes.Variables
{
    public class ReferencedScopeVariable : BaseScopeVariable
    {
        public ReferencedScopeVariable(Scope scope, ScopeVariable baseVariable)
            : base(scope, baseVariable.Name, baseVariable.Local)
        { }

        protected override Expression CompileInitialization() =>
            // CallFrame.Current.AddLocal($Local)
            CallFrame.Expressions.AddLocal(CallFrame.Expressions.Current(), Local);
    }
}
