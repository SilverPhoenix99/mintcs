using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Ex=System.Linq.Expressions.Expression;

namespace mint.types
{
    class Class : Object
    {
        public Class(Class superclass = null, string name = null) : base(CLASS)
        {
            Super = superclass ?? Object.CLASS;
            Name = name;
        }


        public string                        Name        { get; set; }
        public Class                         Super       { get; set; }
        public Class                         Container   { get; set; }
        public IDictionary<string, Delegate> Methods     { get; } = new Dictionary<string, Delegate>();
        public IDictionary<string, iObject>  Constants   { get; } = new Dictionary<string, iObject>();
        public bool                          IsSingleton { get; set; }
        public bool                          IsModule    { get; set; }


        public String FullName
        {
            get
            {
                if(IsSingleton)
                {
                    return (String) $"#<Class:{Super.FullName}>";
                }

                if(Name == null)
                {
                    return (String) base.ToString();
                }

                var names = new List<string>();
                names.Add(Name);
                var container = Container;
                while(container != null)
                {
                    names.Add(container.Name);
                    container = container.Container;
                }

                names.Reverse();
                return (String) string.Join("::", names);
            }
        }


        public string Def(string name, Delegate func)
        {
            Methods[name] = func;
            return name;
        }


        public string Def(string name, MethodInfo info)
        {
            // is it an instance method from an iObject?
            if(!info.IsStatic && typeof(iObject).IsAssignableFrom(info.DeclaringType))
            {
                // generate wrapping delegate
                return Def(name, WrapInstanceMethod(info));
            }

            var parms = info.GetParameters().Select(((_) => _.ParameterType))
                .Concat(new[] { info.ReturnType })
                .ToArray();

            var delegateType = Ex.GetDelegateType(parms);

            object instance = null;
            if(!info.IsStatic && info.DeclaringType.Name.StartsWith("<>"))
            {
                instance = info.DeclaringType.GetConstructor(Type.EmptyTypes).Invoke(null);
            }

            var deleg = Delegate.CreateDelegate(delegateType, instance, info);
            return Def(name, deleg);
        }


        public string Def<Type>(string ruby_name, string native_name)
        {
            var mInfo = typeof(Type).GetMethod(native_name);

            if(mInfo != null)
            {
                return Def(ruby_name, mInfo);
            }

            var pInfo = typeof(Type).GetProperty(native_name);

            if(pInfo == null)
            {
                throw new NoMethodError("undefined native method `" + native_name + "' for type " + typeof(Type).FullName);
            }

            mInfo = ruby_name.EndsWith("=") ? pInfo.GetSetMethod() : pInfo.GetGetMethod();

            if(mInfo == null)
            {
                throw new NoMethodError("undefined native property `" + native_name + "' for " + typeof(Type).FullName);
            }

            return Def(ruby_name, mInfo);
        }


        public Delegate GetMethod(string name)
        {
            Delegate deleg;
            Methods.TryGetValue(name, out deleg);
            return deleg;
        }


        public Delegate FindMethod(string name)
        {
            Delegate deleg = null;
            for(var klass = this; klass != null; klass = klass.Super)
            {
                deleg = klass.GetMethod(name);
                if(deleg != null)
                {
                    break;
                }
            }

            return deleg;
        }


        // Tries to call the dynamically defined (at runtime) instance method.
        // If the method was created statically or doesn't exist, returns false.
        public bool TryInvokeMethod(iObject value, string name, out iObject result, params object[] args)
        {
            var deleg = FindMethod(name);
            if(deleg == null)
            {
                result = null;
                return false;
            }

            var args2 = new[] { value }.Concat(args).ToArray();

            result = Object.Convert(deleg.DynamicInvoke(args2));

            // make sure ref/out parameters get assigned
            Array.Copy(args2, 1, args, 0, args.Length);
            return true;
        }


        public bool TryInvokeMethod(iObject value, string name, params object[] args)
        {
            iObject result;
            return TryInvokeMethod(value, name, out result, args);
        }

        
        private Delegate WrapInstanceMethod(MethodInfo info)
        {
            var parms = from p in info.GetParameters()
                        select Ex.Parameter(p.ParameterType, p.Name);

            var allParms = new[] { Ex.Parameter(info.DeclaringType, "_") }.Concat(parms);

            var wrapper = Ex.Lambda(
                Ex.Call(allParms.First(), info, parms),
                allParms
            );

            return wrapper.Compile();
        }


        public override string ToString() => FullName;


        public new static readonly Class CLASS = new Class(name: "Class");


        static Class()
        {
            // difficult cyclical dependency:
            if(CLASS != null)
            {
                Object.CLASS.Constants[CLASS.Name] = CLASS;
            }

            CLASS.Def<Class>("to_s", "FullName");
        }
    }
}
