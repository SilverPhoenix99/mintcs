using Mint.Binding;
using Mint.Binding.Arguments;
using System.Linq.Expressions;
using static Mint.Parse.TokenType;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal abstract class BinaryOperatorCompiler : CompilerComponentBase
    {
        protected abstract Symbol Operator { get; }

        protected BinaryOperatorCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var left = Pop();
            var right = Pop();

            // TODO if protected in instance_eval, and lhs != self but same class => public

            var visibility = Node[0].Value?.Type == kSELF ? Visibility.Protected : Visibility.Public;
            var argument = new InvocationArgument(ArgumentKind.Simple, right);
            return CompilerUtils.Invoke(visibility, left, Operator, argument);
        }
    }
}