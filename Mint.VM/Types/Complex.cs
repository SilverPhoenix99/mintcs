using System;

namespace Mint
{
    public class Complex : BaseObject
    {
        public iObject Real { get; }
        public iObject Imag { get; }

        // TODO accept string
        public Complex(iObject real, iObject imag) : base(CLASS)
        {
            if(NilClass.IsNil(real) || NilClass.IsNil(imag))
            {
                throw new TypeError("can't convert nil into Complex");
            }
            
            if(!real.IsA(Fixnum.INTEGER_CLASS) && !real.IsA(Float.CLASS))
            {
                throw new TypeError($"can't convert {real.Class} into Complex");
            }

            if(!imag.IsA(Fixnum.INTEGER_CLASS) && !imag.IsA(Float.CLASS))
            {
                throw new TypeError($"can't convert {imag.Class} into Complex");
            }

            Real = real;
            Imag = imag;
        }

        public override string ToString()
        {
            dynamic imag = Imag;
            var sign = imag < 0 ? "-" : "+";
            imag = imag.abs();
            return $"{Real}{sign}{imag.ToString()}i";
        }

        public override string Inspect()
        {
            dynamic imag = Imag;
            var sign = imag < 0 ? "-" : "+";
            imag = imag.abs();
            return $"({Real.Inspect()}{sign}{imag.Inspect()}i)";
        }

        public static Complex operator -(Complex v) { throw new NotImplementedException(); }

        #region Static

        public static readonly Class CLASS;

        static Complex()
        {
            CLASS = ClassBuilder<Complex>.Describe(Fixnum.NUMERIC_CLASS)
            .Class;
        }

        #endregion
    }
}
