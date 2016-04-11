using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace Mint.MethodBinding
{
    public abstract class BaseMethodBinder : MethodBinder
    {
        public BaseMethodBinder(Symbol name, Module owner)
        {
            Contract.Assert(name != null);
            Contract.Assert(owner != null);
            Name      = name;
            Owner     = owner;
            Condition = new Condition();
        }

        protected BaseMethodBinder(MethodBinder other, bool copyValidation = false)
            : this(other.Name, other.Owner)
        {
            Arity = other.Arity;
            if(copyValidation && !other.Condition.Valid)
            {
                Condition.Invalidate();
            }
        }

        public Symbol    Name      { get; }
        public Module    Owner     { get; }
        public Condition Condition { get; }
        public Range     Arity     { get; protected set; }

        public abstract Expression Bind(Expression instance, IEnumerable<Expression> args);

        public Expression Bind(Expression instance, params Expression[] args) =>
            Bind(instance, (IEnumerable<Expression>) args);

        public abstract MethodBinder Duplicate(bool copyValidation);
    }
}