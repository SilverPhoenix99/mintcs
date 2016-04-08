using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint
{
    public class CompiledProperty : Method
    {
        public CompiledProperty(Symbol name, Module owner, PropertyInfo property) : base(name, owner)
        {
            Property = property;
        }

        public PropertyInfo Property { get; }

        public override Expression Bind(Expression instance, IEnumerable<Expression> args)
        {
            if(Property.DeclaringType != null)
            {
                instance = Convert(instance, Property.DeclaringType);
            }

            var parms = Property.GetIndexParameters().Select(_ => _.ParameterType);
            args = args.Zip(parms, Convert);

            Expression call = Property(instance, Property, args);

            if(!typeof(iObject).IsAssignableFrom(Property.PropertyType))
            {
                call = Call(
                    CompiledMethod.OBJECT_BOX_METHOD,
                    Convert(call, typeof(object))
                );
            }

            return call;
        }
    }
}
