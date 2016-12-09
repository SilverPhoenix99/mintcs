using System.Linq.Expressions;
using Mint.Parse;

namespace Mint.Compilation.Components
{
    public abstract class CompilerComponentBase : CompilerComponent
    {
        public Compiler Compiler { get; }

        protected Ast<Token> Node => Compiler.CurrentNode;
        
        protected CompilerComponentBase(Compiler compiler)
        {
            Compiler = compiler;
        }

        public virtual void Shift()
        {
            foreach(var child in Node)
            {
                Push(child);
            }
        }

        public abstract Expression Reduce();

        protected void Push(Ast<Token> node) => Compiler.Push(node);

        protected Expression Pop() => Compiler.Pop();

        public virtual Expression Compile()
        {
            throw new System.MethodAccessException("Compile() must be overriden.");
        }
    }
}