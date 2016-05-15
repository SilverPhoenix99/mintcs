﻿using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
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
            value.Freeze();
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
            var fixnum = new NilClass();

            Assert.IsTrue(fixnum.IsA(Class.NIL));
            Assert.IsTrue(fixnum.IsA(Class.OBJECT));
            Assert.IsTrue(fixnum.IsA(Class.BASIC_OBJECT));
            Assert.IsFalse(fixnum.IsA(Class.STRING));
            Assert.IsFalse(fixnum.IsA(Class.TRUE));
        }

        [Test]
        public void TestSend()
        {
            Assert.That(new NilClass().Send(new Symbol("nil?")), Is.EqualTo(new TrueClass()));
        }

        [Test]
        public void TestEquals()
        {
            Assert.IsTrue(new NilClass().Equals(new NilClass()));
        }

        [Test]
        public void TestEqual()
        {
            Assert.IsTrue(new NilClass().Equal(new NilClass()));
            Assert.IsTrue(new NilClass().Equal(null));
            Assert.IsFalse(new NilClass().Equal(true));
            Assert.IsFalse(new NilClass().Equal(new Fixnum()));
            Assert.IsFalse(new NilClass().Equal(new FalseClass()));
            Assert.IsFalse(new NilClass().Equal(false));
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
