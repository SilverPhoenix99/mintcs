using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding
{
    public sealed partial class PolymorphicSiteBinder
    {
        internal class CachedMethod
        {
            public CachedMethod(MethodBinder binder, CallSite site, Expression instance, Expression args)
            {
                Binder = binder;
                Expression = CompileExpression(site, instance, args);
            }

            public MethodBinder Binder { get; }
            public Expression Expression { get; }

            private Expression CompileExpression(CallSite site, Expression instance, Expression args)
            {
                // TODO assuming always ParameterKind.Req. change to accept Block, Rest, KeyRequired, KeyRest
                var unsplatArgs = Enumerable.Range(0, site.Parameters.Length).Select(i => ArrayIndex(args, Constant(i)));

                return Binder.Bind(site, instance, unsplatArgs);
            }
        }

    }
}
