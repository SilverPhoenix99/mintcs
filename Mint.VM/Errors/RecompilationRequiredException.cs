using System.Linq.Expressions;

namespace Mint
{
    public class RecompilationRequiredException : Exception
    {
        public static class Expressions
        {
            public static NewExpression New(Expression message)
                => Expression.New(typeof(RecompilationRequiredException));
        }
    }
}