using System;
using System.Collections.Generic;
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

        public Func<iObject> Allocator { get; set; }

        public Class(Class superclass, Symbol? baseName = null, Module container = null, bool isSingleton = false)
            : base(CLASS, baseName, container)
        {
            if(Class.CLASS != null && superclass == Class.CLASS)
            {
                throw new TypeError("can't make subclass of Class");
            }

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

        public iObject Allocate()
        {
            if(Allocator == null)
            {
                var name = Name ?? Inspect();
                throw new TypeError($"allocator undefined for {name}");
            }

            return Allocator();
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
