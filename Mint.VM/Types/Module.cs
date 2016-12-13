using Mint.MethodBinding.Methods;
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

        public Symbol? Name { get; }

        public string FullName { get; }

        private Module container;
        public Module Container => container ?? (container = Class.OBJECT);

        public virtual bool IsModule => true;

        public virtual IEnumerable<Module> Ancestors => Prepended.Concat(new[] { this }).Concat(Included);

        public IEnumerable<Module> IncludedModules => Prepended.Concat(Included);

        protected internal IDictionary<Symbol, MethodBinder> Methods { get; }

        protected IDictionary<Symbol, iObject> Constants { get; }

        protected internal IList<Module> Included { get; protected set; }

        protected internal IList<Module> Prepended { get; protected set; }

        protected internal IList<WeakReference<Class>> Subclasses { get; }

        public Module(Symbol? name = null, Module container = null)
            : this(Class.MODULE, name, container)
        { }

        protected Module(Class klass, Symbol? name = null, Module container = null)
            : base(klass)
        {
            // Called by subclasses to prepare Class property. See class Class.

            Name = name;
            this.container = container ?? Class.OBJECT;
            FullName = CalculateFullName(name?.ToString(), this.container);
            Methods = new Dictionary<Symbol, MethodBinder>();
            Constants = new Dictionary<Symbol, iObject>();
            Included = new List<Module>();
            Prepended = new List<Module>();
            Subclasses = new List<WeakReference<Class>>();
        }

        ~Module()
        {
            // TODO : invalidate methods, including subclasses
        }

        private string CalculateFullName(string name, Module container)
        {
            if(name == null)
            {
                name = base.ToString();
            }

            if(container != Class.OBJECT)
            {
                name = $"{container.FullName}::{name}";
            }

            return name;
        }

        public override string ToString() => FullName;

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

        public MethodBinder FindMethod(Symbol methodName)
        {
            foreach(var mod in Ancestors)
            {
                MethodBinder method;
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

        public bool IsConstantDefined(Symbol name, [Optional] bool inherit = true)
        {
            // If constant found, then name is valid
            if(Constants.ContainsKey(name))
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

        public iObject GetConstant(Symbol name, [Optional] bool inherit = true) => GetConstant(name, null, inherit);

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

            IEnumerable<Module> modules = inherit ? Ancestors : new[] { this };
            if(nesting != null)
            {
                modules = nesting.Concat(modules).Distinct();
            }

            var module = modules.FirstOrDefault(_ => _.IsConstantDefined(name, false));
            return module?.Constants[name];
        }

        private static void ValidateConstantName(string name)
        {
            if(!CONSTANT_NAME.IsMatch(name))
            {
                throw new NameError($"wrong constant name {name}");
            }
        }

        public iObject SetConstant(Symbol name, iObject value)
        {
            if(IsConstantDefined(name, false))
            {
                var fullName = this == Class.OBJECT ? name.Name : $"{FullName}::{name}";
                Console.Error.WriteLine($"warning: already initialized constant {fullName}");
            }

            ValidateConstantName(name.Name);
            return Constants[name] = value;
        }

        protected iObject TryGetConstant(Symbol name)
        {
            iObject constant;
            Constants.TryGetValue(name, out constant);
            return constant;
        }

        private Exception UninitializedConstant(Symbol name)
        {
            var fullName = this == Class.OBJECT ? name.Name : $"{FullName}::{name}";
            return new NameError($"uninitialized constant {fullName}");
        }

        protected Module GetOrCreateModule(Symbol name, IEnumerable<Module> nesting)
        {
            var constant = TryGetConstant(name, nesting);

            if(constant == null)
            {
                constant = SetConstant(name, new Module(name, this));
            }
            else if(!(constant is Module))
            {
                throw new TypeError(constant.Inspect() + " is not a module");
            }

            return (Module) constant;
        }

        protected static Module GetModuleOrThrow(iObject parent, Symbol name, IEnumerable<Module> nesting)
        {
            if(parent is Module)
            {
                return ((Module) parent).GetOrCreateModule(name, nesting);
            }

            throw new TypeError($"{parent.Inspect()} is not a class/module");
        }

        public static class Reflection
        {
            public static readonly MethodInfo GetConstant = Reflector<Module>.Method(
                _ => _.GetConstant(default(Symbol), default(IEnumerable<Module>), default(bool))
            );

            public static readonly MethodInfo TryGetConstant = Reflector<Module>.Method(
                _ => _.TryGetConstant(default(Symbol), default(IEnumerable<Module>), default(bool))
            );

            public static readonly MethodInfo SetConstant = Reflector<Module>.Method(
                _ => _.SetConstant(default(Symbol), default(iObject))
            );

            public static readonly MethodInfo GetOrCreateModule = Reflector<Module>.Method(
                _ => _.GetOrCreateModule(default(Symbol), default(IEnumerable<Module>))
            );

            public static readonly MethodInfo GetModuleOrThrow = Reflector.Method(
                () => GetModuleOrThrow(default(iObject), default(Symbol), default(IEnumerable<Module>))
            );
        }

        public static class Expressions
        {
            public static MethodCallExpression GetConstant(Expression module,
                                                           Expression name,
                                                           Expression inherit = null,
                                                           Expression nesting = null) =>
                Call(
                    module,
                    Reflection.GetConstant,
                    name,
                    nesting ?? Constant(System.Array.Empty<Module>(), typeof(IEnumerable<Module>)),
                    inherit ?? Constant(true)
                );

            public static MethodCallExpression TryGetConstant(Expression module,
                                                              Expression name,
                                                              Expression inherit = null,
                                                              Expression nesting = null) =>
                Call(
                    module,
                    Reflection.TryGetConstant,
                    name,
                    nesting ?? Constant(System.Array.Empty<Module>(), typeof(IEnumerable<Module>)),
                    inherit ?? Constant(true)
                );

            public static MethodCallExpression SetConstant(Expression module, Expression name, Expression value) =>
                Call(module, Reflection.SetConstant, name, value);

            public static MethodCallExpression GetOrCreateModule(Expression module,
                                                                 Expression name,
                                                                 Expression nesting = null) =>
                Call(module, Reflection.GetOrCreateModule, name, nesting ?? Constant(System.Array.Empty<Module>()));

            public static MethodCallExpression GetModuleOrThrow(Expression parent,
                                                                Expression name,
                                                                Expression nesting = null) =>
                Call(Reflection.GetModuleOrThrow, parent, name, nesting ?? Constant(System.Array.Empty<Module>()));
        }
    }
}
