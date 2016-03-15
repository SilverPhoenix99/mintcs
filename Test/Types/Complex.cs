using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mint
{
    public class Complex : Object
    {
        public static new readonly Class CLASS = new Class(new Symbol("Complex"));

        public iObject Real { get; }
        public iObject Imag { get; }

        public Complex(iObject real, iObject imag)
        {
            if(Nil.IsNil(real) || Nil.IsNil(imag))
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
            return $"{Real.ToString()}{sign}{imag.ToString()}i";
        }


        public override string Inspect()
        {
            dynamic imag = Imag;
            var sign = imag < 0 ? "-" : "+";
            imag = imag.abs();
            return $"({Real.Inspect()}{sign}{imag.Inspect()}i)";
        }


        // TODO accept string
        //public Complex(String s) {  }

        static Complex()
        {
            DefineClass(CLASS);
        }
    }
}
