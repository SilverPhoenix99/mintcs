using Mint.MethodBinding.Methods;
using Mint.Reflection.Parameters.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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

        protected internal Dictionary<Symbol, MethodBinder> Methods { get; } = new Dictionary<Symbol, MethodBinder>();

        protected internal Dictionary<Symbol, iObject> Constants { get; } = new Dictionary<Symbol, iObject>();

        protected internal List<Module> Included { get; protected set; } = new List<Module>();

        protected internal List<Module> Prepended { get; protected set; } = new List<Module>();

        protected internal IList<WeakReference<Class>> Subclasses { get; } = new List<WeakReference<Class>>();

        public Module(Symbol? name = null, Module container = null)
            : this(Class.MODULE, name, container)
        { }

        protected Module(Class klass, Symbol? name = null, Module container = null)
            : base(klass)
        {
            // Called by subclasses to prepare Class property. See class Class.

            Name = name;
            this.container = container ?? Class.OBJECT;

            FullName = name?.ToString() ?? base.ToString();

            if(this.container != Class.OBJECT)
            {
                FullName = $"{this.container.FullName}::{FullName}";
            }
        }

        ~Module()
        {
            // TODO : invalidate methods, including subclasses
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

        public Module GetModuleOrThrow(Symbol name, IEnumerable<Module> nesting = null)
        {
            var module = GetModule(name, nesting);
            if(module == null)
            {
                throw new TypeError($"{name} is not a module");
            }

            return module;
        }

        public Module GetOrCreateModule(Symbol name, IEnumerable<Module> nesting = null) =>
            GetModule(name, nesting) ?? SetConstant(name, new Module(name, this)) as Module;

        private Module GetModule(Symbol name, IEnumerable<Module> nesting)
        {
            var constant = GetConstant(name, nesting);
            if(constant == null)
            {
                var fullName = this == Class.OBJECT ? name.Name : $"{FullName}::{name}";
                throw new NameError($"uninitialized constant {fullName}");
            }

            return constant as Module;
        }

        public iObject GetConstant(Symbol name) => GetConstant(name, null);

        private iObject GetConstant(Symbol name, IEnumerable<Module> nesting)
        {
            ValidateConstantName(name.Name);

            IEnumerable<Module> modules = Ancestors;
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
            Constants[name] = value;
            return value;
        }
    }
}
