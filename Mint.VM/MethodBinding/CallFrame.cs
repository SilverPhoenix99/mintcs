using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Mint.MethodBinding.Arguments;
using Mint.Reflection;

namespace Mint.MethodBinding
{
    public class CallFrame
    {
        [ThreadStatic]
        public static CallFrame Current;

        public CallFrame Caller { get; private set; }

        public CallSite CallSite { get; }

        public iObject Instance { get; }

        public ArgumentBundle Arguments { get; }

        public IDictionary<Symbol, LocalVariable> Locals { get; }

        public CallFrame(CallSite callSite,
                         iObject instance,
                         ArgumentBundle arguments = null,
                         CallFrame caller = null)
        {
            CallSite = callSite;
            Instance = instance;
            Caller = caller ?? Current;
            Arguments = arguments ?? new ArgumentBundle();
            Locals = new LinkedDictionary<Symbol, LocalVariable>();
        }
        
        public Module Module => Instance as Module ?? Instance.EffectiveClass;
        
        public LocalVariable AddLocal(LocalVariable local)
        {
            Locals.Add(local.Name, local);
            return local;
        }

        public static CallFrame Push(CallSite callSite,
                                     iObject instance,
                                     ArgumentBundle arguments = null)
            => Current = new CallFrame(callSite, instance, arguments);

        public static void Push(CallFrame other)
        {
            other.Caller = Current;
            Current = other;
        }

        public static void Pop() => Current = Current?.Caller;
        
        public static class Reflection
        {
            public static readonly FieldInfo Current = Reflector.Field(() => CallFrame.Current);

            public static readonly PropertyInfo Instance = Reflector<CallFrame>.Property(_ => _.Instance);

            public static readonly PropertyInfo Arguments = Reflector<CallFrame>.Property(_ => _.Arguments);

            public static readonly PropertyInfo Locals = Reflector<CallFrame>.Property(_ => _.Locals);

            public static readonly PropertyInfo Module = Reflector<CallFrame>.Property(_ => _.Module);

            public static readonly PropertyInfo IDictionary_Indexer =
                Reflector<IDictionary<Symbol, LocalVariable>>.Property(_ => _[default]);

            public static readonly MethodInfo AddLocal =
                Reflector<CallFrame>.Method(_ => _.AddLocal(default));

            public static readonly MethodInfo Push = Reflector.Method(
                () => Push(default, default, default)
            );

            public static readonly MethodInfo PushFrame = Reflector.Method(
                () => Push(default)
            );

            public static readonly MethodInfo Pop = Reflector.Method(() => Pop());
        }

        public static class Expressions
        {
            public static MemberExpression Current() => Expression.Field(null, Reflection.Current);
            
            public static MemberExpression Instance(Expression callFrame) => Expression.Property(callFrame, Reflection.Instance);

            public static MemberExpression Arguments(Expression callFrame)
                => Expression.Property(callFrame, Reflection.Arguments);
            
            public static MemberExpression Locals(Expression callFrame) => Expression.Property(callFrame, Reflection.Locals);
            
            public static MemberExpression Module(Expression callFrame) => Expression.Property(callFrame, Reflection.Module);

            public static MethodCallExpression AddLocal(Expression callFrame, Expression localVariable)
                => Expression.Call(callFrame, Reflection.AddLocal, localVariable);
            
            public static IndexExpression LocalsIndexer(Expression callFrame, Expression name)
                => Expression.Property(Locals(callFrame), Reflection.IDictionary_Indexer, name);

            public static MethodCallExpression Push(Expression callSite,
                                                    Expression instance,
                                                    Expression arguments = null)
                => Expression.Call(
                    Reflection.Push,
                    callSite,
                    instance,
                    arguments ?? Expression.Constant(null, typeof(ArgumentBundle))
                );

            public static MethodCallExpression Push(Expression otherFrame) => Expression.Call(Reflection.PushFrame, otherFrame);

            public static MethodCallExpression Pop() => Expression.Call(Reflection.Pop);
        }
    }
}
