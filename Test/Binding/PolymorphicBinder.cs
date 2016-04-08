using System;
using System.Collections.Generic;
using Mint;

namespace Mint.Binding
{
    class PolymorphicBinder : Binder
    {
        //private Dictionary<long, Method> cache = new Dictionary<long, Method>();

        public PolymorphicBinder(Symbol methodName)
        {
            MethodName = methodName;
        }

        public Symbol MethodName { get; }

        public CallSite.Function Compile(CallSite site)
        {
            // TODO flush invalid methods : cache.Where(_ => _.Value.Condition.Valid);
            // TODO promote to megamorphic binder when cache is too big
            throw new NotImplementedException();
        }

        class CachedMethod
        {
            public CachedMethod(Method method)
            {
                Method = method;
            }

            public Method Method { get; }
        }
    }
}


/*
call site stub:

(iObject instance, iObject[] args) : iObject => {
    // constant Symbol                          @method_name;
    // global   CallSite                        @site;
    // global   PolymorphicBinder               @binder;
    // global   Dictionary<Class, CachedMethod> @cache;
    // constant long                            @class_id1  .. @class_id99;
    // global   CallSite.Function               @method1    .. @method99;
    // global   Condition                       @condition1 .. @condition99;

    var $class = instance.CalculatedClass;

    //Expression.Switch(
    //    typeof(iObject),
    //    switchValue, // Expression
    //    null,
    //    null,
    //    cases) // IEnumerable<SwitchCase>

    switch($class.Id)
    {
        case @class_id1:
        {
            if(@condition1.Valid)
            {
                return @method1(instance, args);
            }
            break;
        }

        // repeat for all @class_id*
    }

    @cache.Remove($class.Id);
    @cache[$class.Id] = Object.FindMethod(instance, @method_name);
    @site.Call = @binder.Compile(@site);
    return @site.Call(instance, args);
}
*/