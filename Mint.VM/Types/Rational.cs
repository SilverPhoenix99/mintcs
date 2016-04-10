using System;
using System.Reflection;

namespace Mint
{
    public class Rational : BaseObject
    {
        public Rational(iObject numerator, iObject denominator) : base(CLASS)
        {
            throw new NotImplementedException();
        }

        public static Rational operator -(Rational v) { throw new NotImplementedException(); }

        #region Static

        public static readonly Class CLASS;

        static Rational()
        {
            CLASS = ClassBuilder<Rational>.Describe(Fixnum.NUMERIC_CLASS)
            .Class;
        }

        #endregion
    }
}
