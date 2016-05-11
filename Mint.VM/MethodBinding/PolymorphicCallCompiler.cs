using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.MethodBinding
{
    public sealed partial class PolymorphicCallCompiler : CallSiteBinder
    {
        private const int MEGAMORPHIC_THREASHOLD = 32;
        
        private static readonly PropertyInfo PROP_CALCULATEDCLASS = Reflector<iObject>.Property(_ => _.CalculatedClass);

        private static readonly PropertyInfo PROP_ID = Reflector<iObject>.Property(_ => _.Id);

        private static readonly MethodInfo METHOD_FINDMETHOD = Reflector.Method(
            () => Object.FindMethod(default(iObject), default(Symbol), default(iObject[]))
        );

        private static readonly MethodInfo METHOD_CACHE_INDEXER = Reflector<Dictionary<long, CachedMethod>>.Setter(
            _ => _[default(long)]
        );

        private static readonly PropertyInfo PROP_CALL = Reflector<CallSite>.Property(_ => _.Call);

        private static readonly MethodInfo METHOD_COMPILE = Reflector<CallSiteBinder>.Method(_ => _.Compile());

        private static readonly PropertyInfo PROP_VALID = Reflector<Condition>.Property(_ => _.Valid);

        private static readonly ConstructorInfo CTOR_CACHEDMETHOD =
            Reflector.Ctor<CachedMethod>(typeof(MethodBinder), typeof(CallSite), typeof(Expression), typeof(Expression));
        
        private readonly Dictionary<long, CachedMethod> cache = new Dictionary<long, CachedMethod>();
        private readonly ParameterExpression instance = Parameter(typeof(iObject), "instance");
        private readonly ParameterExpression args = Parameter(typeof(iObject[]), "args");
        
        public CallSite CallSite { get; }
        
        public PolymorphicCallCompiler(CallSite callSite)
        {
            CallSite = callSite;
        }

        public Function Compile()
        {
            DeleteInvalidCachedMethods();

            if(IsCacheEmpty())
            {
                return DefaultCall;
            }

            if(IsCacheFull())
            {
                var methodBinderCache = cache.Select(_ => new KeyValuePair<long, MethodBinder>(_.Key, _.Value.Binder));
                CallSite.CallCompiler = new MegamorphicCallCompiler(CallSite, methodBinderCache);
                return CallSite.CallCompiler.Compile();
            }

            var lambda = Lambda<Function>(BuildBodyExpression(), instance, args);
            return lambda.Compile();
        }
        
        private void DeleteInvalidCachedMethods()
        {
            var invalidKeys = cache.Where(_ => !_.Value.Binder.Condition.Valid).Select(_ => _.Key).ToArray();
            foreach(var key in invalidKeys)
            {
                cache.Remove(key);
            }
        }
        
        private bool IsCacheEmpty() => cache.Count == 0;

        private iObject DefaultCall(iObject self, iObject[] arguments)
        {
            var method = Object.FindMethod(self, CallSite.MethodName, arguments);
            cache[self.CalculatedClass.Id] = new CachedMethod(method, CallSite, instance, args);
            CallSite.Call = Compile();
            return CallSite.Call(self, arguments);
        }

        private bool IsCacheFull() => cache.Count > MEGAMORPHIC_THREASHOLD;
        
        private Expression BuildBodyExpression()
        {
            var retTarget = Label(typeof(iObject), "return");
            var classId = Variable(typeof(long), "classId");
            var switchCases = cache.Select(_ => CreateSwitchCase(_.Key, _.Value, retTarget));

            return Block(
                typeof(iObject),
                new[] { classId },
                
                // var $class = instance.CalculatedClass.Id;
                Assign(classId, Property(Property(instance, PROP_CALCULATEDCLASS), PROP_ID)),
                
                // switch($classId) { ... }
                Switch(classId, null, null, switchCases),

                // @cache[$classId] = new CachedMethod(Object.FindMethod(instance, @method_name, args), @site, @instance, @args);
                Call(
                    Constant(cache),
                    METHOD_CACHE_INDEXER,
                    classId,
                    New(
                        CTOR_CACHEDMETHOD,
                        Call(METHOD_FINDMETHOD, instance, Constant(CallSite.MethodName), args),
                        Constant(CallSite),
                        Constant(instance),
                        Constant(args)
                    )
                ),

                // @site.Call = @binder.Compile();
                Assign(
                    Property(Constant(CallSite), PROP_CALL),
                    Call(Constant(this), METHOD_COMPILE)
                ),

                // return @site.Call(instance, args);
                Label(
                    retTarget,
                    Invoke(Property(Constant(CallSite), PROP_CALL), instance, args)
                )
            );
        }

        private static SwitchCase CreateSwitchCase(long classId, CachedMethod method, LabelTarget retTarget)
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
                    Property(Constant(method.Binder.Condition), PROP_VALID),
                    Return(retTarget, method.Expression, typeof(iObject))
                ),
                Constant(classId)
            );
        }
    }
}
