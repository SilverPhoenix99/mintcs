using System;
using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    public class FixnumTests
    {
        private readonly Random rand = new Random();

        [Test]
        public void TestCreate()
        {
            const long integer = 15648L;
            var fixnum = new Fixnum(integer);

            Assert.AreEqual(integer, fixnum.Value);
            Assert.AreEqual(integer << 2 | 1, fixnum.Id);
        }

        [Test]
        public void TestSimpleMethods()
        {
            var fixnum = new Fixnum();

            Assert.AreEqual(Class.FIXNUM, fixnum.Class);
            Assert.AreEqual(Class.FIXNUM, fixnum.CalculatedClass);
            Assert.Throws<TypeError>(() => { var singletonClass = fixnum.SingletonClass; });
            Assert.IsFalse(fixnum.HasSingletonClass);
            Assert.AreEqual(fixnum.Value.GetHashCode(), fixnum.GetHashCode());
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
        public void TestToString()
        {
            Assert.AreEqual("0", new Fixnum().ToString());
            Assert.AreEqual("-64", new Fixnum(-100).ToString(16));

            var fixnum = new Fixnum(257);

            Assert.AreEqual("257", fixnum.ToString());
            Assert.AreEqual("100000001", fixnum.ToString(2));
            Assert.AreEqual("401", fixnum.ToString(8));
            Assert.AreEqual("101", fixnum.ToString(16));
            Assert.AreEqual("16a", fixnum.ToString(13));
            Assert.AreEqual("75", fixnum.ToString(36));
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

            Assert.AreEqual(fixnum.Inspect(), fixnum.ToString());
            Assert.AreEqual(fixnum.Inspect(16), fixnum.ToString(16));
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
            var fixnum = new Fixnum(10000);
            var sendResult = fixnum.Send(new Symbol("abs"));

            Assert.AreEqual(sendResult, fixnum);
        }
    }
}
