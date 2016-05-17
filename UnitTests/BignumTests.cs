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

        //[Test]
        //public void TestToArray()
        //{
        //    const string value = "18446744073709551616";
        //    var bignum = Bignum.Parse(value);
        //
        //    Assert.That(bignum.ToArray(), Is.EqualTo(new[] { 0UL, 1UL }));
        //}

        [Test]
        public void TestHexParse()
        {
            const string value = "693ea77ad11a5bbb1b44f185443956 red balloons";
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

            Assert.That(Bignum.Parse("").ToString(), Is.EqualTo("0"));
        }
    }
}
