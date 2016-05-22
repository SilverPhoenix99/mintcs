using Mint.MethodBinding.Binders;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;

namespace Mint.MethodBinding
{
    public class ArgumentBundle
    {
        private static readonly PropertyInfo PROPERTY_SPLAT = Reflector<ArgumentBundle>.Property(_ => _.Splat);
        private static readonly PropertyInfo PROPERTY_KEYS = Reflector<ArgumentBundle>.Property(_ => _.Keys);
        private static readonly PropertyInfo PROPERTY_BLOCK = Reflector<ArgumentBundle>.Property(_ => _.Block);

        private static readonly MethodInfo METHOD_UNBUNDLE = Reflector<ArgumentBundle>.Method(
            _ => _.Unbundle(default(IEnumerable<ParameterBinder>))
        );

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

        public static Expression SplatExpression(Expression argumentBundle) =>
            Expression.Property(argumentBundle, PROPERTY_SPLAT);

        public static Expression KeysExpression(Expression argumentBundle) =>
            Expression.Property(argumentBundle, PROPERTY_KEYS);

        public static Expression BlockExpression(Expression argumentBundle) =>
            Expression.Property(argumentBundle, PROPERTY_BLOCK);

        public static Expression UnbundleCallExpression(Expression argumentBundle, Expression parameterBinders) =>
            Expression.Call(argumentBundle, METHOD_UNBUNDLE, parameterBinders);
    }
}