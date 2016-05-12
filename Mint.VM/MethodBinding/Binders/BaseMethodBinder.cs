using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace Mint.MethodBinding
{
    public abstract class BaseMethodBinder : MethodBinder
    {
        public BaseMethodBinder(Symbol name, Module owner, Visibility visibility = Visibility.Public)
        {
            Contract.Assert(name != null);
            Contract.Assert(owner != null);
            Name       = name;
            Owner      = owner;
            Condition  = new Condition();
            Visibility = visibility;
        }

        protected BaseMethodBinder(MethodBinder other, bool copyValidation = false)
            : this(other.Name, other.Owner, other.Visibility)
        {
            Arity = other.Arity;
            if(copyValidation && !other.Condition.Valid)
            {
                Condition.Invalidate();
            }
        }

        public Symbol    Name        { get; }
        public Module    Owner       { get; }
        public Condition Condition   { get; }
        public Range     Arity       { get; protected set; }
        public Visibility Visibility { get; }

        public abstract Expression Bind(CallSite site, Expression instance, Expression args);

        public abstract MethodBinder Duplicate(bool copyValidation);
    }
}