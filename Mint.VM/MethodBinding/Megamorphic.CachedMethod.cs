using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding
{
    public sealed partial class MegamorphicCallCompiler
    {
        private class CachedMethod
        {
            public CachedMethod(MethodBinder binder, CallSite site)
            {
                Binder = binder;
                Call = CompileMethod(site);
            }

            public MethodBinder Binder { get; }
            public Function Call { get; }

            private Function CompileMethod(CallSite site)
            {
                var instance = Parameter(typeof(iObject), "instance");
                var args     = Parameter(typeof(iObject[]), "args");
                var body     = Binder.Bind(site, instance, args);
                var lambda   = Lambda<Function>(body, instance, args);
                return lambda.Compile();
            }
        }
    }
}
