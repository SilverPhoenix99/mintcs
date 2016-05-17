namespace Mint
{
    public enum Sign
    {
        Positive = 1,
        Negative = -1
    }

    public partial class Bignum : FrozenObject
    {
        public override Class Class => Class.BIGNUM;

        private readonly ulong[] digits;

        public Sign Sign { get; }

        private Bignum(Sign sign, ulong[] digits)
        {
            Sign = sign;
            this.digits = digits;
        }

        public static Bignum Parse(string value, int radix = 10) =>  new Parser(value, (uint) radix).Parse();

        public ulong[] ToArray() => (ulong[]) digits.Clone();
    }
}
