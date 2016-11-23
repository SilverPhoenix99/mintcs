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

        public CallFrame Caller { get; }

        public iObject Instance { get; }

        public IList<LocalVariable> Arguments { get; }

        public IList<LocalVariable> Locals { get; }

        public IEnumerable<Symbol> VariableNames => Arguments.Concat(Locals).Select(v => v.Name);

        public CallFrame(iObject instance, CallFrame caller = null, params LocalVariable[] arguments)
        {
            Instance = instance;
            Caller = caller;
            Arguments = new List<LocalVariable>(arguments);
            Locals = new List<LocalVariable>();
        }

        public static CallFrame Push(iObject instance, params LocalVariable[] arguments) =>
            CurrentFrame = new CallFrame(instance, CurrentFrame, arguments);

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
