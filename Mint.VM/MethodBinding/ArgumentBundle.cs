using Mint.MethodBinding.Binders;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Mint.MethodBinding
{
    public class ArgumentBundle
    {
        public IList<iObject> Splat { get; set; }
        public IDictionary<iObject, iObject> Keys { get; set; }
        public iObject Block { get; set; }

        public ArgumentBundle(IList<iObject> splat, IDictionary<iObject, iObject> keys, iObject block)
        {
            Splat = splat;
            Keys = keys;
            Block = block;
        }

        public ArgumentBundle()
            : this(new List<iObject>(), new LinkedDictionary<iObject, iObject>(), null)
        { }

        public iObject[] Unbundle(IEnumerable<ParameterBinder> binders) =>
            binders.Select(binder => binder.Bind(this)).ToArray();

        public static Expression SplatExpression(Expression argumentBundle)
        {
            return Expression.Property(argumentBundle, "Splat");
        }

        public static Expression KeysExpression(Expression argumentBundle)
        {
            return Expression.Property(argumentBundle, "Keys");
        }

        public static Expression BlockExpression(Expression argumentBundle)
        {
            return Expression.Property(argumentBundle, "Block");
        }

        public static Expression UnbundleCallExpression(Expression argumentBundle, Expression parameterBinders)
        {
            return Expression.Call(argumentBundle, "Unbundle", null, parameterBinders);
        }
    }
}