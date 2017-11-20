using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.Methods
{
    public abstract class BaseMethodBinder : MethodBinder
    {
        private static readonly Dictionary<Type, Type> TYPES = new Dictionary<Type, Type>
        {
            { typeof(string),        typeof(String) },
            { typeof(StringBuilder), typeof(String) },
            { typeof(sbyte),         typeof(Fixnum) },
            { typeof(byte),          typeof(Fixnum) },
            { typeof(short),         typeof(Fixnum) },
            { typeof(ushort),        typeof(Fixnum) },
            { typeof(int),           typeof(Fixnum) },
            { typeof(uint),          typeof(Fixnum) },
            { typeof(long),          typeof(Fixnum) },
            { typeof(float),         typeof(Float)  },
            { typeof(double),        typeof(Float)  }
        };

        public Symbol Name { get; }

        public Module Owner { get; }

        public Module Caller { get; }

        public Condition Condition { get; }

        public Visibility Visibility { get; }

        public Func<iObject> Call { get; protected set; }

        protected BaseMethodBinder(Symbol name,
                                   Module owner,
                                   Module caller = null,
                                   Visibility visibility = Visibility.Public)
        {
            Name = name;
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Caller = caller ?? owner;
            Condition = new Condition();
            Visibility = visibility;
            Call = DefaultCall;
        }

        protected BaseMethodBinder(Symbol newName, MethodBinder other)
            : this(newName, other.Owner, other.Caller, other.Visibility)
        { }

        public abstract MethodBinder Duplicate(Symbol newName);
        
        public MethodBinder Duplicate() => Duplicate(Name);

        private iObject DefaultCall() => (Call = Compile())();

        private Func<iObject> Compile()
        {
            var body = Bind();
            var lambda = Lambda<Func<iObject>>(body);
            return lambda.Compile();
        }

        protected abstract Expression Bind();

        protected static Expression Box(Expression expression)
        {
            if(expression.Type == typeof(void))
            {
                return Block(expression, NilClass.Expressions.Instance);
            }

            return Object.Expressions.Box(expression);
        }
        
        protected static Expression TypeIs(Expression expression, Type type)
        {
            if(TYPES.TryGetValue(type, out var convertedType))
            {
                type = convertedType;
            }

            return Expression.TypeIs(expression, type);
        }
        
        protected static Expression TryConvert(Expression expression, Type type)
        {
            if(TYPES.TryGetValue(type, out var convertedType))
            {
                expression = expression.Cast(convertedType);
            }

            return expression.Cast(type);
        }
    }
}