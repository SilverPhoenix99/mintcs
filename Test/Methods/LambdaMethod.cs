using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint
{
    public class LambdaMethod : Method
    {
        public LambdaMethod(Symbol name, Module owner, Delegate lambda) : base(name, owner)
        {
            Lambda = lambda;
        }

        public Delegate Lambda { get; }

        public override Expression Bind(Expression target, IEnumerable<Expression> args)
        {
            return Call(Constant(Lambda.Target), Lambda.Method, new[] { target }.Concat(args));
        }
    }
}
