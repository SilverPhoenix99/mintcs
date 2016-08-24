using Mint.MethodBinding.Arguments;
using System.Linq.Expressions;

namespace Mint.Compilation
{
    public class InvocationArgument
    {
        public ArgumentKind Kind { get; }
        public Expression Expression { get; }

        public InvocationArgument(ArgumentKind kind, Expression expression)
        {
            Kind = kind;
            Expression = expression;
        }
    }
}