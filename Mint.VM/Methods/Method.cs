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
        public delegate iObject Delegate(iObject instance, iObject[] args);

        public Method(Symbol name, Module owner)
        {
            Name = name;
            Owner = owner;
            Condition = new Condition();
        }

        public Symbol    Name      { get; }
        public Module    Owner     { get; }
        public Condition Condition { get; }

        public abstract Expression Bind(Expression instance, IEnumerable<Expression> args);

        public Expression Bind(Expression instance, params Expression[] args) =>
            Bind(instance, (IEnumerable<Expression>) args);

        public Func<iObject> Compile(iObject instance, IEnumerable<iObject> args)
        {
            var body = Bind(Constant(instance), args.Select(_ => (Expression) Constant(_)));
            return Lambda<Func<iObject>>(body).Compile();
        }

        public Func<iObject> Compile(iObject instance, params iObject[] args) =>
            Compile(instance, (IEnumerable<iObject>) args);

        public Delegate Compile()
        {
            var instance = Parameter(typeof(iObject), "instance");
            var args = Parameter(typeof(iObject[]), "args");
            var body = Bind(instance, args);
            var lambda = Lambda<Delegate>(body, instance, args);
            return lambda.Compile();
        }

        public iObject Invoke(iObject instance, IEnumerable<iObject> args) => Compile(instance, args)();
        public iObject Invoke(iObject instance, params iObject[] args) => Compile(instance, args)();

        #region Static

        public static Method Create(Symbol name, Module owner, MethodInfo info) => new CompiledMethod(name, owner, info);

        public static Method Create(Symbol name, Module owner, Delegate lambda) => new LambdaMethod(name, owner, lambda);

        #endregion
    }
}
