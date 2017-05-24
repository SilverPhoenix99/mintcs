using System.Numerics;

namespace Mint
{
    public partial class Bignum
    {
        private class Parser
        {
            private const uint INVALID_DIGIT = uint.MaxValue;


            private readonly string value;
            private readonly uint radix;
            private int beginIndex;
            private readonly int sign;


            public Parser(string value, uint radix)
            {
                this.value = value;
                this.radix = radix;
                beginIndex = CalculateBeginIndex(value);
                sign = CalculateSign(beginIndex);
            }


            private static int CalculateBeginIndex(string value)
            {
                for(var i = 0; i < value.Length; i++)
                {
                    if(!char.IsWhiteSpace(value[i]))
                    {
                        return i;
                    }
                }

                return value.Length;
            }


            private int CalculateSign(int index)
            {
                if(index >= value.Length || value[index] != '-')
                {
                    return 1;
                }

                beginIndex++;
                return -1;
            }


            public Bignum Parse()
            {
                var accumulator = new BigInteger();

                for(var i = beginIndex; i < value.Length; i++)
                {
                    var digit = ConvertLetterToDigit(value[i], radix);

                    if(digit == INVALID_DIGIT)
                    {
                        break;
                    }

                    accumulator = accumulator * radix + digit;
                }

                if(sign < 0)
                {
                    accumulator = -accumulator;
                }

                return new Bignum(accumulator);
            }


            private static uint ConvertLetterToDigit(uint letter, uint radix)
            {
                if('0' <= letter && letter <= '9')
                {
                    letter -= '0';
                }
                else if('a' <= letter && letter <= 'z')
                {
                    letter -= 'a' - 10;
                }
                else if('A' <= letter && letter <= 'Z')
                {
                    letter -= 'A' - 10;
                }
                else
                {
                    return INVALID_DIGIT;
                }

                return letter < radix ? letter : INVALID_DIGIT;
            }
        }
    }
}
