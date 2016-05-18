using Mint.MethodBinding.Binders;

namespace Mint.MethodBinding.CallCompilation
{
    public abstract class BaseCallCompiler : CallCompiler
    {
        public CallSite CallSite { get; }

        protected BaseCallCompiler(CallSite callSite)
        {
            CallSite = callSite;
        }

        public abstract Function Compile();

        protected MethodBinder TryFindMethodBinder(iObject instance)
        {
            var binder = instance.EffectiveClass.FindMethod(CallSite.CallInfo.MethodName);
            if(binder == null)
            {
                var methodName = CallSite.CallInfo.MethodName.ToString();
                var instanceInspect = instance.Inspect();
                var className = instance.EffectiveClass.FullName;

                throw new NoMethodError($"undefined method `{methodName}' for {instanceInspect}:{className}");
            }
            return binder;
        }
    }
}