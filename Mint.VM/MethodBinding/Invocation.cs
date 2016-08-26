using System.Linq.Expressions;

namespace Mint.MethodBinding
{
    public class Invocation
    {
        public CallSite CallSite { get; }
        public Expression Instance { get; }
        public Expression Arguments { get; }

        public Invocation(CallSite callSite, Expression instance, Expression arguments)
        {
            CallSite = callSite;
            Instance = instance;
            Arguments = arguments;
        }
    }
}
