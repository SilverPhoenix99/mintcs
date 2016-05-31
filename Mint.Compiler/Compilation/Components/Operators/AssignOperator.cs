using System.Linq.Expressions;

namespace Mint.Compilation.Components.Operators
{
    internal interface AssignOperator
    {
        Expression Reduce(AssignCompiler component);
    }
}
