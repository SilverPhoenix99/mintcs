using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint
{
    public class Class : Module
    {
        public Class(Class superclass, Symbol? name = null, Module container = null, bool isSingleton = false)
            : base(CLASS, name, container)
        {
            Superclass = superclass;
            IsSingleton = isSingleton;
        }

        public Class(Symbol? name = null, Module container = null, bool isSingleton = false)
            : this(Object.CLASS, name, container, isSingleton)
        { }

        ~Class()
        {
            if(Superclass != null)
            {
                var list = Superclass.Subclasses;
                for(int i = 0; i < list.Count; i++)
                {
                    Class klass;
                    WeakReference<Class> wr = list[i];
                    if(wr.TryGetTarget(out klass) && klass == this)
                    {
                        list.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public Class         Superclass  { get; }
        public bool          IsSingleton { get; }
        public override bool IsModule    => false;

        protected internal IList<WeakReference<Class>> Subclasses { get; } = new List<WeakReference<Class>>();

        public override IEnumerable<Module> Ancestors =>
            Superclass == null ? base.Ancestors : base.Ancestors.Concat(Superclass.Ancestors);

        public override void Include(Module module)
        {
            Included = AppendModule(Included, module, Superclass);
        }

        public override void Prepend(Module module)
        {
            Prepended = AppendModule(Prepended, module, Superclass);
        }

        public override Method FindMethod(Symbol name)
        {
            // Method resolution: See Object#FindMethod

            throw new NotImplementedException();

            for(var klass = this; klass != null; klass = klass.Superclass)
            {
                Method method;
                if(Methods.TryGetValue(name, out method))
                {
                    return method;
                }
            }

            return null;
        }

        // Tries to call the dynamically defined (at runtime) instance method.
        // If the method was created statically or doesn't exist, returns false.
        /*public bool TryInvokeMethod(iObject value, string name, out iObject result, params object[] args)
        {
            var deleg = FindMethod(name);
            if(deleg == null)
            {
                result = null;
                return false;
            }

            var args2 = new[] { value }.Concat(args).ToArray();

            result = Object.Box(deleg.DynamicInvoke(args2));

            // make sure ref/out parameters get assigned
            Array.Copy(args2, 1, args, 0, args.Length);
            return true;
        }


        public bool TryInvokeMethod(iObject value, string name, params object[] args)
        {
            iObject result;
            return TryInvokeMethod(value, name, out result, args);
        }*/

        private Delegate WrapInstanceMethod(MethodInfo info)
        {
            var parms = from p in info.GetParameters()
                        select Parameter(p.ParameterType, p.Name);

            var lambdaParm = Parameter(info.DeclaringType, "_");

            var wrapper = Lambda(
                Call(lambdaParm, info, parms),
                new[] { lambdaParm }.Concat(parms)
            );

            return wrapper.Compile();
        }

        #region Static

        public new static readonly Class CLASS;

        public static bool IsA(iObject o, Class c)
        {
            if(Nil.IsNil(c))
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

        static Class()
        {
            CLASS = new Class(Module.CLASS, new Symbol("Class"));
        
            // difficult cyclical dependency:
            /*if(CLASS != null)
            {
                DefineClass(CLASS);
            }

            CLASS.DefineMethod(new Symbol("to_s"),
                (Func<Class, String>) ((self) => new String(self.FullName)));
            */
        }

        #endregion
    }
}
