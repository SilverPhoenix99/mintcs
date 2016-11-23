using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Mint.MethodBinding.Methods;

namespace Mint
{
    public class CompilerClosure
    {
        private class Variable
        {
            public readonly Symbol Name;
            public readonly int Index;
            public readonly ParameterExpression Local;
            public readonly Expression InitialValue;

            public Variable(Symbol name, int index, ParameterExpression local, Expression initialValue)
            {
                Name = name;
                Index = index;
                Local = local;
                InitialValue = initialValue;
            }
        }

        private readonly IDictionary<Symbol, Variable> variables;

        public Expression CallFrame { get; set; }

        public MemberExpression Self => Mint.MethodBinding.Methods.CallFrame.Expressions.Instance(CallFrame);

        public CompilerClosure()
        {
            CallFrame = Expression.Variable(typeof(CallFrame), "frame");
            variables = new LinkedDictionary<Symbol, Variable>();
        }

        public void AddVariable(Symbol name, ParameterExpression local = null, Expression initialValue = null)
        {
            if(local == null)
            {
                local = Expression.Variable(typeof(LocalVariable), name.Name);
            }

            var index = variables.Count;
            var variable = new Variable(name, index, local, initialValue);
            variables.Add(name, variable);
        }

        public ParameterExpression GetLocal(Symbol name) => variables[name].Local;

        public int IndexOf(Symbol name) => variables[name].Index;

        public bool IsDefined(Symbol name) => variables.ContainsKey(name);
    }
}
