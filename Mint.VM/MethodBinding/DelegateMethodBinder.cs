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

        public override Expression Bind(CallSite site, Expression instance, params Expression[] args)
        {
            // TODO parameter check

            Expression expression = Invoke(Constant(function), new[] { instance }.Concat(args));

            if(!typeof(iObject).IsAssignableFrom(function.Method.ReturnType))
            {
                expression = Call(
                    ClrMethodBinder.OBJECT_BOX_METHOD,
                    Convert(expression, typeof(object))
                );
            }

            return expression;
        }

        public override MethodBinder Duplicate(bool copyValidation) => new DelegateMethodBinder(this, copyValidation);
    }
}