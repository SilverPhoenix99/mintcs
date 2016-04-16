using System.Linq;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding
{
    public sealed partial class MegamorphicSiteBinder
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
                var args = Parameter(typeof(iObject[]), "args");

                // TODO assuming always ParameterKind.Req. change to accept Block, Rest, KeyReq, KeyRest
                var unsplatArgs = Enumerable.Range(0, site.Parameters.Length).Select(i => ArrayIndex(args, Constant(i)));

                var body = Binder.Bind(site, instance, unsplatArgs);
                var lambda = Lambda<Function>(body, instance, args);
                return lambda.Compile();
            }
        }
    }
}
