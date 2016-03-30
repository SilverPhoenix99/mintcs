using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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
        public ScopeType Type { get; }

        public Dictionary<Symbol, ParameterExpression> Variables { get; } = new Dictionary<Symbol, ParameterExpression>();

        public Dictionary<string, LabelTarget> Labels { get; } = new Dictionary<string, LabelTarget>();

        public Scope(ScopeType type)
        {
            Type = type;
        }

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
    }
}
