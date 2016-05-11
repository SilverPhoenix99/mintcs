using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding
{
    public sealed partial class PolymorphicCallCompiler
    {
        internal class CachedMethod
        {
            public CachedMethod(MethodBinder binder, CallSite site, Expression instance, Expression args)
            {
                Binder = binder;
                Expression = Binder.Bind(site, instance, args);

                if(!typeof(iObject).IsAssignableFrom(Expression.Type))
                {
                    Expression = Call(
                        ClrMethodBinder.OBJECT_BOX_METHOD,
                        Convert(Expression, typeof(object))
                    );
                }
                else if(Expression.Type != typeof(iObject))
                {
                    Expression = Convert(Expression, typeof(iObject));
                }
            }

            public MethodBinder Binder { get; }
            public Expression Expression { get; }
        }

    }
}
