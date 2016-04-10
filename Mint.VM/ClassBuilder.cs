using System;
using System.Linq.Expressions;

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
            Class.DefineMethod(new CompiledMethod(name, Class, Reflector.Method(lambda)));
            return this;
        }

        public ClassBuilder<T> DefMethod<TResult>(string name, Expression<Func<TResult>> lambda) =>
            DefMethod(new Symbol(name), lambda);

        public ClassBuilder<T> DefMethod<TResult>(Symbol name, Expression<Func<T, TResult>> lambda)
        {
            Class.DefineMethod(new CompiledMethod(name, Class, Reflector<T>.Method(lambda)));
            return this;
        }

        public ClassBuilder<T> DefMethod<TResult>(string name, Expression<Func<T, TResult>> lambda) =>
            DefMethod(new Symbol(name), lambda);

        public ClassBuilder<T> DefProperty<TResult>(Symbol name, Expression<Func<TResult>> lambda)
        {
            Class.DefineMethod(new CompiledProperty(name, Class, Reflector.Property(lambda)));
            return this;
        }

        public ClassBuilder<T> DefProperty<TResult>(string name, Expression<Func<TResult>> lambda) =>
            DefProperty(new Symbol(name), lambda);

        public ClassBuilder<T> DefProperty<TResult>(Symbol name, Expression<Func<T, TResult>> lambda)
        {
            Class.DefineMethod(new CompiledProperty(name, Class, Reflector<T>.Property(lambda)));
            return this;
        }

        public ClassBuilder<T> DefProperty<TResult>(string name, Expression<Func<T, TResult>> lambda) =>
            DefProperty(new Symbol(name), lambda);

        public ClassBuilder<T> DefOperator<TResult>(Symbol name, Expression<Func<TResult>> lambda)
        {
            Class.DefineMethod(new CompiledMethod(name, Class, Reflector.Operator(lambda)));
            return this;
        }

        public ClassBuilder<T> DefOperator<TResult>(string name, Expression<Func<TResult>> lambda) =>
            DefOperator(new Symbol(name), lambda);
        
        public ClassBuilder<T> DefOperator<TResult>(Symbol name, Expression<Func<T, TResult>> lambda)
        {
            Class.DefineMethod(new CompiledMethod(name, Class, Reflector<T>.Operator(lambda)));
            return this;
        }

        public ClassBuilder<T> DefOperator<TResult>(string name, Expression<Func<T, TResult>> lambda) =>
            DefOperator(new Symbol(name), lambda);

        public ClassBuilder<T> DefLambda(Symbol name, Method.Delegate lambda)
        {
            Class.DefineMethod(new LambdaMethod(name, Class, lambda));
            return this;
        }

        public ClassBuilder<T> DefLambda(string name, Method.Delegate lambda) =>
            DefLambda(new Symbol(name), lambda);
    }
}
