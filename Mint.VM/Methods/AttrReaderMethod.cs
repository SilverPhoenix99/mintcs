using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mint
{
    public class AttrReaderMethod : Method
    {
        public AttrReaderMethod(Symbol name, Module owner) : base(name, owner)
        {
        }

        public override Expression Bind(Expression instance, IEnumerable<Expression> args)
        {
            throw new NotImplementedException();
        }

        public override Method Duplicate()
        {
            var method = new AttrReaderMethod(Name, Owner);
            if(!Condition.Valid)
            {
                method.Condition.Invalidate();
            }
            return method;
        }
    }
}
