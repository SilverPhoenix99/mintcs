using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.Methods
{
    public abstract class BaseMethodBinder : MethodBinder
    {
        private static readonly Dictionary<Type, Type> TYPES = new Dictionary<Type, Type>(11)
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

        public Condition Condition { get; }

        public Visibility Visibility { get; }

        public BaseMethodBinder(Symbol name, Module owner, Visibility visibility = Visibility.Public)
        {
            if(owner == null) throw new ArgumentNullException(nameof(owner));

            Name = name;
            Owner = owner;
            Condition = new Condition();
            Visibility = visibility;
        }

        protected BaseMethodBinder(Symbol newName, MethodBinder other)
            : this(newName, other.Owner, other.Visibility)
        { }

        public abstract Expression Bind(CallFrameBinder frame);

        public abstract MethodBinder Duplicate(Symbol newName);

        public MethodBinder Duplicate() => Duplicate(Name);

        protected static Expression Box(Expression expression)
        {
            if(expression.Type == typeof(void))
            {
                return Block(expression, NilClass.Expressions.Instance);
            }

            return Object.Expressions.Box(expression);
        }

        protected internal static Expression TypeIs(Expression expression, Type type)
        {
            if(TYPES.TryGetValue(type, out var convertedType))
            {
                type = convertedType;
            }

            return Expression.TypeIs(expression, type);
        }

        protected internal static Expression TryConvert(Expression expression, Type type)
        {
            if(TYPES.TryGetValue(type, out var convertedType))
            {
                expression = expression.Cast(convertedType);
            }

            return expression.Cast(type);
        }
    }
}