using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint
{
    public abstract class Method
    {
        public Method(Symbol name, Module owner)
        {
            Name = name;
            Owner = owner;
            Condition = new Condition();
        }

        public Symbol    Name      { get; }
        public Module    Owner     { get; }
        public Condition Condition { get; }

        // TODO parameter positions (simple, kw, rest, ...)

        public abstract Expression Bind(Expression target, IEnumerable<Expression> args);

        public Expression Bind(Expression target, params Expression[] args) => Bind(target, (IEnumerable<Expression>) args);

        public object Invoke(object target, IEnumerable<object> args)
        {
            var argsExpr = args.Select(_ => (Expression) Constant(_));
            var expr = Bind(Constant(target), argsExpr);
            var lambda = Lambda(expr).Compile();
            return lambda.DynamicInvoke();
        }

        public object Invoke(object target, params object[] args) => Invoke(target, (IEnumerable<object>) args);

        public static Method Create(Symbol name, Module owner, MethodInfo info) => new CompiledMethod(name, owner, info);

        public static Method Create(Symbol name, Module owner, Delegate lambda) => new LambdaMethod(name, owner, lambda);
    }
}
