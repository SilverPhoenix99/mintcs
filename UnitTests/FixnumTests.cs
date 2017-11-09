using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    [TestOf(typeof(Fixnum))]
    public class FixnumTests
    {
        [Test]
        public void TestCreate([Random(long.MinValue, long.MaxValue, 1)] long value)
        {
            var fixnum = new Fixnum(value);

            Assert.That(fixnum.Value, Is.EqualTo(value));
            Assert.That(fixnum.Id, Is.EqualTo(value << 1 | 1));
        }

        [Test]
        public void TestClass([Random(long.MinValue, long.MaxValue, 1)] long value)
        {
            var fixnum = new Fixnum(value);

            Assert.That(fixnum.Class, Is.EqualTo(Class.FIXNUM));
            Assert.That(fixnum.EffectiveClass, Is.EqualTo(Class.FIXNUM));
            Assert.Throws<TypeError>(() => { var singletonClass = fixnum.SingletonClass; });
            Assert.IsFalse(fixnum.HasSingletonClass);
        }

        [Test]
        public void TestGetHashCode([Random(long.MinValue, long.MaxValue, 1)] long value)
        {
            var fixnum = new Fixnum(value);
            Assert.That(fixnum.GetHashCode(), Is.EqualTo(fixnum.Value.GetHashCode()));
        }

        [Test]
        public void TestFreeze()
        {
            var fixnum = new Fixnum();

            Assert.IsTrue(fixnum.Frozen);
            Assert.That(fixnum.Freeze(), Is.EqualTo(fixnum));
            Assert.IsTrue(fixnum.Frozen);
        }

        [Test]
        public void TestEquals([Random(0L, 10000L, 3)] long value)
        {
            var fixnum = new Fixnum(value);
            var floatObject = new Float(value);

            Assert.IsTrue(fixnum.Equals(new Fixnum(value)));
            Assert.IsTrue(fixnum.Equals(floatObject));
            Assert.IsFalse(fixnum.Equals(new Fixnum(value+1)));
            Assert.IsFalse(fixnum.Equals(null));
            Assert.IsFalse(fixnum.Equals(new String(value.ToString())));
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
        public void TestInspect([Random(long.MinValue, long.MaxValue, 3)] long value)
        {
            var fixnum = new Fixnum(value);

            Assert.That(fixnum.Inspect(), Is.EqualTo(fixnum.ToString()));
        }

        [Test]
        public void TestIsA()
        {
            var fixnum = new Fixnum();

            Assert.IsTrue(Object.IsA(fixnum, Class.FIXNUM));
            Assert.IsTrue(Object.IsA(fixnum, Class.INTEGER));
            Assert.IsTrue(Object.IsA(fixnum, Class.NUMERIC));
            Assert.IsTrue(Object.IsA(fixnum, Class.OBJECT));
            Assert.IsTrue(Object.IsA(fixnum, Class.BASIC_OBJECT));
            Assert.IsFalse(Object.IsA(fixnum, Class.STRING));
            Assert.IsFalse(Object.IsA(fixnum, Class.FLOAT));
        }

        [Test]
        public void TestToStringError()
        {
            var fixnum = new Fixnum(10000);

            Assert.Throws<ArgumentError>(() => { fixnum.ToString(1); });
            Assert.Throws<ArgumentError>(() => { fixnum.ToString(37); });
        }

        [Test]
        public void TestSend([Random(1L, long.MaxValue, 3)] long value)
        {
            var positive = new Fixnum(value);
            var negative = new Fixnum(-value);
            var result = Object.Send(negative, new Symbol("abs"));

            Assert.That(result, Is.EqualTo(positive));
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
        public void TestUnaryNegation([Random(long.MinValue, long.MaxValue, 1)] long value)
        {
            Assert.That(-new Fixnum(0), Is.EqualTo(new Fixnum(0)));
            Assert.That(-new Fixnum(value), Is.EqualTo(new Fixnum(-value)));
        }

        [Test]
        public void TestLongCast([Random(long.MinValue, long.MaxValue, 1)] long value)
        {
            Assert.That((Fixnum) value, Is.EqualTo(new Fixnum(value)));
            Assert.That((long) new Fixnum(value), Is.EqualTo(value));
        }

        [Test]
        public void TestIntCast([Random(int.MinValue, int.MaxValue, 1)] int value)
        {
            Assert.That((Fixnum) value, Is.EqualTo(new Fixnum(value)));
            Assert.That((int) new Fixnum(value), Is.EqualTo(value));
        }

        [Test]
        public void TestUIntCast([Random(uint.MinValue, uint.MaxValue, 1)] uint value)
        {
            Assert.That((Fixnum) value, Is.EqualTo(new Fixnum(value)));
            Assert.That((uint) new Fixnum(value), Is.EqualTo(value));
        }

        [Test]
        public void TestShortCast([Random(short.MinValue, short.MaxValue, 1)] short value)
        {
            Assert.That((Fixnum) value, Is.EqualTo(new Fixnum(value)));
            Assert.That((short) new Fixnum(value), Is.EqualTo(value));
        }

        [Test]
        public void TestUShortCast([Random(ushort.MinValue, ushort.MaxValue, 1)] ushort value)
        {
            Assert.That((Fixnum) value, Is.EqualTo(new Fixnum(value)));
            Assert.That((ushort) new Fixnum(value), Is.EqualTo(value));
        }

        [Test]
        public void TestSByteCast([Random(sbyte.MinValue, sbyte.MaxValue, 1)] sbyte value)
        {
            Assert.That((Fixnum) value, Is.EqualTo(new Fixnum(value)));
            Assert.That((sbyte) new Fixnum(value), Is.EqualTo(value));
        }

        [Test]
        public void TestByteCast([Random(byte.MinValue, byte.MaxValue, 1)] byte value)
        {
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
            Assert.That((Fixnum) new Float(longValue), Is.EqualTo(new Fixnum(longValue)));
        }

        [Test]
        public void TestSum(
            [Random(long.MinValue, long.MaxValue, 1)] long left,
            [Random(long.MinValue, long.MaxValue, 1)] long right
        )
        {
            Assert.That(new Fixnum(left) + new Fixnum(right), Is.EqualTo(new Fixnum(left + right)));
        }
    }
}
