using System;
using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    [TestOf(typeof(Fixnum))]
    public class FixnumTests
    {
        private readonly Random rand = new Random();

        [Test]
        public void TestCreate()
        {
            const long integer = 15648L;
            var fixnum = new Fixnum(integer);

            Assert.That(fixnum.Value, Is.EqualTo(integer));
            Assert.That(fixnum.Id, Is.EqualTo(integer << 2 | 1));
        }

        [Test]
        public void TestSimpleMethods()
        {
            var fixnum = new Fixnum();

            Assert.That(fixnum.Class, Is.EqualTo(Class.FIXNUM));
            Assert.That(fixnum.CalculatedClass, Is.EqualTo(Class.FIXNUM));
            Assert.Throws<TypeError>(() => { var singletonClass = fixnum.SingletonClass; });
            Assert.IsFalse(fixnum.HasSingletonClass);
            Assert.That(fixnum.GetHashCode(), Is.EqualTo(fixnum.Value.GetHashCode()));
        }

        [Test]
        public void TestFreeze()
        {
            var fixnum = new Fixnum();

            Assert.IsTrue(fixnum.Frozen);
            fixnum.Freeze();
            Assert.IsTrue(fixnum.Frozen);
        }

        [Test]
        public void TestEquals()
        {
            var integer = rand.Next(10000);
            var fixnum1 = new Fixnum(integer);
            var fixnum2 = new Fixnum(integer);
            var floatObject = new Float(integer);

            Assert.IsTrue(fixnum1.Equals(fixnum2));
            Assert.IsTrue(fixnum1.Equals(floatObject));
            Assert.IsFalse(fixnum1.Equals(new String(integer.ToString())));
        }

        [Test]
        public void TestToString()
        {
            Assert.That(new Fixnum().ToString(), Is.EqualTo("0"));
            Assert.That(new Fixnum(-100).ToString(16), Is.EqualTo("-64"));

            var fixnum = new Fixnum(257);

            Assert.That(fixnum.ToString(), Is.EqualTo("257"));
            Assert.That(fixnum.ToString(2), Is.EqualTo("100000001"));
            Assert.That(fixnum.ToString(8), Is.EqualTo("401"));
            Assert.That(fixnum.ToString(16), Is.EqualTo("101"));
            Assert.That(fixnum.ToString(13), Is.EqualTo("16a"));
            Assert.That(fixnum.ToString(36), Is.EqualTo("75"));
        }

        [Test]
        public void TestIsA()
        {
            var fixnum = new Fixnum();

            Assert.IsTrue(fixnum.IsA(Class.FIXNUM));
            Assert.IsTrue(fixnum.IsA(Class.INTEGER));
            Assert.IsTrue(fixnum.IsA(Class.NUMERIC));
            Assert.IsTrue(fixnum.IsA(Class.OBJECT));
            Assert.IsTrue(fixnum.IsA(Class.BASIC_OBJECT));
            Assert.IsFalse(fixnum.IsA(Class.STRING));
            Assert.IsFalse(fixnum.IsA(Class.FLOAT));
        }

        [Test]
        public void TestInspect()
        {
            var fixnum = new Fixnum(rand.Next(int.MinValue, int.MaxValue));

            Assert.That(fixnum.Inspect(), Is.EqualTo(fixnum.ToString()));
            Assert.That(fixnum.Inspect(16), Is.EqualTo(fixnum.ToString(16)));
        }

        [Test]
        public void TestToStringError()
        {
            var fixnum = new Fixnum(10000);

            Assert.Throws<ArgumentError>(() => { fixnum.ToString(1); });
            Assert.Throws<ArgumentError>(() => { fixnum.ToString(37); });
        }

        [Test]
        public void TestSend()
        {
            var positiveFixnum = new Fixnum(10000);
            var negativeFixnum = new Fixnum(-positiveFixnum.Value);
            var sendResult = negativeFixnum.Send(new Symbol("abs"));

            Assert.That(sendResult, Is.EqualTo(positiveFixnum));
        }

        [Test]
        public void TestBitLength()
        {
            Assert.That(new Fixnum(-(1L<<37)).BitLength(),  Is.EqualTo(new Fixnum(37)));
            Assert.That(new Fixnum(-(1<<27)).BitLength(),   Is.EqualTo(new Fixnum(27)));
            Assert.That(new Fixnum(-(1<<12)-1).BitLength(), Is.EqualTo(new Fixnum(13)));
            Assert.That(new Fixnum(-(1<<12)).BitLength(),   Is.EqualTo(new Fixnum(12)));
            Assert.That(new Fixnum(-(1<<12)+1).BitLength(), Is.EqualTo(new Fixnum(12)));
            Assert.That(new Fixnum(-0x101).BitLength(),     Is.EqualTo(new Fixnum( 9)));
            Assert.That(new Fixnum(-0x100).BitLength(),     Is.EqualTo(new Fixnum( 8)));
            Assert.That(new Fixnum(-0xff).BitLength(),      Is.EqualTo(new Fixnum( 8)));
            Assert.That(new Fixnum(-2).BitLength(),         Is.EqualTo(new Fixnum( 1)));
            Assert.That(new Fixnum(-1).BitLength(),         Is.EqualTo(new Fixnum( 0)));
            Assert.That(new Fixnum(0).BitLength(),          Is.EqualTo(new Fixnum( 0)));
            Assert.That(new Fixnum(1).BitLength(),          Is.EqualTo(new Fixnum( 1)));
            Assert.That(new Fixnum(0xff).BitLength(),       Is.EqualTo(new Fixnum( 8)));
            Assert.That(new Fixnum(0x100).BitLength(),      Is.EqualTo(new Fixnum( 9)));
            Assert.That(new Fixnum((1<<12)-1).BitLength(),  Is.EqualTo(new Fixnum(12)));
            Assert.That(new Fixnum((1<<12)).BitLength(),    Is.EqualTo(new Fixnum(13)));
            Assert.That(new Fixnum((1<<12)+1).BitLength(),  Is.EqualTo(new Fixnum(13)));
            Assert.That(new Fixnum(1<<27).BitLength(),      Is.EqualTo(new Fixnum(28)));
            Assert.That(new Fixnum(1L<<37).BitLength(),     Is.EqualTo(new Fixnum(38)));
        }

        [Test]
        public void TestUnaryNegation()
        {
            var integer = rand.Next(int.MinValue, int.MaxValue);
            Assert.That(-new Fixnum(0), Is.EqualTo(new Fixnum(0)));
            Assert.That(-new Fixnum(integer), Is.EqualTo(new Fixnum(-integer)));
        }

        [Test]
        public void TestLongCast()
        {
            const long value = 100L;
            Assert.That((Fixnum) value, Is.EqualTo(new Fixnum(value)));
            Assert.That((long) new Fixnum(value), Is.EqualTo(value));
        }

        [Test]
        public void TestIntCast()
        {
            const int value = 100;
            Assert.That((Fixnum) value, Is.EqualTo(new Fixnum(value)));
            Assert.That((int) new Fixnum(value), Is.EqualTo(value));
        }

        [Test]
        public void TestUIntCast()
        {
            const uint value = 100u;
            Assert.That((Fixnum) value, Is.EqualTo(new Fixnum(value)));
            Assert.That((uint) new Fixnum(value), Is.EqualTo(value));
        }

        [Test]
        public void TestShortCast()
        {
            const short value = 100;
            Assert.That((Fixnum) value, Is.EqualTo(new Fixnum(value)));
            Assert.That((short) new Fixnum(value), Is.EqualTo(value));
        }

        [Test]
        public void TestUShortCast()
        {
            const ushort value = 100;
            Assert.That((Fixnum) value, Is.EqualTo(new Fixnum(value)));
            Assert.That((ushort) new Fixnum(value), Is.EqualTo(value));
        }

        [Test]
        public void TestSByteCast()
        {
            const sbyte value = 100;
            Assert.That((Fixnum) value, Is.EqualTo(new Fixnum(value)));
            Assert.That((sbyte) new Fixnum(value), Is.EqualTo(value));
        }

        [Test]
        public void TestByteCast()
        {
            const byte value = 100;
            Assert.That((Fixnum) value, Is.EqualTo(new Fixnum(value)));
            Assert.That((byte) new Fixnum(value), Is.EqualTo(value));
        }

        [Test]
        public void TestDoubleCast()
        {
            const long longValue = 100;
            const double doubleValue = 100;
            Assert.That((Fixnum) doubleValue, Is.EqualTo(new Fixnum(longValue)));
            Assert.That((double) new Fixnum(longValue), Is.EqualTo(doubleValue));
        }

        [Test]
        public void TestFloatCast()
        {
            const long longValue = 100;
            Assert.That((Fixnum) new Float(100), Is.EqualTo(new Fixnum(longValue)));
        }

        [Test]
        public void TestSum()
        {
            long left = rand.Next(int.MinValue, int.MaxValue);
            long right = rand.Next(int.MinValue, int.MaxValue);

            Assert.That(new Fixnum(left) + new Fixnum(right), Is.EqualTo(new Fixnum(left + right)));
        }
    }
}
