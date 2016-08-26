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

        public abstract Function Compile();

        protected MethodBinder TryFindMethodBinder(iObject instance)
        {
            var binder = instance.EffectiveClass.FindMethod(CallSite.MethodName);
            if(binder == null)
            {
                var methodName = CallSite.MethodName.ToString();
                var instanceInspect = instance.Inspect();
                var className = instance.EffectiveClass.FullName;

                throw new NoMethodError($"undefined method `{methodName}' for {instanceInspect}:{className}");
            }
            return binder;
        }
    }
}