using System;

namespace Mint
{
    public class Rational : BaseObject
    {
        public Rational(iObject numerator, iObject denominator) : base(Class.RATIONAL)
        {
            throw new NotImplementedException();
        }

        public static Rational operator -(Rational v) { throw new NotImplementedException(); }
    }
}
