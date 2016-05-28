using Mint.Binding;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class NotOperatorCompiler : CompilerComponentBase
    {
        public NotOperatorCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            // TODO if protected in instance_eval, and lhs != self but same class => public

            var instance = Pop();
            var visibility = Node[0].Value?.Type == kSELF ? Visibility.Protected : Visibility.Public;
            return CompilerUtils.Invoke(visibility, instance, Symbol.NOT_OP);
        }
    }
}