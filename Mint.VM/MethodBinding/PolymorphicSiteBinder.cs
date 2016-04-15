using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding
{
    public sealed class PolymorphicSiteBinder : CallSiteBinder
    {
        private readonly Dictionary<long, MethodBinder> cache = new Dictionary<long, MethodBinder>();

        public Function Compile(CallSite site)
        {
            var invalidKeys = cache.Where(_ => !_.Value.Condition.Valid).Select(_ => _.Key).ToArray();
            foreach(var key in invalidKeys)
            {
                cache.Remove(key);
            }

            if(cache.Count == 0)
            {
                return (inst, parms) => DefaultCall(site, inst, parms);
            }

            if(cache.Count > MEGAMORPHIC_THREASHOLD)
            {
                site.Binder = new MegamorphicSiteBinder(cache, site);
                return site.Binder.Compile(site);
            }

            var instance = Parameter(typeof(iObject), "instance");

            var args = Parameter(typeof(iObject[]), "args");

            // TODO assuming always ParameterKind.Req. change to accept Block, Rest, KeyReq, KeyRest
            var unsplatArgs = Enumerable.Range(0, site.Parameters.Length).Select(i => ArrayIndex(args, Constant(i)));

            var retTarget = Label(typeof(iObject), "return");
            var classId = Variable(typeof(long), "classId");

            var cases = cache.Select(_ => CreateCase(_.Key, _.Value, site, instance, unsplatArgs, retTarget));

            var body = Block(
                typeof(iObject),
                new[] { classId },
                // var $class = instance.CalculatedClass.Id;
                Assign(classId, Property(Property(instance, PROP_CALCULATEDCLASS), PROP_ID)),
                // switch($classId) { ... }
                Switch(classId, null, null, cases),
                // @cache[$classId] = Object.FindMethod(instance, @method_name, args);
                Call(
                    Constant(cache),
                    METHOD_CACHE_INDEXER,
                    classId,
                    Call(METHOD_FINDMETHOD, instance, Constant(site.MethodName), args)
                ),
                // @site.Call = @binder.Compile(@site);
                Assign(
                    Property(Constant(site), PROP_CALL),
                    Call(Constant(this), METHOD_COMPILE, Constant(site))
                ),
                // return @site.Call(instance, args);
                Label(
                    retTarget,
                    Invoke(Property(Constant(site), PROP_CALL), instance, args)
                )
            );

            // (iObject instance, iObject[] args) : iObject => { @body }
            var lambda = Lambda<Function>(body, instance, args);
            return lambda.Compile();
        }

        private static SwitchCase CreateCase(long classId, MethodBinder binder, CallSite site,
                                             Expression instance, IEnumerable<Expression> args,
                                             LabelTarget retTarget)
        {
            // case @class_id:
            // {
            //     if(@condition.Valid)
            //     {
            //         return @method(instance, args);
            //     }
            // }
            return SwitchCase(
                IfThen(
                    Property(Constant(binder.Condition), PROP_VALID),
                    Return(retTarget, binder.Bind(site, retTarget, instance, args), typeof(iObject))
                ),
                Constant(classId)
            );
        }

        private iObject DefaultCall(CallSite site, iObject instance, iObject[] args)
        {
            var method = Object.FindMethod(instance, site.MethodName, args);
            cache[instance.CalculatedClass.Id] = method;
            site.Call = Compile(site);
            return site.Call(instance, args);
        }

        private const int MEGAMORPHIC_THREASHOLD = 32;

        private static readonly PropertyInfo PROP_CALCULATEDCLASS = Reflector<iObject>.Property(
            _ => _.CalculatedClass
        );

        private static readonly PropertyInfo PROP_ID = Reflector<iObject>.Property(
            _ => _.Id
        );

        private static readonly MethodInfo METHOD_FINDMETHOD = Reflector.Method(
            () => Object.FindMethod(default(iObject), default(Symbol), default(iObject[]))
        );

        private static readonly MethodInfo METHOD_CACHE_INDEXER = Reflector<Dictionary<long, MethodBinder>>.Setter(
            _ => _[default(long)]
        );

        private static readonly PropertyInfo PROP_CALL = Reflector<CallSite>.Property(
            _ => _.Call
        );

        private static readonly MethodInfo METHOD_COMPILE = Reflector<CallSiteBinder>.Method(
            _ => _.Compile(default(CallSite))
        );

        private static readonly PropertyInfo PROP_VALID = Reflector<Condition>.Property(
            _ => _.Valid
        );
    }
}
