using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Mint
{
    public partial class Bignum : FrozenObject
    {
        private const string CHAR_CONVERSION = "0123456789abcdefghijklmnopqrstuvwxyz";

        public override Class Class => Class.BIGNUM;

        public BigInteger Value { get; }

        public int Sign => Value.Sign;

        internal Bignum(BigInteger value)
        {
            Value = value;
        }

        public static Bignum Parse(string value, int radix = 10)
        {
            ValidateRadixBoundary(radix);
            return new Parser(value, (uint) radix).Parse();
        }

        private static void ValidateRadixBoundary(int radix)
        {
            if(radix < 2 || radix > 36)
            {
                throw new ArgumentError("invalid radix " + radix);
            }
        }

        public override string ToString() => Value.ToString();

        public override bool Equals(object other)
        {
            if(other is Bignum) return Equals((Bignum) other);
            if(other is Fixnum) return Value.Equals((long) (Fixnum) other);
            if(other is Float) return Value.Equals(new BigInteger((Float) other));
            var instance = other as iObject;
            return instance != null && Object.ToBool(Class.EqOp.Call(instance, this));
        }

        public bool Equals(Bignum other) => other != null && Value.Equals(other.Value);

        public string ToString(int radix)
        {
            ValidateRadixBoundary(radix);
            
            if(radix == 10)
            {
                return ToString();
            }

            var digits = BuildDigitsList(radix);

            if(digits.Count == 0)
            {
                return "0";
            }

            ReverseDigits(digits);
            return new string(digits.ToArray());
        }

        private IList<char> BuildDigitsList(int radix)
        {
            var bigRadix = new BigInteger(radix);
            var value = Value;
            var digits = new List<char>();

            while(!value.IsZero)
            {
                var convertedChar = CHAR_CONVERSION[(int) (value % bigRadix)];
                digits.Add(convertedChar);
                value /= bigRadix;
            }

            return digits;
        }

        private static void ReverseDigits(IList<char> digits)
        {
            var halfList = digits.Count / 2;
            for(var i = 0; i < halfList; i++)
            {
                SwapValues(digits, i, digits.Count - i - 1);
            }
        }

        private static void SwapValues(IList<char> list, int index1, int index2)
        {
            list[index1] ^= list[index2];
            list[index2] ^= list[index1];
            list[index1] ^= list[index2];
        }
    }
}
