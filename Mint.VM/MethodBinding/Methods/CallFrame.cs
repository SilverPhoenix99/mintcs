using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.Methods
{
    public class CallFrame
    {
        [ThreadStatic]
	    public static CallFrame Current;

        public CallFrame Caller { get; }

        public iObject Instance { get; }

        public IList<LocalVariable> Arguments { get; }

        public IList<LocalVariable> Locals { get; }

        public IEnumerable<LocalVariable> Variables => Arguments.Concat(Locals);

        public CallFrame(iObject instance, CallFrame caller = null, params LocalVariable[] arguments)
        {
            Instance = instance;
            Caller = caller;
            Arguments = new List<LocalVariable>(arguments);
            Locals = new List<LocalVariable>();
        }

        public static CallFrame Push(iObject instance, params LocalVariable[] arguments) =>
            Current = new CallFrame(instance, Current, arguments);

        public static void Pop() => Current = Current?.Caller;

        public static class Reflection
        {
            public static readonly FieldInfo Current = Reflector.Field(() => CallFrame.Current);

            public static readonly PropertyInfo Instance = Reflector<CallFrame>.Property(_ => _.Instance);

            public static readonly PropertyInfo Locals = Reflector<CallFrame>.Property(_ => _.Locals);

            public static readonly MethodInfo Locals_Add =
                Reflector<IList<LocalVariable>>.Method(_ => _.Add(default(LocalVariable)));

            public static readonly PropertyInfo Locals_Indexer =
                Reflector<IList<LocalVariable>>.Property(_ => _[default(int)]);
        }

        public static class Expressions
        {
            public static MemberExpression Current() =>
                Field(null, Reflection.Current);

            public static MemberExpression Instance(Expression callFrame) =>
                Property(callFrame, Reflection.Instance);

            public static MemberExpression Locals(Expression callFrame) =>
                Property(callFrame, Reflection.Locals);

            public static MethodCallExpression Locals_Add(Expression locals, Expression localVariable) =>
                Call(locals, Reflection.Locals_Add, localVariable);

            public static IndexExpression Locals_Indexer(Expression locals, Expression index) =>
                Property(locals, Reflection.Locals_Indexer, index);
        }
    }
}
