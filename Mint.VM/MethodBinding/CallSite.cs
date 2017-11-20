using System;
using Mint.MethodBinding.Arguments;
using Mint.MethodBinding.Cache;
using Mint.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mint.MethodBinding
{
    public sealed class CallSite
    {
        private const string RB_STACK_KEY = "rb_stack";

        private Arity arity;
        
        public Visibility Visibility { get; }

        public Symbol MethodName { get; }

        public IList<ArgumentKind> ArgumentKinds { get; }

        public CallSiteCache CallCache { get; set; }

        public CallSite(Symbol methodName,
            Visibility visibility = Visibility.Public,
            IEnumerable<ArgumentKind> argumentKinds = null)
        {
            MethodName = methodName;
            Visibility = visibility;
            ArgumentKinds = System.Array.AsReadOnly(argumentKinds?.ToArray() ?? System.Array.Empty<ArgumentKind>());
            CallCache = new EmptyCallSiteCache(this);
        }

        public CallSite(Symbol methodName,
                        Visibility visibility = Visibility.Public,
                        params ArgumentKind[] argumentKinds)
            : this(methodName, visibility, (IEnumerable<ArgumentKind>) argumentKinds)
        { }

        public Arity Arity => arity ?? (arity = CalculateArity());
        
        public iObject Call(iObject instance, params iObject[] arguments)
        {
            if(arguments.Length != ArgumentKinds.Count)
            {
                throw new ArgumentException(
                    $"{MethodName}: Number of arguments ({arguments.Length}) doesn't match expected number ({ArgumentKinds.Count})."
                );
            }

            var bundle = new ArgumentBundle(ArgumentKinds.Zip(arguments, (kind, arg) => (kind, arg)));

            CallFrame.Push(this, instance, bundle);

            try
            {
                try
                {
                    return CallCache.Call();
                }
                catch(RecompilationRequiredException)
                {
                    // caught while a method was being redefined.
                    // give a second chance to recover.
                    return CallCache.Call();
                }
            }
            catch(Exception e)
            {
                if(!e.Data.Contains(RB_STACK_KEY))
                {
                    e.Data[RB_STACK_KEY] = CallFrame.Current;
                }

                throw;
            }
            catch(System.Exception e)
            {
                if(!e.Data.Contains(RB_STACK_KEY))
                {
                    e.Data[RB_STACK_KEY] = CallFrame.Current.CallSite.MethodName.Name;
                }

                throw;
            }
            finally
            {
                CallFrame.Pop();
            }
        }
        
        private Arity CalculateArity()
        {
            var min = ArgumentKinds.Count(a => a == ArgumentKind.Simple);
            if(ArgumentKinds.Any(a => a == ArgumentKind.Key || a == ArgumentKind.KeyRest))
            {
                min++;
            }
            var max = ArgumentKinds.Any(a => a == ArgumentKind.Rest) ? int.MaxValue : min;
            return new Arity(min, max);
        }
        
        public override string ToString()
        {
            var argumentKinds = string.Join(", ", ArgumentKinds.Select(_ => _.Description));
            return $"CallSite<\"{MethodName}\"<{Arity}>({argumentKinds})>";
        }
        
        public static class Reflection
        {
            public static readonly MethodInfo Call = Reflector<CallSite>.Method(
                _ => _.Call(default, default)
            );
        }
        
        public static class Expressions
        {
            public static MethodCallExpression Call(Expression callSite, Expression instance, Expression arguments)
                => Expression.Call(callSite, Reflection.Call, instance, arguments);
        }
    }
}
