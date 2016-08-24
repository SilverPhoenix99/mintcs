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

        public Module(Symbol? name = null, Module container = null)
            : this(Class.MODULE, name, container)
        { }

        protected Module(Class klass, Symbol? name = null, Module container = null)
            : base(klass)
        {
            // Called by subclasses to prepare Class property. See class Class.

            Name = name;
            Container = container;

            FullName = name?.ToString() ?? base.ToString();

            if(container != null)
            {
                FullName = string.Concat(container.FullName, "::", FullName);
            }
        }

        ~Module()
        {
            // TODO : invalidate methods, including subclasses
        }

        public  Symbol? Name { get; }
        public  string FullName { get; }
        public  Module Container { get; }
        public virtual bool IsModule => true;
        public virtual IEnumerable<Module> Ancestors => Prepended.Concat(new[] { this }).Concat(Included);
        public IEnumerable<Module> IncludedModules => Prepended.Concat(Included);

        protected internal Dictionary<Symbol, MethodBinder> Methods { get; } = new Dictionary<Symbol, MethodBinder>();
        protected internal Dictionary<Symbol, iObject> Constants { get; } = new Dictionary<Symbol, iObject>();
        protected internal List<Module> Included { get; protected set; } = new List<Module>();
        protected internal List<Module> Prepended { get; protected set; } = new List<Module>();
        protected internal IList<WeakReference<Class>> Subclasses { get; } = new List<WeakReference<Class>>();

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

        private static void ValidateConstantName(string name)
        {
            if(!CONSTANT_NAME.IsMatch(name))
            {
                throw new NameError($"wrong constant name {name}");
            }
        }

        public bool IsConstDefined(String name, [Optional] bool inherit = true)
        {
            throw new System.NotImplementedException(nameof(IsConstDefined));
        }
    }
}
