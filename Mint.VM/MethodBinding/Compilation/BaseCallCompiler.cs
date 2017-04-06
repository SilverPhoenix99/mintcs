using Mint.MethodBinding.Methods;

namespace Mint.MethodBinding.Compilation
{
    public abstract class BaseCallCompiler : CallCompiler
    {
        public CallSite CallSite { get; }

        protected BaseCallCompiler(CallSite callSite)
        {
            CallSite = callSite;
        }

        public abstract CallSite.Stub Compile();

        protected MethodBinder TryFindMethodBinder(iObject instance)
        {
            var binder = instance.EffectiveClass.FindMethod(CallSite.MethodName);
            if(binder != null)
            {
                return binder;
            }

            var methodName = CallSite.MethodName.ToString();
            var instanceInspect = instance.Inspect();
            var className = instance.EffectiveClass.Name;

            throw new NoMethodError($"undefined method `{methodName}' for {instanceInspect}:{className}");
        }
    }
}