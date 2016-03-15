using Mint.Compiler;
using System;

namespace Mint
{
    public class Method
    {
        public Method(Symbol name, Module owner, Delegate function)
        {
            Name = name;
            Owner = owner;
            Function = function;
            Condition = new Condition();
        }

        public Symbol    Name      { get; }
        public Module    Owner     { get; }
        public Delegate  Function  { get; }
        public Condition Condition { get; }

        public Method Duplicate()
        {
            return new Method(Name, Owner, Function);
        }
    }
}
