using System;
using System.Reflection;

namespace Mint
{
    public class Float : aObject
    {
        public Float(double value) : base()
        {
            Value = value;
        }

        public override Class  Class => CLASS;
        public          double Value { get; private set; }

        public override string ToString() => Value.ToString();

        public static explicit operator Float(double s) => new Float(s);

        public static explicit operator Float(float s) => new Float(s);

        public static explicit operator Float(Fixnum s) => new Float(s.Value);

        public static explicit operator double (Float s) => s.Value;

        #region Static
        
        public static readonly Class CLASS;

        static Float()
        {
            CLASS = new Class(Fixnum.INTEGER_CLASS, new Symbol(MethodBase.GetCurrentMethod().DeclaringType.Name));
            //Object.DefineClass(CLASS);
        }

        #endregion
    }
}
