using Mint.MethodBinding;
using Mint.MethodBinding.Arguments;
using Mint.MethodBinding.Compilation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Mint.Reflection;

namespace Mint
{
    public class Object : BaseObject
    {
        public static iObject Box(iObject obj) => obj ?? new NilClass();
        public static iObject Box(string obj) => new String(obj);
        public static iObject Box(StringBuilder obj) => new String(obj);
        public static iObject Box(short obj) => new Fixnum(obj);
        public static iObject Box(int obj) => new Fixnum(obj);
        public static iObject Box(long obj) => new Fixnum(obj);
        public static iObject Box(float obj) => new Float(obj);
        public static iObject Box(double obj) => new Float(obj);

        public static iObject Box(bool obj) => obj ? new TrueClass() : (iObject) new FalseClass();

        public static iObject Box(object value)
        {
            if(value == null) return new NilClass();
            if(value is iObject) return (iObject) value;
            if(value is string) return Box((string) value);
            if(value is StringBuilder) return Box((StringBuilder) value);
            if(value is bool) return Box((bool) value);
            if(value is short) return Box((short) value);
            if(value is int) return Box((int) value);
            if(value is long) return Box((long) value);
            if(value is float) return Box((float) value);
            if(value is double) return Box((double) value);

            throw new ArgumentError(nameof(value));
        }

        public static bool ToBool(iObject obj) => obj != null && !(obj is NilClass) && !(obj is FalseClass);

        public static Module DefineModule(Module module)
        {
            if(module.Name.HasValue)
            {
                Class.OBJECT.Constants[module.Name.Value] = module;
            }
            return module;
        }

        internal static string MethodMissingInspect(iObject obj) => $"{obj.Inspect()}:{obj.Class.FullName}";

        internal static iObject Send(iObject instance, iObject methodName, params iObject[] arguments)
        {
            var methodNameAsSymbol = MethodNameAsSymbol(methodName);
            var argumentKinds = Enumerable.Range(0, arguments.Length).Select(_ => ArgumentKind.Simple);
            var callSite = new CallSite(methodNameAsSymbol, Visibility.Private, argumentKinds);
            callSite.CallCompiler = new MonomorphicCallCompiler(callSite);
            return callSite.Call(instance, arguments);
        }

        private static Symbol MethodNameAsSymbol(iObject methodName)
        {
            if(methodName is Symbol)
            {
                return (Symbol) methodName;
            }

            var name = methodName as String;
            if(name != null)
            {
                return new Symbol(name.Value);
            }

            throw new TypeError($"{methodName.Inspect()} is not a symbol nor a string");
        }

        internal static void ValidateInstanceVariableName(string name)
        {
            if(IVAR.IsMatch(name))
            {
                return;
            }

            throw new NameError($"`{name}' is not allowed as an instance variable name");
        }

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
            
            public static MethodCallExpression InstanceVariableGet(Expression instance, Expression variableName) =>
                Expression.Call(instance, Reflection.InstanceVariableGet, variableName);

            public static MethodCallExpression InstanceVariableSet(Expression instance,
                                                                   Expression variableName,
                                                                   Expression value) =>
                Expression.Call(instance, Reflection.InstanceVariableSet, variableName, value);
        }
    }
}
