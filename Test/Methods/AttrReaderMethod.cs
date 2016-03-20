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

        public override Expression Bind(Expression target, IEnumerable<Expression> args)
        {


            throw new NotImplementedException();
        }
    }
}
