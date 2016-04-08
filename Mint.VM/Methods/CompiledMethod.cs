using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint
{
    public class CompiledMethod : Method
    {
        public CompiledMethod(Symbol name, Module owner, MethodInfo info) : base(name, owner)
        {
            MethodInfo = info;
        }

        public MethodInfo MethodInfo { get; }

        public override Expression Bind(Expression instance, IEnumerable<Expression> args)
        {
            var parms = MethodInfo.GetParameters().Select(_ => _.ParameterType);

            if(MethodInfo.IsStatic)
            {
                args     = new[] { instance }.Concat(args);
                parms    = new[] { MethodInfo.DeclaringType }.Concat(parms);
                instance = null;
            }

            if(instance != null && MethodInfo.DeclaringType != null)
            {
                instance = Convert(instance, MethodInfo.DeclaringType);
            }

            args = args.Zip(parms, Convert);

            Expression call = Call(instance, MethodInfo, args);

            if(!typeof(iObject).IsAssignableFrom(MethodInfo.ReturnType))
            {
                call = Call(
                    OBJECT_BOX_METHOD,
                    Convert(call, typeof(object))
                );
            }

            return call;
        }

        public static readonly MethodInfo OBJECT_BOX_METHOD = Reflector<object>.Method(_ => Object.Box(_));
    }
}
