using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint
{
    public class LambdaMethod : Method
    {
        public LambdaMethod(Symbol name, Module owner, Delegate lambda) : base(name, owner)
        {
            Contract.Assert(lambda != null);
            Lambda = lambda;
        }

        public Delegate Lambda { get; }

        public override Expression Bind(Expression instance, IEnumerable<Expression> args)
        {
            return Call(Constant(Lambda.Target), Lambda.Method, new[] { instance }.Concat(args));
        }

        public override Method Duplicate()
        {
            var method = new LambdaMethod(Name, Owner, Lambda);
            if(!Condition.Valid)
            {
                method.Condition.Invalidate();
            }
            return method;
        }
    }
}
