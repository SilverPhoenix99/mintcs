using Mint.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding.Methods
{
    public sealed class DelegateMethodBinder : BaseMethodBinder
    {
        private Delegate Lambda { get; }

        private readonly IReadOnlyList<Attribute> parameterAttributes; // TODO

        public DelegateMethodBinder(Symbol name, Module owner, Delegate lambda/*, IReadOnlyList<Attribute> parameterAttributes*/)
            : base(name, owner)
        {
            Lambda = lambda;

            //this.parameterAttributes = parameterAttributes;
            Arity = Lambda.Method.GetParameterCounter().Arity;
        }

        private DelegateMethodBinder(Symbol newName, DelegateMethodBinder other)
            : base(newName, other)
        {
            Lambda = other.Lambda;
            parameterAttributes = other.parameterAttributes;
        }

        public override MethodBinder Duplicate(Symbol newName) => new DelegateMethodBinder(newName, this);

        public override Expression Bind(CallFrameBinder frame)
        {
            // TODO parameter check

            var length = frame.CallSite.ArgumentKinds.Count;
            var parameterTypes = Lambda.Method.GetParameters().Select(_ => _.ParameterType);

            var unsplatArgs = new[] { frame.Instance }.Concat(
                Enumerable.Range(0, length).Select(i => (Expression) ArrayIndex(frame.Arguments, Constant(i)))
            ).Zip(parameterTypes, TryConvert);

            return Box(Invoke(Constant(Lambda), unsplatArgs));
        }
    }
}