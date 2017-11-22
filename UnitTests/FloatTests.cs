using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    [TestOf(typeof(Float))]
    public class FloatTests
    {
        [Test]
        public void TestCreate([Random(float.MinValue, float.MaxValue, 1)] double rawValue)
        {
            var value = new Float(rawValue);

            Assert.That(value.Value, Is.EqualTo(rawValue));
            Assert.That(value.Id, Is.Positive);
            Assert.That(value.Id & 0x3, Is.Zero);
        }

        [Test]
        public void TestClass([Random(float.MinValue, float.MaxValue, 1)] double rawValue)
        {
            var value = new Float(rawValue);

            Assert.That(value.Class, Is.EqualTo(Class.FLOAT));
            Assert.That(value.EffectiveClass, Is.EqualTo(Class.FLOAT));
            Assert.Throws<TypeError>(() => { var singletonClass = value.SingletonClass; });
            Assert.IsFalse(value.HasSingletonClass);
        }

        [Test]
        public void TestGetHashCode([Random(float.MinValue, float.MaxValue, 1)] double rawValue)
        {
            var value = new Float(rawValue);
            Assert.That(value.GetHashCode(), Is.EqualTo(value.Value.GetHashCode()));
        }

        [Test]
        public void TestFreeze()
        {
            var value = new Float(0.0);

            Assert.IsTrue(value.Frozen);
            Assert.That(value.Freeze(), Is.EqualTo(value));
            Assert.IsTrue(value.Frozen);
        }

        [Test]
        public void TestEquals([Random(0L, 10000L, 3)] long rawValue)
        {
            var float1 = new Float(rawValue);
            var fixnum = new Fixnum(rawValue);

            Assert.IsTrue(float1.Equals(new Float(rawValue)));
            Assert.IsTrue(float1.Equals(fixnum));
            Assert.IsFalse(float1.Equals(new Float(rawValue+1)));
            Assert.IsFalse(float1.Equals(null));
            Assert.IsFalse(float1.Equals(new String(rawValue.ToString())));
        }

        [Test]
        public void TestToString()
        {
            Assert.That(new Float(0.0).ToString(), Is.EqualTo("0.0"));
            Assert.That(new Float(-100.0).ToString(), Is.EqualTo("-100.0"));
            Assert.That(new Float(1.123).ToString(), Is.EqualTo("1.123"));
        }

        [Test]
        public void TestInspect([Random(long.MinValue, long.MaxValue, 3)] long rawValue)
        {
            var value = new Float(rawValue);

            Assert.That(value.Inspect(), Is.EqualTo(value.ToString()));
        }

        [Test]
        public void TestIsA()
        {
            var value = new Float(0.0);

            Assert.IsTrue(value.IsA(Class.FLOAT));
            Assert.IsFalse(value.IsA(Class.INTEGER));
            Assert.IsTrue(value.IsA(Class.NUMERIC));
            Assert.IsTrue(value.IsA(Class.OBJECT));
            Assert.IsTrue(value.IsA(Class.BASIC_OBJECT));
            Assert.IsFalse(value.IsA(Class.STRING));
            Assert.IsFalse(value.IsA(Class.FIXNUM));
        }

        [Test]
        public void TestSend([Random(1L, long.MaxValue, 3)] long value)
        {
            var positive = new Float(value);
            var negative = new Float(-value);
            var result = Object.Send(negative, new Symbol("abs"));

            Assert.That(result, Is.EqualTo(positive));
        }

        [Test]
        public void TestUnaryNegation([Random(1L, long.MaxValue, 3)] long value)
        {
            var positive = new Float(value);
            var negative = new Float(-value);

            Assert.That(-positive, Is.EqualTo(negative));
        }

        [Test]
        public void TestCast([Random(float.MinValue, float.MaxValue, 3)] double rawValue)
        {
            var value = new Float(rawValue);
            var floatValue = (float) rawValue;

            Assert.That((double) value, Is.EqualTo(rawValue));
            Assert.That((Float) floatValue, Is.EqualTo(new Float(floatValue)));
            Assert.That((float) value, Is.EqualTo(floatValue));
        }

        [Test]
        public void TestCastToFixnum([Random(long.MinValue, long.MaxValue, 3)] long rawValue)
        {
            var value = new Fixnum(rawValue);

            Assert.That((Float) value, Is.EqualTo(new Float(rawValue)));
        }
    }
}
