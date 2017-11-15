using Mint.MethodBinding.Methods;
using Mint.Reflection;
using System;
using System.Diagnostics;
using System.Linq;
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


        #region Allocator


        public ModuleBuilder<T> Allocator(Func<iObject> allocator)
        {
            Class.Allocator = allocator;
            return this;
        }


        public ModuleBuilder<T> GenerateAllocator()
        {
            var type = typeof(T);

            var ctor = type.GetConstructors().FirstOrDefault(c =>
                c.GetParameters().Length == 0
                || c.GetParameters()[0].HasDefaultValue);

            Debug.Assert(Module is Class);
            Debug.Assert(!type.IsAbstract);
            Debug.Assert(!type.IsInterface);
            Debug.Assert(ctor != null);

            // () => new T()
            var args = ctor.GetParameters().Select(p => Expression.Default(p.ParameterType));
            var lambda = Expression.Lambda<Func<iObject>>(Expression.New(ctor, args).Cast<iObject>());
            var @delegate = lambda.Compile();

            return Allocator(@delegate);
        }


        #endregion


        #region DefMethod

        public ModuleBuilder<T> AutoDefineMethods()
        {
            var type = typeof(T);

            var methods =
                from m in type.GetMethods()
                from a in m.GetCustomAttributes<RubyMethodAttribute>()
                select new { Method = m, Attribute = a }
            ;

            var getters =
                from p in type.GetProperties()
                where p.CanRead
                from a in p.GetCustomAttributes<RubyMethodAttribute>()
                select new { Method = p.GetGetMethod(), Attribute = a }
            ;

            var setters =
                from p in type.GetProperties()
                where p.CanWrite
                from a in p.GetCustomAttributes<RubyMethodAttribute>()
                select new
                {
                    Method = p.GetSetMethod(),
                    Attribute = new RubyMethodAttribute($"{a.MethodName}=")
                    {
                        Visibility = a.Visibility
                    }
                }
            ;

            var methodBinders =
                from namedMethod in methods.Concat(getters).Concat(setters)
                group new MethodMetadata(namedMethod.Method)
                by namedMethod.Attribute into metadatas
                select new ClrMethodBinder(
                    new Symbol(metadatas.Key.MethodName),
                    Module,
                    metadatas,
                    null,
                    metadatas.Key.Visibility
                )
            ;

            foreach(var methodBinder in methodBinders)
            {
                Module.Methods.Add(methodBinder.Name, methodBinder);
            }

            return this;
        }


        public ModuleBuilder<T> DefMethod(Symbol name, MethodInfo method)
        {
            Module.DefineMethod(new ClrMethodBinder(name, Module, new[] { new MethodMetadata(method) }));
            return this;
        }


        public ModuleBuilder<T> DefMethod<TResult>(string name, Expression<Func<TResult>> lambda)
            => DefMethod(name, (LambdaExpression) lambda);


        public ModuleBuilder<T> DefMethod<TResult>(string name, Expression<Func<T, TResult>> lambda)
            => DefMethod(name, (LambdaExpression) lambda);


        public ModuleBuilder<T> DefMethod(string name, LambdaExpression lambda)
            => DefMethod(name, Reflector.Method(lambda));


        public ModuleBuilder<T> DefMethod(string name, MethodInfo method)
            => DefMethod(new Symbol(name), method);


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


        public ModuleBuilder<T> DefLambda(string name, Delegate lambda)
            => DefLambda(new Symbol(name), lambda);


        private ModuleBuilder<T> DefLambda(Symbol name, Delegate lambda)
        {
            var method = new MethodMetadata(lambda.Method, name.Name, hasInstance: true);
            var delegateMetadata = new DelegateMetadata(lambda, method);
            Module.DefineMethod(new DelegateMethodBinder(name, Module, delegateMetadata));
            return this;
        }


        #endregion


        #region AttrReader


        public ModuleBuilder<T> AttrReader<TResult>(string name, Expression<Func<TResult>> lambda)
            => AttrReader(name, (LambdaExpression) lambda);


        public ModuleBuilder<T> AttrReader<TResult>(string name, Expression<Func<T, TResult>> lambda)
            => AttrReader(name, (LambdaExpression) lambda);


        private ModuleBuilder<T> AttrReader(string name, LambdaExpression lambda)
        {
            if("=?!".Contains(name[name.Length - 1]))
            {
                throw new NameError($"invalid attribute name `{name}'");
            }
            return DefMethod(name, Reflector.Getter(lambda));
        }


        #endregion


        #region AttrWriter


        public ModuleBuilder<T> AttrWriter<TResult>(string name, Expression<Func<TResult>> lambda)
            => AttrWriter(name, (LambdaExpression) lambda);


        public ModuleBuilder<T> AttrWriter<TResult>(string name, Expression<Func<T, TResult>> lambda)
            => AttrWriter(name, (LambdaExpression) lambda);


        private ModuleBuilder<T> AttrWriter(string name, LambdaExpression lambda)
        {
            if("=?!".Contains(name[name.Length - 1]))
            {
                throw new NameError($"invalid attribute name `{name}'");
            }
            return DefMethod(name + "=", Reflector.Setter(lambda));
        }


        #endregion


        #region AttrAccessor


        public ModuleBuilder<T> AttrAccessor<TResult>(string name, Expression<Func<T, TResult>> lambda)
            => AttrAccessor(name, (LambdaExpression) lambda);


        public ModuleBuilder<T> AttrAccessor<TResult>(string name, Expression<Func<TResult>> lambda)
            => AttrAccessor(name, (LambdaExpression) lambda);


        private ModuleBuilder<T> AttrAccessor(string name, LambdaExpression lambda)
        {
            AttrReader(name, lambda);
            return AttrWriter(name, lambda);
        }


        #endregion


        #region DescribeModule


        public static ModuleBuilder<T> DescribeModule(string name, Module container = null)
            => new ModuleBuilder<T>(new Module(new Symbol(name ?? typeof(T).Name), container));


        public static ModuleBuilder<T> DescribeModule(Module container = null)
            => DescribeModule(typeof(T).Name, container);


        #endregion


        #region DescribeClass


        public static ModuleBuilder<T> DescribeClass(Class superclass,
                                                     string name = null,
                                                     Module container = null,
                                                     bool isSingleton = false)
            => new ModuleBuilder<T>(new Class(superclass, new Symbol(name ?? typeof(T).Name), container, isSingleton));


        public static ModuleBuilder<T> DescribeClass(string name = null,
                                                     Module container = null,
                                                     bool isSingleton = false)
            => new ModuleBuilder<T>(new Class(new Symbol(name ?? typeof(T).Name), container, isSingleton));


        #endregion


        public static implicit operator Module(ModuleBuilder<T> m)
            => m.Module;


        public static implicit operator Class(ModuleBuilder<T> m)
            => m.Class;
    }
}
