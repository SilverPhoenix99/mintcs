using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.Binders
{
    public sealed class DelegateMethodBinder : BaseMethodBinder
    {
        private readonly Delegate function;
        private readonly IReadOnlyList<IReadOnlyList<Attribute>> parameterAttributes; // TODO

        public DelegateMethodBinder(Symbol name, Module owner, Delegate function/*, IReadOnlyList<IReadOnlyList<Attribute>> parameterAttributes*/)
            : base(name, owner)
        {
            this.function = function;
            //this.parameterAttributes = parameterAttributes;
            var numParameters = function.Method.GetParameters().Length;
            Arity = new Range(numParameters, numParameters); // TODO - 1, due to instance?
        }

        private DelegateMethodBinder(DelegateMethodBinder other, bool copyValidation)
            : base(other, copyValidation)
        {
            function = other.function;
            parameterAttributes = other.parameterAttributes;
        }

        public override Expression Bind(CallSite site, Expression instance, Expression arguments)
        {
            // TODO parameter check

            var length = site.CallInfo.Parameters.Length;
            var unsplatArgs = new[] { instance }.Concat(
                Enumerable.Range(0, length).Select(i => (Expression) ArrayIndex(arguments, Constant(i)))
            ).Zip(function.Method.GetParameters(), ConvertArgument);

            return Box(Invoke(Constant(function), unsplatArgs));
        }

        public override MethodBinder Duplicate(bool copyValidation) => new DelegateMethodBinder(this, copyValidation);
    }
}