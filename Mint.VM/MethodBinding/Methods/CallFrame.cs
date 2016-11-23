using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;

namespace Mint.MethodBinding.Methods
{
    public class CallFrame
    {
        [ThreadStatic]
	    public static CallFrame CurrentFrame;

        private LocalVariable[] variables;

        public CallFrame Caller { get; }

        public iObject Instance { get; }

        public int NumArguments { get; }

        public IList<LocalVariable> Variables => variables;

        public IList<LocalVariable> Arguments => new ArraySegment<LocalVariable>(variables, 0, NumArguments);

        public int NumLocals => variables.Length - NumArguments;

        public IList<LocalVariable> Locals => new ArraySegment<LocalVariable>(variables, NumArguments, NumLocals);

        public IEnumerable<Symbol> VariableNames => variables.Select(v => v.Name);

        public CallFrame(iObject instance, int numArguments, CallFrame caller = null, params LocalVariable[] variables)
        {
            if(numArguments > variables.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(numArguments),
                    numArguments,
                    $"{nameof(numArguments)} ({numArguments}) cannot be larger than total number of variables ({variables.Length})"
                );
            }

            Instance = instance;
            NumArguments = numArguments;
            Caller = caller;
            this.variables = variables;
        }

        public static CallFrame Push(iObject instance, int numArguments, params LocalVariable[] variables) =>
            CurrentFrame = new CallFrame(instance, numArguments, CurrentFrame, variables);

        public static void Pop() => CurrentFrame = CurrentFrame?.Caller;

        public static class Reflection
        {
            public static readonly PropertyInfo Instance = Reflector<CallFrame>.Property(_ => _.Instance);
        }

        public static class Expressions
        {
            public static MemberExpression Instance(Expression callFrame) =>
                Expression.Property(callFrame, Reflection.Instance);
        }
    }
}
