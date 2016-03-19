using System.Globalization;
using System.Reflection;

namespace Mint
{
    public class Float : aFrozenObject
    {
        public Float(double value) : base()
        {
            Value = value;
        }

        public override Class  Class => CLASS;
        public          double Value { get; }

        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

        public static Float operator -(Float v) => new Float(-v.Value);

        public static explicit operator Float(double v) => new Float(v);

        public static explicit operator double(Float v) => v.Value;

        public static explicit operator Float(float v) => new Float(v);

        public static explicit operator float (Float v) => (float) v.Value;

        public static explicit operator Float(Fixnum v) => new Float(v.Value);

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
