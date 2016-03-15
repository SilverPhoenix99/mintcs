using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Threading;

namespace Mint
{
    public abstract class aObject : iObject
    {
        private static long nextId = 0;

        public virtual  long  Id                { get; } = Interlocked.Increment(ref nextId) << 2;
        public abstract Class Class             { get; }
        public virtual  bool  HasSingletonClass => false;
        public virtual  Class CalculatedClass   => HasSingletonClass ? SingletonClass : Class;
        public virtual  Class SingletonClass    { get { throw new TypeError("can't define singleton"); } }

        public virtual bool Frozen
        {
            get { return true; }
            protected set { throw new NotImplementedException(); }
        }

        public virtual void Freeze() {}


        public virtual string Inspect() => ToString();


        internal virtual string InspectInternal() => $"{Inspect()}:{Class.Name}";


        public bool IsA(Class klass) => Class.IsA(this, klass);


        public DynamicMetaObject GetMetaObject(Expression parameter) => new Object.Meta(parameter, this);


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
    }
}
