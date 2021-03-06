﻿using Mint.MethodBinding.Methods;
using Mint.Reflection.Parameters.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using Mint.Reflection;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint
{
    public class Module : BaseObject
    {
        private static readonly Regex CONSTANT_NAME =
            new Regex("^[A-Z](?:[A-Za-z0-9_]|[^\x00-\x7F])*$", RegexOptions.Compiled);


        private static readonly Regex NAME_PATH =
            new Regex("^(?:::)?([^:]+)(?:::([^:]+))*$", RegexOptions.Compiled);


        protected readonly IDictionary<Symbol, iObject> constants;
        private Module container;


        public Module(Symbol? name = null, Module container = null)
            : this(Class.MODULE, name, container)
        { }


        protected Module(Class klass, Symbol? baseName = null, Module container = null)
            : base(klass)
        {
            // Called by subclasses to prepare Class property. See class Class.

            if(container != null && baseName == null)
            {
                throw new ArgumentNullException($"{nameof(baseName)} cannot be null if Module has {nameof(container)}");
            }

            BaseName = baseName;
            this.container = container ?? Class.OBJECT;
            Methods = new Dictionary<Symbol, MethodBinder>();
            constants = new Dictionary<Symbol, iObject>();
            Included = new List<Module>();
            Prepended = new List<Module>();
            Subclasses = new List<WeakReference<Class>>();

            if(this.container != null && baseName != null)
            {
                this.container.SetConstant(baseName.Value, this);
            }
        }


        ~Module()
        {
            // TODO : invalidate methods, including subclasses
        }


        public Symbol? BaseName { get; private set; }
        public Module Container => container ?? (container = Class.OBJECT);
        public virtual bool IsModule => true;
        public virtual IEnumerable<Module> Ancestors => Prepended.Concat(new[] { this }).Concat(Included);
        public IEnumerable<Module> IncludedModules => Prepended.Concat(Included);
        protected internal IDictionary<Symbol, MethodBinder> Methods { get; }
        public IEnumerable<Symbol> Constants => constants.Keys;
        protected IList<Module> Included { get; set; }
        protected IList<Module> Prepended { get; set; }
        protected IList<WeakReference<Class>> Subclasses { get; }


        public string Name
        {
            get
            {
                var baseName = BaseName?.Name ?? base.ToString();
                return ReferenceEquals(Container, Class.OBJECT) ? baseName : $"{Container.Name}::{baseName}";
            }
        }


        private string CalculateFullName(string name, Module container)
        {
            if(name == null)
            {
                name = base.ToString();
            }

            if(!ReferenceEquals(container, Class.OBJECT))
            {
                name = $"{container.Name}::{name}";
            }

            return name;
        }


        public override string ToString()
            => Name;


        public Symbol DefineMethod(MethodBinder method)
        {
            // TODO invalidate previously defined method.
            return ( Methods[method.Name] = method ).Name;
        }


        public virtual void Include(Module module)
        {
            Included = AppendModule(Included, module, null);
        }


        public virtual void Prepend(Module module)
        {
            Prepended = AppendModule(Prepended, module, null);
        }


        protected List<Module> AppendModule(IList<Module> modules, Module module, Class superclass)
        {
            if(module.GetType() != typeof(Module))
            {
                throw new TypeError($"wrong argument type {module.Class.Name} (expected Module)");
            }

            var included = new List<Module>(modules.Count + module.Prepended.Count + module.Included.Count + 1);
            included.AddRange(modules);

            var superclassAncestors = superclass == null
                ? null
                : new HashSet<Module>(superclass.Ancestors);

            var index = 0;
            foreach(var mod in module.Ancestors)
            {
                if(ReferenceEquals(mod, this))
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


        public MethodBinder FindMethod(Symbol methodName)
        {
            foreach(var mod in Ancestors)
            {
                if(!mod.Methods.TryGetValue(methodName, out var method) || !method.Condition.Valid)
                {
                    continue;
                }

                if(!ReferenceEquals(mod, this))
                {
                    DefineMethod(method.Duplicate());
                }

                return method;
            }

            return null;
        }


        public bool IsConstantDefined(Symbol name, [Optional] bool inherit = true)
        {
            // If constant found, then name is valid
            if(constants.ContainsKey(name))
            {
                return true;
            }

            // Only check name if not found
            ValidateConstantName(name.Name);
            return false;
        }


        public bool IsConstantDefined(String name, [Optional] bool inherit = true)
        {
            throw new NotImplementedException(nameof(IsConstantDefined));
        }


        public iObject GetConstant(Symbol name, [Optional] bool inherit = true)
            => GetConstant(name, null, inherit);


        public iObject GetConstant(String path, [Optional] bool inherit = true)
        {
            throw new NotImplementedException();
        }


        public iObject GetConstant(Symbol name, IEnumerable<Module> nesting, bool inherit = true)
        {
            var constant = TryGetConstant(name, nesting, inherit);

            if(constant == null)
            {
                throw UninitializedConstant(name);
            }

            return constant;
        }


        public iObject TryGetConstant(Symbol name, IEnumerable<Module> nesting, bool inherit = true)
        {
            ValidateConstantName(name.Name);

            var modules = inherit ? Ancestors : new[] { this };
            if(nesting != null)
            {
                modules = nesting.Concat(modules).Distinct();
            }

            var module = modules.FirstOrDefault(_ => _.IsConstantDefined(name, false));
            return module?.constants[name];
        }


        private static void ValidateConstantName(string name)
        {
            if(!CONSTANT_NAME.IsMatch(name))
            {
                throw new NameError($"wrong constant baseName {name}");
            }
        }


        public iObject SetConstant(Symbol name, iObject value)
        {
            if(IsConstantDefined(name, false))
            {
                var fullName = ReferenceEquals(this, Class.OBJECT) ? name.Name : $"{Name}::{name}";
                Console.Error.WriteLine($"warning: already initialized constant {fullName}");
            }

            ValidateConstantName(name.Name);

            if(value is Module module && module.BaseName == null)
            {
                module.BaseName = name;
                module.container = this;
            }

            return constants[name] = value;
        }


        protected iObject TryGetConstant(Symbol name)
        {
            constants.TryGetValue(name, out var constant);
            return constant;
        }


        private Exception UninitializedConstant(Symbol name)
        {
            var fullName = ReferenceEquals(this, Class.OBJECT) ? name.Name : $"{Name}::{name}";
            return new NameError($"uninitialized constant {fullName}");
        }


        protected Module GetOrCreateModule(Symbol name, IEnumerable<Module> nesting)
        {
            var constant = TryGetConstant(name, nesting);

            switch(constant)
            {
                case null:
                    return new Module(name, this);
                case Module mod:
                    return mod;
                default:
                    throw new TypeError($"{constant.Inspect()} is not a module");
            }
        }


        protected static Module GetOrCreateModuleWithParentCast(iObject parent,
                                                                Symbol name,
                                                                IEnumerable<Module> nesting)
        {
            switch(parent)
            {
                case Module mod:
                    return mod.GetOrCreateModule(name, nesting);
                case iObject vmObj:
                    throw new TypeError($"{vmObj.Inspect()} is not a class/module");
                default:
                    throw new TypeError($"{parent} is not a class/module");
            }
        }


        protected Class GetOrCreateClass(Symbol name, Class superclass, IEnumerable<Module> nesting)
        {
            var constant = TryGetConstant(name, nesting);

            if(constant == null)
            {
                return superclass == null ? new Class(name, this) : new Class(superclass, name, this);
            }

            if(!(constant is Class))
            {
                throw new TypeError(constant.Inspect() + " is not a class");
            }

            if(superclass != null && ((Class) constant).Superclass != superclass)
            {
                throw new TypeError($"superclass mismatch for class {name}");
            }

            return (Class) constant;
        }


        protected static Class GetOrCreateClassWithParentCast(iObject parent,
                                                              Symbol name,
                                                              Class superclass,
                                                              IEnumerable<Module> nesting)
        {
            if(parent is Module mod)
            {
                return mod.GetOrCreateClass(name, superclass, nesting);
            }

            throw new TypeError($"{parent.Inspect()} is not a class/module");
        }


        public static class Reflection
        {
            public static readonly MethodInfo DefineMethod = Reflector<Module>.Method(
                _ => _.DefineMethod(default)
            );


            public static readonly MethodInfo GetConstant = Reflector<Module>.Method(
                _ => _.GetConstant(default, default, default)
            );


            public static readonly MethodInfo TryGetConstant = Reflector<Module>.Method(
                _ => _.TryGetConstant(default, default, default)
            );


            public static readonly MethodInfo SetConstant = Reflector<Module>.Method(
                _ => _.SetConstant(default, default)
            );


            public static readonly MethodInfo GetOrCreateModule = Reflector<Module>.Method(
                _ => _.GetOrCreateModule(default, default)
            );


            public static readonly MethodInfo GetOrCreateModuleWithParentCast = Reflector.Method(
                () => GetOrCreateModuleWithParentCast(default, default, default)
            );


            public static readonly MethodInfo GetOrCreateClass = Reflector<Module>.Method(
                _ => _.GetOrCreateClass(default, default, default)
            );


            public static readonly MethodInfo GetOrCreateClassWithParentCast = Reflector.Method(
                () => GetOrCreateClassWithParentCast(default, default, default, default)
            );
        }


        public static class Expressions
        {
            public static MethodCallExpression DefineMethod(Expression module, Expression methodBinder)
                => Call(module, Reflection.DefineMethod, methodBinder);


            public static MethodCallExpression GetConstant(Expression module,
                                                           Expression name,
                                                           Expression inherit = null,
                                                           Expression nesting = null)
                => Call(
                    module,
                    Reflection.GetConstant,
                    name,
                    nesting ?? Constant(System.Array.Empty<Module>(), typeof(IEnumerable<Module>)),
                    inherit ?? Constant(true)
                );


            public static MethodCallExpression TryGetConstant(Expression module,
                                                              Expression name,
                                                              Expression inherit = null,
                                                              Expression nesting = null)
                => Call(
                    module,
                    Reflection.TryGetConstant,
                    name,
                    nesting ?? Constant(System.Array.Empty<Module>(), typeof(IEnumerable<Module>)),
                    inherit ?? Constant(true)
                );


            public static MethodCallExpression SetConstant(Expression module, Expression name, Expression value)
                => Call(module, Reflection.SetConstant, name, value);


            public static MethodCallExpression GetOrCreateModule(Expression module,
                                                                 Expression name,
                                                                 Expression nesting = null)
                => Call(module, Reflection.GetOrCreateModule, name, nesting ?? Constant(System.Array.Empty<Module>()));


            public static MethodCallExpression GetOrCreateModuleWithParentCast(Expression parent,
                                                                               Expression name,
                                                                               Expression nesting = null)
                => Call(
                    Reflection.GetOrCreateModuleWithParentCast,
                    parent,
                    name,
                    nesting ?? Constant(System.Array.Empty<Module>())
                );


            public static MethodCallExpression GetOrCreateClass(Expression module,
                                                                Expression name,
                                                                Expression superclass = null,
                                                                Expression nesting = null)
                => Call(
                    module,
                    Reflection.GetOrCreateClass,
                    name,
                    superclass ?? Constant(null, typeof(Class)),
                    nesting ?? Constant(System.Array.Empty<Module>())
                );


            public static MethodCallExpression GetOrCreateClassWithParentCast(Expression parent,
                                                                              Expression name,
                                                                              Expression superclass = null,
                                                                              Expression nesting = null)
                => Call(
                    Reflection.GetOrCreateClassWithParentCast,
                    parent,
                    name,
                    superclass ?? Constant(null, typeof(Class)),
                    nesting ?? Constant(System.Array.Empty<Module>())
                );
        }
    }
}
