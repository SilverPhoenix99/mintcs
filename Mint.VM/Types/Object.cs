using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.MethodBinding;

namespace Mint
{
    public partial class Object : BaseObject
    {
        public Object() { }

        public Object(Class klass = null) : base(klass) { }

        #region Static

        public static iObject Box(string obj) => new String(obj);
        public static iObject Box(short obj)  => new Fixnum(obj);
        public static iObject Box(int obj)    => new Fixnum(obj);
        public static iObject Box(long obj)   => new Fixnum(obj);
        public static iObject Box(float obj)  => new Float(obj);
        public static iObject Box(double obj) => new Float(obj);

        public static iObject Box(bool obj) => obj ? new TrueClass() : (iObject) new FalseClass();

        public static iObject Box(object value)
        {
            if(value is iObject) return (iObject) value;
            if(value is string) return Box((string) value);
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

        internal static iObject Send(iObject obj, iObject name, params iObject[] args)
        {
            Symbol methodName;
            if(name is Symbol)
            {
                methodName = (Symbol) name;
            }
            else if(name is String)
            {
                methodName = new Symbol(((String) name).Value);
            }
            else
            {
                throw new TypeError($"{obj.Inspect()} is not a symbol nor a string");
            }

            //var method = obj.Class.FindMethod(methodName);
            //return (iObject) method.Invoke(args);

            var info = obj.GetType().GetMethod(methodName.Name);
            return (iObject) info.Invoke(obj, args);
        }

        public static MethodBinder FindMethod(iObject instance, Symbol methodName, iObject[] args)
        {
            return instance.CalculatedClass.FindMethod(methodName)
                //?? FindClrMethod(instance, methodName, args)
                //?? FindClrProperty(instance, methodName, args)
                //?? FindClrExtension(instance, methodName, args)
            ;
        }

        private static MethodBinder FindClrMethod(iObject instance, Symbol methodName, iObject[] args)
        {
            var type = instance.GetType();
            var method = type.GetMethods(
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(_ => _.Name == methodName.Name)
                .FirstOrDefault(_ => _.GetParameters().Length == args.Length);

            return method != null
                ? new ClrMethodBinder(methodName, instance.CalculatedClass, method)
                : null;
        }

        /*private static MethodBinder FindClrProperty(iObject instance, Symbol methodName, iObject[] args)
        {
            var property = instance.GetType().GetProperties(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
                ).Where(_ => _.Name == methodName.Name)
                .FirstOrDefault(_ => _.GetIndexParameters().Length == args.Length);

            return property != null
                ? new CompiledProperty(methodName, instance.CalculatedClass, property)
                : null;
        }

        private static MethodBinder FindClrExtension(iObject instance, Symbol methodName, iObject[] args)
        {
            // TODO static extension method
            throw new NotImplementedException();
        }*/

        // Convert from ExpressionType to operator name
        public static string Operator(ExpressionType type)
        {
            switch(type)
            {
                case ExpressionType.Add:                   return "+";
                case ExpressionType.AddChecked:            return "+";
                case ExpressionType.And:                   return "&";
                case ExpressionType.AndAlso:               return "&&";
                case ExpressionType.ArrayIndex:            return "[]";
                case ExpressionType.Call:                  return "call";
                case ExpressionType.Coalesce:              return "||";
                case ExpressionType.Divide:                return "/";
                case ExpressionType.Equal:                 return "==";
                case ExpressionType.ExclusiveOr:           return "^";
                case ExpressionType.GreaterThan:           return ">";
                case ExpressionType.GreaterThanOrEqual:    return ">=";
                case ExpressionType.LeftShift:             return "<<";
                case ExpressionType.LessThan:              return "<";
                case ExpressionType.LessThanOrEqual:       return "<=";
                case ExpressionType.Modulo:                return "%";
                case ExpressionType.Multiply:              return "*";
                case ExpressionType.MultiplyChecked:       return "*";
                case ExpressionType.Negate:                return "-@";
                case ExpressionType.UnaryPlus:             return "+@";
                case ExpressionType.NegateChecked:         return "-@";
                case ExpressionType.Not:                   return "!";
                case ExpressionType.NotEqual:              return "!=";
                case ExpressionType.Or:                    return "|";
                case ExpressionType.OrElse:                return "||";
                case ExpressionType.Power:                 return "^";
                case ExpressionType.RightShift:            return ">>";
                case ExpressionType.Subtract:              return "-";
                case ExpressionType.SubtractChecked:       return "-";
                case ExpressionType.AddAssign:             return "+=";
                case ExpressionType.AndAssign:             return "&&=";
                case ExpressionType.DivideAssign:          return "/=";
                case ExpressionType.ExclusiveOrAssign:     return "^=";
                case ExpressionType.LeftShiftAssign:       return "<<=";
                case ExpressionType.ModuloAssign:          return "%=";
                case ExpressionType.MultiplyAssign:        return "*=";
                case ExpressionType.OrAssign:              return "||=";
                case ExpressionType.PowerAssign:           return "**=";
                case ExpressionType.RightShiftAssign:      return ">>=";
                case ExpressionType.SubtractAssign:        return "-=";
                case ExpressionType.AddAssignChecked:      return "+=";
                case ExpressionType.MultiplyAssignChecked: return "*=";
                case ExpressionType.SubtractAssignChecked: return "-=";
                case ExpressionType.OnesComplement:        return "~";

                //case ExpressionType.ArrayLength = 4,
                //case ExpressionType.Conditional = 8,
                //case ExpressionType.Constant = 9,
                //case ExpressionType.Convert = 10,
                //case ExpressionType.ConvertChecked = 11,
                //case ExpressionType.Invoke = 17,
                //case ExpressionType.Lambda = 18,
                //case ExpressionType.ListInit = 22,
                //case ExpressionType.MemberAccess = 23,
                //case ExpressionType.MemberInit = 24,
                //case ExpressionType.New = 31,
                //case ExpressionType.NewArrayInit = 32,
                //case ExpressionType.NewArrayBounds = 33,
                //case ExpressionType.Parameter = 38,
                //case ExpressionType.Quote = 40,
                //case ExpressionType.TypeAs = 44,
                //case ExpressionType.TypeIs = 45,
                //case ExpressionType.Assign = 46,
                //case ExpressionType.Block = 47,
                //case ExpressionType.DebugInfo = 48,
                //case ExpressionType.Decrement = 49,
                //case ExpressionType.Dynamic = 50,
                //case ExpressionType.Default = 51,
                //case ExpressionType.Extension = 52,
                //case ExpressionType.Goto = 53,
                //case ExpressionType.Increment = 54,
                //case ExpressionType.Index = 55,
                //case ExpressionType.Label = 56,
                //case ExpressionType.RuntimeVariables = 57,
                //case ExpressionType.Loop = 58,
                //case ExpressionType.Switch = 59,
                //case ExpressionType.Throw = 60,
                //case ExpressionType.Try = 61,
                //case ExpressionType.Unbox = 62,
                //case ExpressionType.PreIncrementAssign = 77,
                //case ExpressionType.PreDecrementAssign = 78,
                //case ExpressionType.PostIncrementAssign = 79,
                //case ExpressionType.PostDecrementAssign = 80,
                //case ExpressionType.TypeEqual = 81,
                //case ExpressionType.IsTrue = 83,
                //case ExpressionType.IsFalse = 84
            };

            return null;
        }

        #endregion
    }
}
