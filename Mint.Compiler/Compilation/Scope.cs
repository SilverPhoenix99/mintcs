using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Mint.Compilation
{
    public enum ScopeType
    {
        Method = 1,
        While,
        For,
        Block
    }

    public class Scope
    {
        public Scope(ScopeType type, Closure closure)
        {
            Type = type;
            Closure = closure;
        }

        public ScopeType Type { get; }

        public Closure Closure { get; }

        public Dictionary<string, LabelTarget> Labels { get; } = new Dictionary<string, LabelTarget>();

        public Scope Previous { get; private set; }

        public LabelTarget Label(string label, Type type = null)
        {
            LabelTarget target;
            if(!Labels.TryGetValue(label, out target))
            {
                Labels[label] = target = type == null
                    ? Expression.Label(label)
                    : Expression.Label(typeof(iObject), label);
            }

            return target;
        }

        public Scope Enter(ScopeType type, Closure closure = null) => new Scope(type, closure ?? new Closure(Closure.Self)) { Previous = this };
    }
}
