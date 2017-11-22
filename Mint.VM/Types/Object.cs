using Mint.MethodBinding;
using Mint.MethodBinding.Arguments;
using Mint.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Collections.Generic;

namespace Mint
{
    public class Object : BaseObject
    {
        public Object(Class klass)
            : base(klass)
        { }


        public Object()
        { }


        public static iObject Box(iObject obj) => obj ?? new NilClass();
        public static iObject Box(string obj) => obj != null ? new String(obj) : (iObject) new NilClass();
        public static iObject Box(StringBuilder obj) => obj != null ? new String(obj) : (iObject) new NilClass();
        public static iObject Box(short obj) => new Fixnum(obj);
        public static iObject Box(int obj) => new Fixnum(obj);
        public static iObject Box(long obj) => new Fixnum(obj);
        public static iObject Box(float obj) => new Float(obj);
        public static iObject Box(double obj) => new Float(obj);
        public static iObject Box(IEnumerable<iObject> obj) => obj != null ? new Array(obj) : (iObject) new NilClass();
        public static iObject Box(bool obj) => obj ? new TrueClass() : (iObject) new FalseClass();


        public static iObject Box(object value)
        {
            switch(value)
            {
                case null: return new NilClass();
                case iObject val: return val;
                case string val: return Box(val);
                case StringBuilder val: return Box(val);
                case bool val: return Box(val);
                case short val: return Box(val);
                case int val: return Box(val);
                case long val: return Box(val);
                case float val: return Box(val);
                case double val: return Box(val);
                case IEnumerable<iObject> val: return Box(val);
                case IEnumerable<Symbol> val: return Box(val.Cast<iObject>());
                case IEnumerable<Fixnum> val: return Box(val.Cast<iObject>());
            }

            throw new ArgumentError(nameof(value));
        }

        //public static bool ToBool(object obj) => obj is iObject instance ? ToBool(instance) : true.Equals(obj);

        public static bool ToBool(iObject obj) => !(obj == null || obj is NilClass || obj is FalseClass);

        internal static string MethodMissingInspect(iObject obj) => $"{obj.Inspect()}:{obj.Class.Name}";

        public static iObject Send(iObject instance, iObject methodName, params iObject[] arguments)
        {
            var methodNameAsSymbol = MethodNameAsSymbol(methodName);
            var argumentKinds = Enumerable.Range(0, arguments.Length).Select(_ => ArgumentKind.Simple);
            var callSite = new CallSite(methodNameAsSymbol, Visibility.Private, argumentKinds);
            return callSite.Call(instance, arguments);
        }
        
        private static Symbol MethodNameAsSymbol(iObject methodName)
        {
            switch(methodName)
            {
                case Symbol symName:
                    return symName;
                case String name:
                    return new Symbol(name.Value);
                default:
                    throw new TypeError($"{methodName.Inspect()} is not a symbol nor a string");
            }
        }

        internal static void ValidateInstanceVariableName(string name)
        {
            if(IVAR.IsMatch(name))
            {
                return;
            }

            throw new NameError($"`{name}' is not allowed as an instance variable name");
        }
        
        public static bool RespondTo(iObject instance, Symbol methodName) 
            => instance.EffectiveClass.Methods.ContainsKey(methodName);


        public static class Reflection
        {
            public static readonly PropertyInfo Id = Reflector<iObject>.Property(_ => _.Id);

            public static readonly PropertyInfo EffectiveClass = Reflector<iObject>.Property(_ => _.EffectiveClass);
            
            public static readonly MethodInfo Box = Reflector.Method(
                () => Box(default(object))
            );

            public static readonly MethodInfo InstanceVariableGet = Reflector<iObject>.Method(
                _ => _.InstanceVariableGet(default(Symbol))
            );

            public static readonly MethodInfo InstanceVariableSet = Reflector<iObject>.Method(
                _ => _.InstanceVariableSet(default(Symbol), default(iObject))
            );

            public static readonly PropertyInfo SingletonClass = Reflector<iObject>.Property(_ => _.SingletonClass);
        }
        
        public static class Expressions
        {
            public static MethodCallExpression Box(Expression value)
            {
                value = value.StripConversions();
                var method = typeof(Object).GetMethod(nameof(Box), new[] { value.Type }) ?? Reflection.Box;

                var parameterType = method.GetParameters()[0].ParameterType;
                if(value.Type != parameterType)
                {
                    value = value.Cast(parameterType);
                }
                
                return Expression.Call(method, value);
            }
            
            public static MethodCallExpression InstanceVariableGet(Expression instance, Expression variableName)
                => Expression.Call(instance, Reflection.InstanceVariableGet, variableName);
            
            public static MethodCallExpression InstanceVariableSet(Expression instance,
                                                                   Expression variableName,
                                                                   Expression value)
                => Expression.Call(instance, Reflection.InstanceVariableSet, variableName, value);
            
            public static MemberExpression SingletonClass(Expression obj)
                => Expression.Property(obj, Reflection.SingletonClass);
        }
    }
}
