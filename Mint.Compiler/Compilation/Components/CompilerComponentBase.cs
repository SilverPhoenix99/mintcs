using System.Linq.Expressions;
using Mint.Binding;
using Mint.Parse;
using static Mint.Binding.Visibility;
using static Mint.Parse.TokenType;

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

        protected static Visibility GetVisibility(Ast<Token> left)
        {
            // TODO if protected in instance_eval, and lhs != self but same class => public

            return left.Value?.Type == kSELF ? Protected : Public;
        }
    }
}