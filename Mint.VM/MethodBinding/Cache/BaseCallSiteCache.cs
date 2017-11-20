using Mint.MethodBinding.Methods;

namespace Mint.MethodBinding.Cache
{
    public abstract class BaseCallSiteCache : CallSiteCache
    {
        protected CallSite CallSite { get; }

        protected BaseCallSiteCache(CallSite callSite)
        {
            CallSite = callSite;
        }

        public abstract iObject Call();

        protected static MethodBinder TryFindMethodBinder()
        {
            var frame = CallFrame.Current;
            var instance = frame.Instance;
            var callSite = frame.CallSite;
            var binder = instance.EffectiveClass.FindMethod(callSite.MethodName);
            if(binder != null)
            {
                return binder;
            }

            var methodName = callSite.MethodName.ToString();
            var instanceInspect = instance.Inspect();
            var className = instance.EffectiveClass.Name;

            throw new NoMethodError($"undefined method `{methodName}' for {instanceInspect}:{className}");
        }
    }
}