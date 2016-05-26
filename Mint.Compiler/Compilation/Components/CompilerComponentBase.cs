using System.Linq.Expressions;
using Mint.Parse;

namespace Mint.Compilation.Components
{
    internal abstract class CompilerComponentBase : CompilerComponent
    {
        protected Compiler Compiler { get; }
        protected Ast<Token> Node => Compiler.CurrentNode;

        protected CompilerComponentBase(Compiler compiler)
        {
            Compiler = compiler;
        }

        public virtual void Shift()
        { }

        protected void ShiftChildren()
        {
            foreach(var child in Node)
            {
                Push(child);
            }
        }

        public abstract Expression Reduce();

        protected void Push(Ast<Token> node) => Compiler.Push(node);

        protected Expression Pop() => Compiler.Pop();
    }
}