using System;
using System.Linq.Expressions;
using System.Reflection;
using Mint.MethodBinding;

namespace Mint
{
    class ModuleBuilder<T>
        where T : iObject
    {
        protected ModuleBuilder(Module mod)
        {
            Module = mod;
        }

        public Module Module { get; }

        public static ModuleBuilder<T> Describe(string name, Module container = null)
        {
            var nameSymbol = new Symbol(name ?? typeof(T).Name);
            return new ModuleBuilder<T>(new Module(nameSymbol, container));
        }

        public static ModuleBuilder<T> Describe(Module container = null)
        {
            return Describe(typeof(T).Name, container);
        }

        public ModuleBuilder<T> Set(Action<ModuleBuilder<T>> action)
        {
            action(this);
            return this;
        }

        public ModuleBuilder<T> DefMethod<TResult>(Symbol name, Expression<Func<TResult>> lambda)
        {
            Module.DefineMethod(new ClrMethodBinder(name, Module, Reflector.Method(lambda)));
            return this;
        }

        public ModuleBuilder<T> DefMethod<TResult>(string name, Expression<Func<TResult>> lambda) =>
            DefMethod(new Symbol(name), lambda);

        public ModuleBuilder<T> DefMethod<TResult>(Symbol name, Expression<Func<T, TResult>> lambda)
        {
            Module.DefineMethod(new ClrMethodBinder(name, Module, Reflector<T>.Method(lambda)));
            return this;
        }

        public ModuleBuilder<T> DefMethod<TResult>(string name, Expression<Func<T, TResult>> lambda) =>
            DefMethod(new Symbol(name), lambda);

        public ModuleBuilder<T> DefMethod(Symbol name, MethodInfo method)
        {
            Module.DefineMethod(new ClrMethodBinder(name, Module, method));
            return this;
        }

        public ModuleBuilder<T> DefMethod(string name, MethodInfo method) =>
            DefMethod(new Symbol(name), method);

        public ModuleBuilder<T> DefOperator<TResult>(Symbol name, Expression<Func<TResult>> lambda)
        {
            Module.DefineMethod(new ClrMethodBinder(name, Module, Reflector.Operator(lambda)));
            return this;
        }

        public ModuleBuilder<T> DefOperator<TResult>(string name, Expression<Func<TResult>> lambda) =>
            DefOperator(new Symbol(name), lambda);

        public ModuleBuilder<T> DefOperator<TResult>(Symbol name, Expression<Func<T, TResult>> lambda)
        {
            Module.DefineMethod(new ClrMethodBinder(name, Module, Reflector<T>.Operator(lambda)));
            return this;
        }

        public ModuleBuilder<T> DefOperator<TResult>(string name, Expression<Func<T, TResult>> lambda) =>
            DefOperator(new Symbol(name), lambda);

        public ModuleBuilder<T> DefLambda(Symbol name, Delegate lambda)
        {
            Module.DefineMethod(new DelegateMethodBinder(name, Module, lambda));
            return this;
        }

        public ModuleBuilder<T> DefLambda(string name, Delegate lambda) => DefLambda(new Symbol(name), lambda);

        public ModuleBuilder<T> AttrReader<TResult>(Symbol name, Expression<Func<TResult>> lambda)
        {
            if(name.Name.EndsWith("=") || name.Name.EndsWith("?") || name.Name.EndsWith("!"))
            {
                throw new NameError($"invalid attribute name `{name}'");
            }
            Module.DefineMethod(new ClrMethodBinder(name, Module, Reflector.Getter(lambda)));
            return this;
        }

        public ModuleBuilder<T> AttrReader<TResult>(string name, Expression<Func<TResult>> lambda) =>
            AttrReader(new Symbol(name), lambda);

        public ModuleBuilder<T> AttrReader<TResult>(Symbol name, Expression<Func<T, TResult>> lambda)
        {
            if(name.Name.EndsWith("=") || name.Name.EndsWith("?") || name.Name.EndsWith("!"))
            {
                throw new NameError($"invalid attribute name `{name}'");
            }
            Module.DefineMethod(new ClrMethodBinder(name, Module, Reflector<T>.Getter(lambda)));
            return this;
        }

        public ModuleBuilder<T> AttrReader<TResult>(string name, Expression<Func<T, TResult>> lambda) =>
            AttrReader(new Symbol(name), lambda);

        public ModuleBuilder<T> AttrWriter<TResult>(Symbol name, Expression<Func<TResult>> lambda)
        {
            if(name.Name.EndsWith("=") || name.Name.EndsWith("?") || name.Name.EndsWith("!"))
            {
                throw new NameError($"invalid attribute name `{name}'");
            }
            name = new Symbol(name.Name + "=");
            Module.DefineMethod(new ClrMethodBinder(name, Module, Reflector.Setter(lambda)));
            return this;
        }

        public ModuleBuilder<T> AttrWriter<TResult>(string name, Expression<Func<TResult>> lambda) =>
            AttrWriter(new Symbol(name), lambda);

        public ModuleBuilder<T> AttrWriter<TResult>(Symbol name, Expression<Func<T, TResult>> lambda)
        {
            if(name.Name.EndsWith("=") || name.Name.EndsWith("?") || name.Name.EndsWith("!"))
            {
                throw new NameError($"invalid attribute name `{name}'");
            }
            name = new Symbol(name.Name + "=");
            Module.DefineMethod(new ClrMethodBinder(name, Module, Reflector<T>.Setter(lambda)));
            return this;
        }

        public ModuleBuilder<T> AttrWriter<TResult>(string name, Expression<Func<T, TResult>> lambda) =>
            AttrWriter(new Symbol(name), lambda);

        public ModuleBuilder<T> AttrAccessor<TResult>(Symbol name, Expression<Func<TResult>> lambda)
        {
            AttrReader(name, lambda);
            return AttrWriter(name, lambda);
        }

        public ModuleBuilder<T> AttrAccessor<TResult>(string name, Expression<Func<TResult>> lambda) =>
            AttrAccessor(new Symbol(name), lambda);

        public ModuleBuilder<T> AttrAccessor<TResult>(Symbol name, Expression<Func<T, TResult>> lambda)
        {
            AttrReader(name, lambda);
            return AttrWriter(name, lambda);
        }

        public ModuleBuilder<T> AttrAccessor<TResult>(string name, Expression<Func<T, TResult>> lambda) =>
            AttrAccessor(new Symbol(name), lambda);

        public static implicit operator Module(ModuleBuilder<T> m) => m.Module;
    }
}
