using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding
{
    internal static class BindingUtils
    {
        public static readonly Expression NIL = Constant(new NilClass(), typeof(iObject));

        public static readonly MethodInfo OBJECT_BOX = Reflector.Method(
            () => Object.Box(default(object))
        );
    }
}
