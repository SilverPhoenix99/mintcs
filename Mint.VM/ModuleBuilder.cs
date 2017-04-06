using Mint.MethodBinding.Methods;
using Mint.Reflection;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Mint
{
    internal class ModuleBuilder<T>
        where T : iObject
    {
        public ModuleBuilder(Module mod)
        {
            Module = mod;
        }

        public Module Module { get; }

        public Class Class => (Class) Module;

        #region DescribeModule

        public static ModuleBuilder<T> DescribeModule(string name, Module container = null) =>
            new ModuleBuilder<T>(new Module(new Symbol(name ?? typeof(T).Name), container));

        public static ModuleBuilder<T> DescribeModule(Module container = null) =>
            DescribeModule(typeof(T).Name, container);

        #endregion

        #region DescribeClass

        public static ModuleBuilder<T> DescribeClass(Class superclass, string name, Module container = null, bool isSingleton = false) =>
            new ModuleBuilder<T>(new Class(superclass, new Symbol(name), container, isSingleton));

        public static ModuleBuilder<T> DescribeClass(Class superclass, Module container = null, bool isSingleton = false) =>
            DescribeClass(superclass, typeof(T).Name, container, isSingleton);

        public static ModuleBuilder<T> DescribeClass(string name, Module container = null, bool isSingleton = false) =>
            new ModuleBuilder<T>(new Class(new Symbol(name ?? typeof(T).Name), container, isSingleton));

        public static ModuleBuilder<T> DescribeClass(Module container = null, bool isSingleton = false) =>
            DescribeClass(typeof(T).Name, container, isSingleton);

        #endregion

        public ModuleBuilder<T> Allocator(Func<iObject> allocator)
        {
            Class.Allocator = allocator;
            return this;
        }

        #region DefMethod

        public ModuleBuilder<T> DefMethod(Symbol name, MethodInfo method)
        {
            Module.DefineMethod(new ClrMethodBinder(name, Module, new MethodMetadata(method)));
            return this;
        }

        public ModuleBuilder<T> DefMethod<TResult>(string name, Expression<Func<TResult>> lambda) =>
            DefMethod(name, (LambdaExpression) lambda);

        public ModuleBuilder<T> DefMethod<TResult>(string name, Expression<Func<T, TResult>> lambda) =>
            DefMethod(name, (LambdaExpression) lambda);

        public ModuleBuilder<T> DefMethod(string name, LambdaExpression lambda) =>
            DefMethod(name, Reflector.Method(lambda));

        public ModuleBuilder<T> DefMethod(string name, MethodInfo method) =>
            DefMethod(new Symbol(name), method);

        public ModuleBuilder<T> Alias(string newName, string oldName)
        {
            var name = new Symbol(oldName);
            if(!Module.Methods.TryGetValue(name, out var binder))
            {
                throw new NameError($"undefined method `{oldName}' for class `{Module.Name}'");
            }

            name = new Symbol(newName);
            Module.DefineMethod(binder.Duplicate(name));
            return this;
        }

        #endregion

        #region DefLambda

        public ModuleBuilder<T> DefLambda(string name, Delegate lambda) => DefLambda(new Symbol(name), lambda);

        private ModuleBuilder<T> DefLambda(Symbol name, Delegate lambda)
        {
            var method = new MethodMetadata(lambda.Method, name.Name, hasInstance: true);
            var delegateMetadata = new DelegateMetadata(lambda, method);
            Module.DefineMethod(new DelegateMethodBinder(name, Module, delegateMetadata));
            return this;
        }

        #endregion

        #region AttrReader

        public ModuleBuilder<T> AttrReader<TResult>(string name, Expression<Func<TResult>> lambda) =>
            AttrReader(name, (LambdaExpression) lambda);

        public ModuleBuilder<T> AttrReader<TResult>(string name, Expression<Func<T, TResult>> lambda) =>
            AttrReader(name, (LambdaExpression) lambda);

        private ModuleBuilder<T> AttrReader(string name, LambdaExpression lambda)
        {
            if(name.EndsWith("=") || name.EndsWith("?") || name.EndsWith("!"))
            {
                throw new NameError($"invalid attribute name `{name}'");
            }
            return DefMethod(name, Reflector.Getter(lambda));
        }

        #endregion

        #region AttrWriter

        public ModuleBuilder<T> AttrWriter<TResult>(string name, Expression<Func<TResult>> lambda) =>
            AttrWriter(name, (LambdaExpression) lambda);

        public ModuleBuilder<T> AttrWriter<TResult>(string name, Expression<Func<T, TResult>> lambda) =>
            AttrWriter(name, (LambdaExpression) lambda);

        private ModuleBuilder<T> AttrWriter(string name, LambdaExpression lambda)
        {
            if(name.EndsWith("=") || name.EndsWith("?") || name.EndsWith("!"))
            {
                throw new NameError($"invalid attribute name `{name}'");
            }
            return DefMethod(name + "=", Reflector.Setter(lambda));
        }

        #endregion

        #region AttrAccessor

        public ModuleBuilder<T> AttrAccessor<TResult>(string name, Expression<Func<T, TResult>> lambda) =>
            AttrAccessor(name, (LambdaExpression) lambda);

        public ModuleBuilder<T> AttrAccessor<TResult>(string name, Expression<Func<TResult>> lambda) =>
            AttrAccessor(name, (LambdaExpression) lambda);

        private ModuleBuilder<T> AttrAccessor(string name, LambdaExpression lambda)
        {
            AttrReader(name, lambda);
            return AttrWriter(name, lambda);
        }

        #endregion

        public static implicit operator Module(ModuleBuilder<T> m) => m.Module;

        public static implicit operator Class(ModuleBuilder<T> m) => m.Class;
    }
}
