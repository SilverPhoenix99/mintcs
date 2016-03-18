using Mint.Compilation;
using System;
using System.Linq.Expressions;
using System.Reflection;

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

        public abstract Expression Bind(Expression target, params Expression[] args);

        public static Method Create(Symbol name, Module owner, MethodInfo info) => new CompiledMethod(name, owner, info);

        public static Method Create(Symbol name, Module owner, Delegate lambda) => new LambdaMethod(name, owner, lambda);
    }
}
