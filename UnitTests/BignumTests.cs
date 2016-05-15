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
        }
    }
}
