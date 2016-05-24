using System.Linq.Expressions;

namespace Mint.Binding
{
    public class InvocationInfo
    {
        public CallInfo CallInfo { get; }
        public Expression Instance { get; }
        public Expression Arguments { get; }

        public InvocationInfo(CallInfo callInfo, Expression instance, Expression arguments)
        {
            CallInfo = callInfo;
            Instance = instance;
            Arguments = arguments;
        }
    }
}
