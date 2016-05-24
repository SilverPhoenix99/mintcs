using System.Linq.Expressions;

namespace Mint.Binding
{
    public class Invocation
    {
        public CallInfo CallInfo { get; }
        public Expression Instance { get; }
        public Expression Arguments { get; }

        public Invocation(CallInfo callInfo, Expression instance, Expression arguments)
        {
            CallInfo = callInfo;
            Instance = instance;
            Arguments = arguments;
        }
    }
}
