using System;
using System.Collections.Generic;

namespace Mint
{
    public class Module : Object
    {
        protected Module(Class klass, Symbol? name = null, Module container = null)
            : base(klass)
        {
            Name = name;
            Container = container;

            FullName = name != null
                     ? name.ToString()
                     : string.Format("#<{0}:0x{1:x}>", Class.Name, Id);

            if(container != null)
            {
                FullName = string.Concat(container.FullName, "::", FullName);
            }
        }

        public Module(Symbol? name = null, Module container = null)
            : this(CLASS, name, container)
        { }

        public Symbol?                      Name      { get; }
        public string                       FullName  { get; }
        public Module                       Container { get; }
        public LinkedList<Module>           Included  { get; } = new LinkedList<Module>();
        public IDictionary<Symbol, Method>  Methods   { get; } = new Dictionary<Symbol, Method>();
        public IDictionary<Symbol, iObject> Constants { get; } = new Dictionary<Symbol, iObject>();
        public virtual bool                 IsModule  => true;

        public override string ToString() => FullName;

        public Symbol DefineMethod(Method method) => ( Methods[method.Name] = method ).Name;

        public Symbol DefineMethod(Symbol name, Delegate function) => DefineMethod(new Method(name, this, function));

        public void Include(Module mod)
        {
            throw new NotImplementedException();

            if(!Included.Contains(mod))
            {
                Included.AddFirst(mod);
            }
        }

        public void Prepend(Module mod)
        {
            throw new NotImplementedException();

            if(!Included.Contains(mod))
            {
                Included.AddLast(mod);
            }
        }

        public override Method FindMethod(Symbol name)
        {
            // Method resolution: See Object#FindMethod

            throw new NotImplementedException();
        }

        #region Static

        public static new Class CLASS = new Class(new Symbol("Module"));

        #endregion
    }
}
