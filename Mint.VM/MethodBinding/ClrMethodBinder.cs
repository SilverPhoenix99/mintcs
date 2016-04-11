using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace Mint.MethodBinding
{
    public sealed partial class ClrMethodBinder : BaseMethodBinder
    {
        private Info[] infos;

        public ClrMethodBinder(Symbol name, Module owner, MethodInfo method)
            : base(name, owner)
        {
            infos = GetOverloads(method);
            Arity = CalculateArity();
        }

        private ClrMethodBinder(ClrMethodBinder other, bool copyValidation)
            : base(other, copyValidation)
        {
            infos = (Info[]) other.infos.Clone();
        }

        public override Expression Bind(Expression instance, IEnumerable<Expression> args)
        {
            throw new NotImplementedException();
        }

        public override MethodBinder Duplicate(bool copyValidation) => new ClrMethodBinder(this, copyValidation);

        private Info[] GetOverloads(MethodInfo method)
        {
            throw new NotImplementedException();
        }

        private Range CalculateArity()
        {
            throw new NotImplementedException();
        }
    }
}