using Mint.Reflection;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.Binders
{
    public abstract class BaseMethodBinder : MethodBinder
    {
        internal static readonly MethodInfo OBJECT_BOX_METHOD = new Func<object, iObject>(Object.Box).Method;

        protected static readonly Dictionary<Type, Type> TYPES = new Dictionary<Type, Type>(11)
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
        public Arity Arity { get; protected set; }
        public Visibility Visibility { get; }

        public BaseMethodBinder(Symbol name, Module owner, Visibility visibility = Visibility.Public)
        {
            if(name == null) throw new ArgumentNullException(nameof(name));
            if(owner == null) throw new ArgumentNullException(nameof(owner));

            Name       = name;
            Owner      = owner;
            Condition  = new Condition();
            Visibility = visibility;
        }

        protected BaseMethodBinder(Symbol newName, MethodBinder other)
            : this(newName, other.Owner, other.Visibility)
        {
            Arity = other.Arity;
        }

        public abstract Expression Bind(CallInfo callInfo, Expression instance, Expression arguments);

        public abstract MethodBinder Alias(Symbol newName);

        public MethodBinder Duplicate() => Alias(Name);

        protected Expression Box(Expression expression)
        {
            if(!typeof(iObject).IsAssignableFrom(expression.Type))
            {
                return Call(OBJECT_BOX_METHOD, Convert(expression, typeof(object)));
            }

            if(expression.Type != typeof(iObject))
            {
                return Convert(expression, typeof(iObject));
            }

            return expression;
        }

        protected static Expression ConvertArgument(Expression arg, ParameterInfo parameter)
        {
            Type type;
            if(TYPES.TryGetValue(parameter.ParameterType, out type))
            {
                arg = Convert(arg, type);
            }

            return Convert(arg, parameter.ParameterType);
        }
    }
}