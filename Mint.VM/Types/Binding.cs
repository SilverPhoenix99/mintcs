using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.MethodBinding.Methods;
using Mint.Reflection;

namespace Mint
{
    public class Binding : BaseObject
    {
        private readonly CallFrame frame;
        private readonly IList<LocalVariable> dynamicLocals;

        public iObject Receiver => frame.Instance;

        private IEnumerable<LocalVariable> Locals => frame.Arguments.Concat(frame.Locals).Concat(dynamicLocals);

        public Array LocalVariables => new Array(Locals.Select(_ => _.Name).Cast<iObject>());

        private Binding(CallFrame frame, IList<LocalVariable> dynamicLocals)
            : base(Class.BINDING)
        {
            this.frame = frame ?? throw new ArgumentNullException(nameof(frame));
            this.dynamicLocals = dynamicLocals;
        }

        private Binding(Binding other) : this(other.frame, new List<LocalVariable>(other.dynamicLocals))
        { }

        internal Binding(CallFrame frame) : this(frame, new List<LocalVariable>())
        { }

        public Binding() : this(CallFrame.Current)
        { }

        public bool IsLocalDefined(Symbol local) => Locals.Any(_ => _.Name == local);

        internal LocalVariable GetLocal(Symbol local) => Locals.FirstOrDefault(_ => _.Name == local);

        public iObject GetLocalValue(Symbol local) => GetLocal(local)?.Value;

        public iObject SetLocalValue(Symbol local, iObject value)
        {
            var variable = Locals.FirstOrDefault(_ => _.Name == local);

            if(variable != null)
            {
                variable.Value = value;
            }
            else
            {
                dynamicLocals.Add(new LocalVariable(local, value));
            }

            return value;
        }

        public Binding Duplicate() => new Binding(this);

        public static class Reflection
        {
            public static readonly ConstructorInfo Ctor = Reflector.Ctor<Binding>(typeof(iObject));

            public static readonly MethodInfo SetLocalValue =
                Reflector<Binding>.Method(_ => _.SetLocalValue(default(Symbol), default(iObject)));
        }

        public static class Expressions
        {
            public static NewExpression New(Expression receiver) => Expression.New(Reflection.Ctor, receiver);
        }
    }
}