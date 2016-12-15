﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;

namespace Mint
{
    public partial class Class : Module
    {
        public Class Superclass { get; }

        public bool IsSingleton { get; }

        public override bool IsModule => false;

        public override IEnumerable<Module> Ancestors =>
            Superclass == null ? base.Ancestors : base.Ancestors.Concat(Superclass.Ancestors);

        public Class(Class superclass, Symbol? name = null, Module container = null, bool isSingleton = false)
            : base(CLASS, name, container)
        {
            Superclass = superclass;
            IsSingleton = isSingleton;
        }

        public Class(Symbol? name = null, Module container = null, bool isSingleton = false)
            : this(OBJECT, name, container, isSingleton)
        { }

        ~Class()
        {
            if(Superclass == null)
            {
                return;
            }

            var list = Superclass.Subclasses;
            for(var i = 0; i < list.Count; i++)
            {
                Class klass;
                var weakRef = list[i];
                if(!weakRef.TryGetTarget(out klass) || klass != this)
                {
                    continue;
                }

                list.RemoveAt(i);
                break;
            }
        }

        public override void Include(Module module)
        {
            Included = AppendModule(Included, module, Superclass);
        }

        public override void Prepend(Module module)
        {
            Prepended = AppendModule(Prepended, module, Superclass);
        }

        public static bool IsA(iObject o, Class c)
        {
            if(NilClass.IsNil(c))
            {
                throw new TypeError("class or module required");
            }

            for(var k = o.Class; k != null; k = k.Superclass)
            {
                if(c == k)
                {
                    return true;
                }
            }

            return false;
        }

        public new static class Reflection
        {
            public static readonly ConstructorInfo Ctor = Reflector<Class>.Ctor<Class, Symbol?, Module, bool>();
        }

        public new static class Expressions
        {
            public static NewExpression New(Expression superclass,
                                            Expression name = null,
                                            Expression container = null,
                                            Expression isSingleton = null) =>
                Expression.New(
                    Reflection.Ctor,
                    superclass,
                    name ?? Expression.Constant(null, typeof(Symbol?)),
                    container ?? Expression.Constant(null, typeof(Module)),
                    isSingleton ?? Expression.Constant(false)
                );
        }
    }
}
