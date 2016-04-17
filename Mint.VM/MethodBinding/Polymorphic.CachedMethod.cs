using System.Linq.Expressions;

namespace Mint.MethodBinding
{
    public sealed partial class PolymorphicSiteBinder
    {
        internal class CachedMethod
        {
            public CachedMethod(MethodBinder binder, CallSite site, Expression instance, Expression args)
            {
                Binder = binder;
                Expression = Binder.Bind(site, instance, args);
            }

            public MethodBinder Binder { get; }
            public Expression Expression { get; }
        }

    }
}
