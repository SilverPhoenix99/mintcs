using System.Linq.Expressions;

namespace Mint.MethodBinding.Binders
{
    public interface MethodBinder
    {
        Symbol     Name       { get; }
        Module     Owner      { get; }
        Condition  Condition  { get; }
        Range      Arity      { get; }
        Visibility Visibility { get; }

        Expression Bind(CallSite site, Expression instance, Expression arguments);

        MethodBinder Duplicate(bool copyValidation);
    }
}