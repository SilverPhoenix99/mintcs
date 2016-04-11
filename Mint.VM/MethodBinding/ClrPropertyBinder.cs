using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace Mint.MethodBinding
{
    public sealed partial class ClrPropertyBinder : BaseMethodBinder
    {
        private Info[] infos;

        public ClrPropertyBinder(Symbol name, Module owner, PropertyInfo property)
            : base(name, owner)
        {
            infos = GetOverloads(property);
            Arity = CalculateArity();
        }

        private ClrPropertyBinder(ClrPropertyBinder other, bool copyValidation)
            : base(other, copyValidation)
        {
            infos = (Info[]) other.infos.Clone();
        }

        public override Expression Bind(Expression instance, IEnumerable<Expression> args)
        {
            throw new NotImplementedException();
        }

        public override MethodBinder Duplicate(bool copyValidation) => new ClrPropertyBinder(this, copyValidation);

        private Info[] GetOverloads(PropertyInfo method)
        {
            throw new NotImplementedException();
        }

        private Range CalculateArity()
        {
            throw new NotImplementedException();
        }
    }
}