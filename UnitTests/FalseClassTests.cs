using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    [TestOf(typeof(FalseClass))]
    public class FalseClassTests
    {
        [Test]
        public void TestId()
        {
            Assert.That(new FalseClass().Id, Is.EqualTo(0));
        }

        [Test]
        public void TestClass()
        {
            var value = new FalseClass();
            Assert.That(value.Class, Is.EqualTo(Class.FALSE));
            Assert.That(value.SingletonClass, Is.EqualTo(Class.FALSE));
            Assert.That(value.EffectiveClass, Is.EqualTo(Class.FALSE));
            Assert.IsFalse(value.HasSingletonClass);
        }

        [Test]
        public void TestIsA()
        {
            var value = new FalseClass();
            Assert.IsTrue(value.IsA(Class.FALSE));
            Assert.IsTrue(value.IsA(Class.OBJECT));
            Assert.IsTrue(value.IsA(Class.BASIC_OBJECT));
            Assert.IsFalse(value.IsA(Class.FIXNUM));
            Assert.IsFalse(value.IsA(Class.SYMBOL));
            Assert.IsFalse(value.IsA(Class.TRUE));
        }

        [Test]
        public void TestToString()
        {
            Assert.That(new FalseClass().ToString(), Is.EqualTo("false"));
        }

        [Test]
        public void TestInspect()
        {
            Assert.That(new FalseClass().Inspect(), Is.EqualTo(new FalseClass().ToString()));
        }

        [Test]
        public void TestFreeze()
        {
            var value = new FalseClass();
            Assert.IsTrue(value.Frozen);
            Assert.That(value.Freeze(), Is.EqualTo(value));
            Assert.IsTrue(value.Frozen);
        }

        [Test]
        public void TestSend()
        {
            Assert.That(new FalseClass().Send(new Symbol("nil?")), Is.EqualTo(new FalseClass()));
        }

        [Test]
        public void TestEquals()
        {
            Assert.IsTrue(new FalseClass().Equals(new FalseClass()));
        }

        [Test]
        public void TestEqual()
        {
            Assert.IsTrue(new FalseClass().Equal(new FalseClass()));
            Assert.IsTrue(new FalseClass().Equal(false));
            Assert.IsFalse(new FalseClass().Equal(null));
            Assert.IsFalse(new FalseClass().Equal(new NilClass()));
            Assert.IsFalse(new FalseClass().Equal(new Fixnum()));
            Assert.IsFalse(new FalseClass().Equal(new TrueClass()));
            Assert.IsFalse(new FalseClass().Equal(true));
        }

        [Test]
        public void TestGetHashCode()
        {
            Assert.That(new FalseClass().GetHashCode(), Is.EqualTo(new FalseClass().Id.GetHashCode()));
        }

        [Test]
        public void TestCast()
        {
            Assert.That((bool) new FalseClass(), Is.False);
        }

        [Test]
        public void TestEqualityOperator()
        {
            Assert.IsTrue(new FalseClass() == new FalseClass());
            Assert.IsTrue(new FalseClass() == false);
            Assert.IsFalse(new FalseClass() == new NilClass());
            Assert.IsFalse(new FalseClass() == new Fixnum());
            Assert.IsFalse(new FalseClass() == new TrueClass());
            Assert.IsFalse(new FalseClass() == true);
        }

        [Test]
        public void TestInequalityOperator()
        {
            Assert.IsFalse(new FalseClass() != new FalseClass());
            Assert.IsFalse(new FalseClass() != false);
            Assert.IsTrue(new FalseClass() != new NilClass());
            Assert.IsTrue(new FalseClass() != new Fixnum());
            Assert.IsTrue(new FalseClass() != new TrueClass());
            Assert.IsTrue(new FalseClass() != true);
        }
    }
}
