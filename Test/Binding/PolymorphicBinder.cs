using System;
using System.Collections.Generic;
using Mint;

namespace Mint.Binding
{
    class PolymorphicBinder : Binder
    {
        private Dictionary<Class, CachedMethod> cache = new Dictionary<Class, CachedMethod>();

        public PolymorphicBinder(Symbol methodName)
        {
            MethodName = methodName;
        }

        public Symbol MethodName { get; }

        public Func<iObject, iObject[], iObject> Compile(CallSite site)
        {
            throw new NotImplementedException();
        }
    }
}


// stub:
/*
(CallSite site, iObject instance, iObject[] args) : iObject => {
    // global CallSite                          @site;
    // global Dictionary<Class, CachedMethod>   @cache;
    // global Class                             @class1     .. @class99;
    // global Func<iObject, iObject[], iObject> @method1    .. @method99;
    // global Condition                         @condition1 .. @condition99;

    var $class = instance.CalculatedClass;

    switch($class) // object.ReferenceEquals
    {
        case @class1: // .. @class99:
        {
            if(!@condition1.Valid)
            {
                return @method1();

                @cache.Remove(@class1);
                goto default:
            }
        }

        default:
        {
            @site.
        }
    }
}
*/