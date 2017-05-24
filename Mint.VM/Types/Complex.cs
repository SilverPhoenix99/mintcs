using System;

namespace Mint
{
    public class Complex : BaseObject
    {
        // TODO accept string
        public Complex(iObject real, iObject imag) : base(Class.COMPLEX)
        {
            if(NilClass.IsNil(real) || NilClass.IsNil(imag))
            {
                throw new TypeError("can't convert nil into Complex");
            }
            
            if(!Object.IsA(real, Class.INTEGER) && !Object.IsA(real, Class.FLOAT))
            {
                throw new TypeError($"can't convert {real.Class} into Complex");
            }

            if(!Object.IsA(imag, Class.INTEGER) && !Object.IsA(imag, Class.FLOAT))
            {
                throw new TypeError($"can't convert {imag.Class} into Complex");
            }

            Real = real;
            Imag = imag;
        }


        public iObject Real { get; }
        public iObject Imag { get; }


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
    }
}
