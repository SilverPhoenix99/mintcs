using System.Collections.Generic;

namespace Mint
{
    public class Bignum : FrozenObject
    {
        private enum Sign
        {
            Positive = 1,
            Negative = -1
        }

        private const int NUM_BITS_BYTE = 8;
        private const int NUM_BITS_UINT = sizeof(uint) * NUM_BITS_BYTE;
        private const int NUM_BITS_ULONG = sizeof(ulong) * NUM_BITS_BYTE;
        private const ulong HALF_ULONG_MASK = (1UL << NUM_BITS_UINT) - 1;
        private const ulong INVALID_DIGIT = ulong.MaxValue;

        public override Class Class => Class.BIGNUM;

        private readonly Sign sign;
        private readonly ulong[] digits;

        private Bignum(Sign sign, ulong[] digits)
        {
            this.sign = sign;
            this.digits = digits;
        }

        public static Bignum Parse(string value, int radix = 10)
        {
            var sign = value[0] == '-' ? Sign.Negative : Sign.Positive;
            var digits = new List<ulong>();

            for(var i = sign == Sign.Negative ? 1 : 0; i < value.Length; i++)
            {
                var accumulator = ConvertLetterToDigit(value[i], radix);

                if(accumulator == INVALID_DIGIT)
                {
                    break;
                }

                for(var j = 0; j < digits.Count; j++)
                {
                    var low = (HALF_ULONG_MASK & digits[j]) * (ulong) radix + accumulator;
                    var high = (HALF_ULONG_MASK & (digits[j] >> NUM_BITS_UINT)) * (ulong) radix;
                    high += low >> NUM_BITS_UINT;
                    accumulator = high >> NUM_BITS_UINT;

                    digits[j] = ((high & HALF_ULONG_MASK) << NUM_BITS_UINT) + (low & HALF_ULONG_MASK);
                }

                if(accumulator != 0)
                {
                    digits.Add(accumulator);
                }
            }

            return new Bignum(sign, digits.ToArray());
        }

        private static ulong ConvertLetterToDigit(ulong letter, int radix)
        {
            if('0' <= letter && letter <= '9')
            {
                letter -= '0';
            }
            else if('a' <= letter && letter <= 'z')
            {
                letter -= 'a' + 10;
            }
            else if('A' <= letter && letter <= 'Z')
            {
                letter -= 'A' + 10;
            }
            else
            {
                return INVALID_DIGIT;
            }

            return letter < (ulong) radix ? letter : INVALID_DIGIT;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
