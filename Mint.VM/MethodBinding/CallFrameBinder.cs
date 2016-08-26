using System.Linq.Expressions;

namespace Mint.MethodBinding
{
    public class CallFrameBinder
    {
        public CallSite CallSite { get; }
        public Expression Instance { get; }
        public Expression Arguments { get; }

        public CallFrameBinder(CallSite callSite, Expression instance, Expression arguments)
        {
            CallSite = callSite;
            Instance = instance;
            Arguments = arguments;
        }
    }
}
