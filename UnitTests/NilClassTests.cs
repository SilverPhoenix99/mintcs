using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    [TestOf(typeof(NilClass))]
    public class NilClassTests
    {
        [Test]
        public void TestId()
        {
            Assert.That(new NilClass().Id, Is.EqualTo(0x4));
        }

        [Test]
        public void TestClass()
        {
            Assert.That(new NilClass().Class, Is.EqualTo(Class.NIL));
            Assert.That(new NilClass().EffectiveClass, Is.EqualTo(Class.NIL));
            Assert.That(new NilClass().SingletonClass, Is.EqualTo(Class.NIL));
            Assert.IsFalse(new NilClass().HasSingletonClass);
        }

        [Test]
        public void TestFreeze()
        {
            var value = new NilClass();
            Assert.IsTrue(value.Frozen);
            Assert.That(value.Freeze(), Is.EqualTo(value));
            Assert.IsTrue(value.Frozen);
        }

        [Test]
        public void TestToString()
        {
            Assert.That(new NilClass().ToString(), Is.EqualTo(""));
        }

        [Test]
        public void TestInspect()
        {
            Assert.That(new NilClass().Inspect(), Is.EqualTo("nil"));
        }

        [Test]
        public void TestIsA()
        {
            var instance = new NilClass();

            Assert.IsTrue(instance.IsA(Class.NIL));
            Assert.IsTrue(instance.IsA(Class.OBJECT));
            Assert.IsTrue(instance.IsA(Class.BASIC_OBJECT));
            Assert.IsFalse(instance.IsA(Class.STRING));
            Assert.IsFalse(instance.IsA(Class.TRUE));
        }

        [Test]
        public void TestSend()
        {
            Assert.That(Object.Send(new NilClass(), new Symbol("nil?")), Is.EqualTo(new TrueClass()));
        }

        [Test]
        public void TestEquals()
        {
            Assert.IsTrue(new NilClass().Equals(new NilClass()));
            Assert.IsTrue(new NilClass().Equals(null));
            Assert.IsFalse(new NilClass().Equals(true));
            Assert.IsFalse(new NilClass().Equals(new Fixnum()));
            Assert.IsFalse(new NilClass().Equals(new FalseClass()));
            Assert.IsFalse(new NilClass().Equals(false));
        }

        [Test]
        public void TestGetHashCode()
        {
            Assert.That(new NilClass().GetHashCode(), Is.EqualTo(new NilClass().Id.GetHashCode()));
        }

        [Test]
        public void TestEqualityOperator()
        {
            Assert.IsTrue(new NilClass() == new NilClass());
            Assert.IsTrue(new NilClass() == null);
            Assert.IsFalse(new NilClass() == new Fixnum());
            Assert.IsFalse(new NilClass() == new FalseClass());
            Assert.IsFalse(new NilClass() == false);
        }

        [Test]
        public void TestInequalityOperator()
        {
            Assert.IsFalse(new NilClass() != new NilClass());
            Assert.IsFalse(new NilClass() != null);
            Assert.IsTrue(new NilClass() != new Fixnum());
            Assert.IsTrue(new NilClass() != new FalseClass());
            Assert.IsTrue(new NilClass() != false);
        }

        [Test]
        public void TestCast()
        {
            Assert.IsFalse((bool) new NilClass());
        }
    }
}
