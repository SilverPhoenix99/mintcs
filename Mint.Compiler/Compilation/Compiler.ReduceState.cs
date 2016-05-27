using System.Collections.Generic;
using System.Linq.Expressions;
using Mint.Compilation.Components;
using Mint.Parse;

namespace Mint.Compilation
{
    public partial class Compiler
    {
        private class ReduceState
        {
            public readonly Ast<Token> Node;
            public readonly CompilerComponent Component;
            private readonly int childCount;
            public readonly Queue<Expression> Children = new Queue<Expression>();

            public ReduceState(Ast<Token> node, CompilerComponent component, int childCount)
            {
                Node = node;
                Component = component;
                this.childCount = childCount;
            }

            public bool CanReduce => childCount == Children.Count;
        }
    }
}
