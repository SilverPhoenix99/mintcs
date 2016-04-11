using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mint.MethodBinding
{
    public sealed class DelegateMethodBinder : BaseMethodBinder
    {
        private readonly Function function;

        public DelegateMethodBinder(Symbol name, Module owner, Function function, Range arity)
            : base(name, owner)
        {
            this.function = function;
            Arity = arity;
        }

        private DelegateMethodBinder(DelegateMethodBinder other, bool copyValidation)
            : base(other, copyValidation)
        {
            this.function = other.function;
        }

        public override Expression Bind(Expression instance, IEnumerable<Expression> args)
        {
            throw new NotImplementedException();
        }

        public override MethodBinder Duplicate(bool copyValidation) => new DelegateMethodBinder(this, copyValidation);
    }
}