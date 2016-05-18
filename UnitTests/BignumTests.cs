using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    public class BignumTests
    {
        [Test]
        public void TestCreate()
        {
            const string value = "18446744073709551616";

            var bignum = Bignum.Parse(value);

            Assert.That(bignum, Is.Not.Null);
        }

        [Test]
        public void TestSign()
        {
            Assert.That(Bignum.Parse("1").Sign, Is.EqualTo(1));
            Assert.That(Bignum.Parse("-1").Sign, Is.EqualTo(-1));
        }

        [Test]
        public void TestHexParse()
        {
            const string value = "693eA77aD11a5bBb1b44F185443956red balloons";
            var bignum = Bignum.Parse(value, 16);

            Assert.That(bignum.ToString(), Is.EqualTo("546461948654684354874631879846541654"));
            Assert.That(bignum.ToString(16), Is.EqualTo("693ea77ad11a5bbb1b44f185443956"));
        }

        [Test]
        public void TestClass()
        {
            const string value = "18446744073709551616";
            var bignum = Bignum.Parse(value);

            Assert.That(bignum.Class, Is.EqualTo(Class.BIGNUM));
            Assert.That(bignum.EffectiveClass, Is.EqualTo(Class.BIGNUM));
            Assert.Throws<TypeError>(() => { var singletonClass = bignum.SingletonClass; });
            Assert.IsFalse(bignum.HasSingletonClass);
        }

        [Test]
        public void TestToString()
        {
            const string value = "18446744073709551616";
            var bignum = Bignum.Parse(value);

            Assert.That(bignum.ToString(), Is.EqualTo(value));
            Assert.That(bignum.ToString(10), Is.EqualTo(bignum.ToString()));
            Assert.Throws<ArgumentError>(() => { bignum.ToString(1); });
            Assert.Throws<ArgumentError>(() => { bignum.ToString(37); });

            Assert.That(Bignum.Parse("0").ToString(10), Is.EqualTo("0"));
        }

        [Test]
        public void TestPrefixWhitespace()
        {
            Assert.That(Bignum.Parse("  1  ").ToString(), Is.EqualTo("1"));
            Assert.That(Bignum.Parse("  -1  ").ToString(), Is.EqualTo("-1"));
        }

        [Test]
        public void TestToStringWithRadixNoDigits()
        {
            Assert.That(Bignum.Parse("0").ToString(8), Is.EqualTo("0"));
        }

        [Test]
        public void TestInvalidString()
        {
            Assert.That(Bignum.Parse("not a number").ToString(8), Is.EqualTo("0"));
        }

        [Test]
        public void TestOnlyWhitespaceString()
        {
            Assert.That(Bignum.Parse("\t \r\n").ToString(8), Is.EqualTo("0"));
        }
    }
}
