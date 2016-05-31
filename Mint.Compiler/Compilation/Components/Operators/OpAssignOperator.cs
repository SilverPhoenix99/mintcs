using System.Linq.Expressions;

namespace Mint.Compilation.Components.Operators
{
    internal interface OpAssignOperator
    {
        Expression Reduce(OpAssignCompiler component);
    }
}
