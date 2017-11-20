using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Mint.MethodBinding.Arguments;

namespace Mint.MethodBinding.Methods
{
    public class CallFrameBinder : IEnumerable<ParameterExpression>
    {
        public ParameterExpression CallFrame { get; } = Expression.Variable(typeof(CallFrame), "frame");

        public ParameterExpression Instance { get; } = Expression.Variable(typeof(iObject), "instance");

        public ParameterExpression Bundle { get; } = Expression.Variable(typeof(ArgumentBundle), "bundle");

        public ParameterExpression Arguments { get; } = Expression.Variable(typeof(iObject[]), "arguments");

        public IEnumerator<ParameterExpression> GetEnumerator()
        {
            yield return CallFrame;
            yield return Instance;
            yield return Bundle;
            yield return Arguments;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
