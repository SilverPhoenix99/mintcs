using System;
using System.Linq.Expressions;
using Mint.MethodBinding;

namespace Mint
{
    class ClassBuilder<T>
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

        public ClassBuilder<T> DefProperty<TResult>(Symbol name, Expression<Func<TResult>> lambda)
        {
            Class.DefineMethod(new ClrPropertyBinder(name, Class, Reflector.Property(lambda)));
            return this;
        }

        public ClassBuilder<T> DefProperty<TResult>(string name, Expression<Func<TResult>> lambda) =>
            DefProperty(new Symbol(name), lambda);

        public ClassBuilder<T> DefProperty<TResult>(Symbol name, Expression<Func<T, TResult>> lambda)
        {
            Class.DefineMethod(new ClrPropertyBinder(name, Class, Reflector<T>.Property(lambda)));
            return this;
        }

        public ClassBuilder<T> DefProperty<TResult>(string name, Expression<Func<T, TResult>> lambda) =>
            DefProperty(new Symbol(name), lambda);

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

        public ClassBuilder<T> DefLambda(Symbol name, Function lambda, Range arity)
        {
            Class.DefineMethod(new DelegateMethodBinder(name, Class, lambda, arity));
            return this;
        }

        public ClassBuilder<T> DefLambda(string name, Function lambda, Range arity) =>
            DefLambda(new Symbol(name), lambda, arity);
            
        public static implicit operator Class(ClassBuilder<T> c) => c.Class;
    }
}
