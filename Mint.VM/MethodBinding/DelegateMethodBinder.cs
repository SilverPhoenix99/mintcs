using System;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding
{
    public sealed class DelegateMethodBinder : BaseMethodBinder
    {
        private readonly Delegate function;

        public DelegateMethodBinder(Symbol name, Module owner, Delegate function)
            : base(name, owner)
        {
            this.function = function;
            var numParameters = function.Method.GetParameters().Length;
            Arity = new Range(numParameters, numParameters); // TODO - 1, due to instance?
        }

        private DelegateMethodBinder(DelegateMethodBinder other, bool copyValidation)
            : base(other, copyValidation)
        {
            function = other.function;
        }

        public override Expression Bind(CallSite site, Expression instance, Expression args)
        {
            // TODO parameter check

            var length = site.Parameters.Length;
            var unsplatArgs = Enumerable.Range(0, length).Select(i => (Expression) ArrayIndex(args, Constant(i)));

            return Invoke(Constant(function), new[] { instance }.Concat(unsplatArgs));
        }

        public override MethodBinder Duplicate(bool copyValidation) => new DelegateMethodBinder(this, copyValidation);
    }
}