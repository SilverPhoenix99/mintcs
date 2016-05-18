using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mint.Reflection;
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
            Arity = new Arity(numParameters, numParameters); // TODO - 1, due to instance?
        }

        private DelegateMethodBinder(Symbol newName, DelegateMethodBinder other)
            : base(newName, other)
        {
            function = other.function;
            parameterAttributes = other.parameterAttributes;
        }

        public override MethodBinder Alias(Symbol newName) => new DelegateMethodBinder(newName, this);

        public override Expression Bind(CallInfo callInfo, Expression instance, Expression arguments)
        {
            // TODO parameter check

            var length = callInfo.Parameters.Length;
            var unsplatArgs = new[] { instance }.Concat(
                Enumerable.Range(0, length).Select(i => (Expression) ArrayIndex(arguments, Constant(i)))
            ).Zip(function.Method.GetParameters(), ConvertArgument);

            return Box(Invoke(Constant(function), unsplatArgs));
        }
    }
}