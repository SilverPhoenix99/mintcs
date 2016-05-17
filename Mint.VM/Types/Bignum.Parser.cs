using System.Collections.Generic;

namespace Mint
{
    public partial class Bignum
    {
		private class Parser
		{
            private const ulong INVALID_DIGIT = ulong.MaxValue;

            private readonly string value;
			private readonly uint radix;
		    private readonly List<ulong> digits = new List<ulong>();
			private readonly Sign sign;
			private int beginIndex;
			private ulong accumulated;

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

			private Sign CalculateSign(int index)
			{
			    if(index >= value.Length || value[index] != '-')
			    {
			        return Sign.Positive;
			    }

			    beginIndex++;
			    return Sign.Negative;
			}

			public Bignum Parse()
			{
                accumulated = 0UL;

                for(var i = beginIndex; i < value.Length; i++)
				{
				    var digit = ConvertLetterToDigit(value[i], radix);

					if(digit == INVALID_DIGIT)
					{
					    break;
					}

					Accumulate(digit);
				}

				if(accumulated != 0)
				{
				    digits.Add(accumulated);
				}

                return new Bignum(sign, digits.ToArray());
			}

            private static ulong ConvertLetterToDigit(ulong letter, uint radix)
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

			private void Accumulate(ulong digit)
			{
                var tempAccumulator = unchecked(accumulated * radix + digit);

                if(tempAccumulator < accumulated)
                {
					digits.Add(tempAccumulator);
					accumulated = 1;
					return;
                }

                accumulated = tempAccumulator;
            }
        }
    }
}
