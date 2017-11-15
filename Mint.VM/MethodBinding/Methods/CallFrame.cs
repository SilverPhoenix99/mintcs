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
        public CallFrame(iObject instance, CallFrame caller = null, params LocalVariable[] arguments)
        {
            Instance = instance;
            Caller = caller;
            Arguments = arguments;
            Locals = new LinkedDictionary<Symbol, LocalVariable>();
            Visibility = Visibility.Public;
        }


        [ThreadStatic]
        public static CallFrame Current;

        public CallFrame Caller { get; }
        public iObject Instance { get; }
        public IList<LocalVariable> Arguments { get; }
        public IDictionary<Symbol, LocalVariable> Locals { get; }
        public IEnumerable<LocalVariable> Variables => Arguments.Concat(Locals.Values);
        public Module Module => Instance as Module ?? Instance.EffectiveClass;
        public Visibility Visibility { get; set; }


        public LocalVariable AddLocal(LocalVariable local)
        {
            Locals.Add(local.Name, local);
            return local;
        }


        public static CallFrame Push(iObject instance, params LocalVariable[] arguments)
            => Current = new CallFrame(instance, Current, arguments);


        public static void Pop()
            => Current = Current?.Caller;


        public static class Reflection
        {
            public static readonly FieldInfo Current = Reflector.Field(() => CallFrame.Current);

            public static readonly PropertyInfo Instance = Reflector<CallFrame>.Property(_ => _.Instance);

            public static readonly PropertyInfo Locals = Reflector<CallFrame>.Property(_ => _.Locals);

            public static readonly PropertyInfo Module = Reflector<CallFrame>.Property(_ => _.Module);

            public static readonly PropertyInfo Visibility = Reflector<CallFrame>.Property(_ => _.Visibility);

            public static readonly PropertyInfo IDictionary_Indexer =
                Reflector<IDictionary<Symbol, LocalVariable>>.Property(_ => _[default(Symbol)]);

            public static readonly MethodInfo AddLocal =
                Reflector<CallFrame>.Method(_ => _.AddLocal(default(LocalVariable)));

            public static readonly MethodInfo Push = Reflector.Method(
                () => Push(default(iObject), default(LocalVariable[]))
            );

            public static readonly MethodInfo Pop = Reflector.Method(() => Pop());
        }

        public static class Expressions
        {
            public static MemberExpression Current()
                => Field(null, Reflection.Current);


            public static MemberExpression Instance(Expression callFrame)
                => Property(callFrame, Reflection.Instance);


            public static MemberExpression Locals(Expression callFrame)
                => Property(callFrame, Reflection.Locals);


            public static MemberExpression Module(Expression callFrame)
                => Property(callFrame, Reflection.Module);


            public static MemberExpression Visibility(Expression callFrame)
                => Property(callFrame, Reflection.Visibility);


            public static MethodCallExpression AddLocal(Expression callFrame, Expression localVariable)
                => Call(callFrame, Reflection.AddLocal, localVariable);


            public static IndexExpression LocalsIndexer(Expression callFrame, Expression name)
                => Property(Locals(callFrame), Reflection.IDictionary_Indexer, name);


            public static MethodCallExpression Push(Expression instance, Expression arguments = null)
                => Call(Reflection.Push, instance, arguments ?? Constant(System.Array.Empty<LocalVariable>()));


            public static MethodCallExpression Pop()
                => Call(Reflection.Pop);
        }
    }
}
