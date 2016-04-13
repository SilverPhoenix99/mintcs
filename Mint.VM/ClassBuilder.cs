using System;
using System.Linq.Expressions;
using System.Reflection;
using Mint.MethodBinding;

namespace Mint
{
    class ClassBuilder<T>
        where T : iObject
    {
        protected ClassBuilder(Class klass)
        {
            Class = klass;
        }

        public Class Class { get; }

        public static ClassBuilder<T> Describe(Class superclass, string name, Module container = null, bool isSingleton = false)
        {
            var nameSymbol = new Symbol(name);
            return new ClassBuilder<T>(new Class(superclass, nameSymbol, container, isSingleton));
        }

        public static ClassBuilder<T> Describe(Class superclass, Module container = null, bool isSingleton = false)
        {
            return Describe(superclass, typeof(T).Name, container, isSingleton);
        }

        public static ClassBuilder<T> Describe(string name, Module container = null, bool isSingleton = false)
        {
            var nameSymbol = new Symbol(name ?? typeof(T).Name);
            return new ClassBuilder<T>(new Class(nameSymbol, container, isSingleton));
        }

        public static ClassBuilder<T> Describe(Module container = null, bool isSingleton = false)
        {
            return Describe(typeof(T).Name, container, isSingleton);
        }

        public ClassBuilder<T> Set(Action<ClassBuilder<T>> action)
        {
            action(this);
            return this;
        }

        public ClassBuilder<T> DefMethod<TResult>(Symbol name, Expression<Func<TResult>> lambda)
        {
            Class.DefineMethod(new ClrMethodBinder(name, Class, Reflector.Method(lambda)));
            return this;
        }

        public ClassBuilder<T> DefMethod<TResult>(string name, Expression<Func<TResult>> lambda) =>
            DefMethod(new Symbol(name), lambda);

        public ClassBuilder<T> DefMethod<TResult>(Symbol name, Expression<Func<T, TResult>> lambda)
        {
            Class.DefineMethod(new ClrMethodBinder(name, Class, Reflector<T>.Method(lambda)));
            return this;
        }

        public ClassBuilder<T> DefMethod<TResult>(string name, Expression<Func<T, TResult>> lambda) =>
            DefMethod(new Symbol(name), lambda);

        public ClassBuilder<T> DefMethod(Symbol name, MethodInfo method)
        {
            Class.DefineMethod(new ClrMethodBinder(name, Class, method));
            return this;
        }

        public ClassBuilder<T> DefMethod(string name, MethodInfo method) =>
            DefMethod(new Symbol(name), method);

        public ClassBuilder<T> DefOperator<TResult>(Symbol name, Expression<Func<TResult>> lambda)
        {
            Class.DefineMethod(new ClrMethodBinder(name, Class, Reflector.Operator(lambda)));
            return this;
        }

        public ClassBuilder<T> DefOperator<TResult>(string name, Expression<Func<TResult>> lambda) =>
            DefOperator(new Symbol(name), lambda);

        public ClassBuilder<T> DefOperator<TResult>(Symbol name, Expression<Func<T, TResult>> lambda)
        {
            Class.DefineMethod(new ClrMethodBinder(name, Class, Reflector<T>.Operator(lambda)));
            return this;
        }

        public ClassBuilder<T> DefOperator<TResult>(string name, Expression<Func<T, TResult>> lambda) =>
            DefOperator(new Symbol(name), lambda);

        public ClassBuilder<T> DefLambda(Symbol name, Delegate lambda)
        {
            Class.DefineMethod(new DelegateMethodBinder(name, Class, lambda));
            return this;
        }

        public ClassBuilder<T> DefLambda(string name, Delegate lambda) => DefLambda(new Symbol(name), lambda);

        public ClassBuilder<T> AttrReader<TResult>(Symbol name, Expression<Func<TResult>> lambda)
        {
            if(name.Name.EndsWith("=") || name.Name.EndsWith("?") || name.Name.EndsWith("!"))
            {
                throw new NameError($"invalid attribute name `{name}'");
            }
            Class.DefineMethod(new ClrMethodBinder(name, Class, Reflector.Getter(lambda)));
            return this;
        }

        public ClassBuilder<T> AttrReader<TResult>(string name, Expression<Func<TResult>> lambda) =>
            AttrReader(new Symbol(name), lambda);

        public ClassBuilder<T> AttrReader<TResult>(Symbol name, Expression<Func<T, TResult>> lambda)
        {
            if(name.Name.EndsWith("=") || name.Name.EndsWith("?") || name.Name.EndsWith("!"))
            {
                throw new NameError($"invalid attribute name `{name}'");
            }
            Class.DefineMethod(new ClrMethodBinder(name, Class, Reflector<T>.Getter(lambda)));
            return this;
        }

        public ClassBuilder<T> AttrReader<TResult>(string name, Expression<Func<T, TResult>> lambda) =>
            AttrReader(new Symbol(name), lambda);

        public ClassBuilder<T> AttrWriter<TResult>(Symbol name, Expression<Func<TResult>> lambda)
        {
            if(name.Name.EndsWith("=") || name.Name.EndsWith("?") || name.Name.EndsWith("!"))
            {
                throw new NameError($"invalid attribute name `{name}'");
            }
            name = new Symbol(name.Name + "=");
            Class.DefineMethod(new ClrMethodBinder(name, Class, Reflector.Setter(lambda)));
            return this;
        }

        public ClassBuilder<T> AttrWriter<TResult>(string name, Expression<Func<TResult>> lambda) =>
            AttrWriter(new Symbol(name), lambda);

        public ClassBuilder<T> AttrWriter<TResult>(Symbol name, Expression<Func<T, TResult>> lambda)
        {
            if(name.Name.EndsWith("=") || name.Name.EndsWith("?") || name.Name.EndsWith("!"))
            {
                throw new NameError($"invalid attribute name `{name}'");
            }
            name = new Symbol(name.Name + "=");
            Class.DefineMethod(new ClrMethodBinder(name, Class, Reflector<T>.Setter(lambda)));
            return this;
        }

        public ClassBuilder<T> AttrWriter<TResult>(string name, Expression<Func<T, TResult>> lambda) =>
            AttrWriter(new Symbol(name), lambda);

        public ClassBuilder<T> AttrAccessor<TResult>(Symbol name, Expression<Func<TResult>> lambda)
        {
            AttrReader(name, lambda);
            return AttrWriter(name, lambda);
        }

        public ClassBuilder<T> AttrAccessor<TResult>(string name, Expression<Func<TResult>> lambda) =>
            AttrAccessor(new Symbol(name), lambda);

        public ClassBuilder<T> AttrAccessor<TResult>(Symbol name, Expression<Func<T, TResult>> lambda)
        {
            AttrReader(name, lambda);
            return AttrWriter(name, lambda);
        }

        public ClassBuilder<T> AttrAccessor<TResult>(string name, Expression<Func<T, TResult>> lambda) =>
            AttrAccessor(new Symbol(name), lambda);

        public static implicit operator Class(ClassBuilder<T> c) => c.Class;
    }
}
