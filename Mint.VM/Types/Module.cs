﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mint
{
    public class Module : BaseObject
    {
        public Module(Symbol? name = null, Module container = null)
            : this(CLASS, name, container)
        { }

        protected Module(Class klass, Symbol? name = null, Module container = null)
            : base(klass)
        {
            // Called by subclasses to prepare Class property. See class Class.

            Name = name;
            Container = container;

            FullName = name != null
                     ? name.ToString()
                     : $"#<{Class.Name}:0x{Id:x}>";

            if(container != null)
            {
                FullName = string.Concat(container.FullName, "::", FullName);
            }
        }

        ~Module()
        {
            // TODO : invalidate methods, including subclasses
        }

        public         Symbol?             Name            { get; }
        public         string              FullName        { get; }
        public         Module              Container       { get; }
        public virtual bool                IsModule        => true;
        public virtual IEnumerable<Module> Ancestors       => Prepended.Concat(new[] { this }).Concat(Included);
        public         IEnumerable<Module> IncludedModules => Prepended.Concat(Included);

        protected internal Dictionary<Symbol, Method>  Methods   { get; } = new Dictionary<Symbol, Method>();
        protected internal Dictionary<Symbol, iObject> Constants { get; } = new Dictionary<Symbol, iObject>();
        protected internal List<Module>                Included  { get; protected set; } = new List<Module>();
        protected internal List<Module>                Prepended { get; protected set; } = new List<Module>();

        protected internal IList<WeakReference<Class>> Subclasses { get; } = new List<WeakReference<Class>>();

        public override string ToString() => FullName;

        public Symbol DefineMethod(Method method) => ( Methods[method.Name] = method ).Name;

        public Symbol DefineMethod(Symbol name, MethodInfo function) =>
            DefineMethod(new CompiledMethod(name, this, function));

        public Symbol DefineMethod(string name, MethodInfo function) => DefineMethod(new Symbol(name), function);

        public Symbol DefineMethod(Symbol name, Method.Delegate function) =>
            DefineMethod(new LambdaMethod(name, this, function));

        public Symbol DefineMethod(string name, Method.Delegate function) => DefineMethod(new Symbol(name), function);

        public Symbol DefineMethod(Symbol name, PropertyInfo function) =>
            DefineMethod(new CompiledProperty(name, this, function));

        public Symbol DefineMethod(string name, PropertyInfo function) => DefineMethod(new Symbol(name), function);

        public virtual void Include(Module module)
        {
            Included = AppendModule(Included, module, null);
        }

        public virtual void Prepend(Module module)
        {
            Prepended = AppendModule(Prepended, module, null);
        }

        protected List<Module> AppendModule(List<Module> modules, Module module, Class superclass)
        {
            if(module.GetType() != typeof(Module))
            {
                throw new TypeError($"wrong argument type {module.Class.FullName} (expected Module)");
            }

            var included = new List<Module>(modules.Count + module.Prepended.Count + module.Included.Count + 1);
            included.AddRange(modules);

            var superclassAncestors = superclass == null
                ? null
                : new HashSet<Module>(superclass.Ancestors);

            var index = 0;
            foreach(var mod in module.Ancestors)
            {
                if(mod == this)
                {
                    throw new ArgumentError("cyclic include detected");
                }

                // exclude modules already included by superclasses
                if(superclassAncestors != null && superclassAncestors.Contains(mod))
                {
                    continue;
                }

                var modIndex = included.IndexOf(mod);
                if(modIndex >= 0)
                {
                    index = modIndex + 1;
                    continue;
                }

                included.Insert(index++, mod);

                // TODO mod.included(this) if mod.respond_to?(:included)
            }

            return included;
        }

        public Method FindMethod(Symbol methodName)
        {
            foreach(var mod in Ancestors)
            {
                Method method;
                if(!mod.Methods.TryGetValue(methodName, out method) || !method.Condition.Valid)
                {
                    continue;
                }

                if(mod != this)
                {
                    DefineMethod(method.Duplicate());
                }

                return method;
            }

            return null;
        }

        #region Static

        public static readonly Class CLASS;

        static Module()
        {
            CLASS = new Class(new Symbol(MethodBase.GetCurrentMethod().DeclaringType.Name));

            CLASS.DefineMethod(new Symbol("to_s"), Reflector<Module>.Method(_ => _.ToString()));
            CLASS.DefineMethod(new Symbol("inspect"), Reflector<Module>.Method(_ => _.Inspect()));
        }

        #endregion
    }
}
