﻿using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    [TestOf(typeof(TrueClass))]
    public class TrueClassTests
    {
        [Test]
        public void TestId()
        {
            Assert.That(new TrueClass().Id, Is.EqualTo(2));
        }

        [Test]
        public void TestClass()
        {
            var value = new TrueClass();
            Assert.That(value.Class, Is.EqualTo(Class.TRUE));
            Assert.That(value.SingletonClass, Is.EqualTo(Class.TRUE));
            Assert.That(value.EffectiveClass, Is.EqualTo(Class.TRUE));
            Assert.IsFalse(value.HasSingletonClass);
        }

        [Test]
        public void TestIsA()
        {
            var value = new TrueClass();
            Assert.IsTrue(value.IsA(Class.TRUE));
            Assert.IsTrue(value.IsA(Class.OBJECT));
            Assert.IsTrue(value.IsA(Class.BASIC_OBJECT));
            Assert.IsFalse(value.IsA(Class.FIXNUM));
            Assert.IsFalse(value.IsA(Class.SYMBOL));
            Assert.IsFalse(value.IsA(Class.FALSE));
        }

        [Test]
        public void TestToString()
        {
            Assert.That(new TrueClass().ToString(), Is.EqualTo("true"));
        }

        [Test]
        public void TestInspect()
        {
            Assert.That(new TrueClass().Inspect(), Is.EqualTo(new TrueClass().ToString()));
        }

        [Test]
        public void TestFreeze()
        {
            var value = new TrueClass();
            Assert.IsTrue(value.Frozen);
            Assert.That(value.Freeze(), Is.EqualTo(value));
            Assert.IsTrue(value.Frozen);
        }

        [Test]
        public void TestSend()
        {
            Assert.That(Object.Send(new TrueClass(), new Symbol("nil?")), Is.EqualTo(new FalseClass()));
        }

        [Test]
        public void TestEquals()
        {
            Assert.IsTrue(new TrueClass().Equals(new TrueClass()));
            Assert.IsTrue(new TrueClass().Equals(true));
            Assert.IsFalse(new TrueClass().Equals(null));
            Assert.IsFalse(new TrueClass().Equals(new NilClass()));
            Assert.IsFalse(new TrueClass().Equals(new Fixnum()));
            Assert.IsFalse(new TrueClass().Equals(new FalseClass()));
            Assert.IsFalse(new TrueClass().Equals(false));
        }

        [Test]
        public void TestGetHashCode()
        {
            Assert.That(new TrueClass().GetHashCode(), Is.EqualTo(new TrueClass().Id.GetHashCode()));
        }

        [Test]
        public void TestCast()
        {
            Assert.That((bool) new TrueClass(), Is.True);
        }

        [Test]
        public void TestEqualityOperator()
        {
            Assert.IsTrue(new TrueClass() == new TrueClass());
            Assert.IsTrue(new TrueClass() == true);
            Assert.IsFalse(new TrueClass() == new NilClass());
            Assert.IsFalse(new TrueClass() == new Fixnum());
            Assert.IsFalse(new TrueClass() == new FalseClass());
            Assert.IsFalse(new TrueClass() == false);
        }

        [Test]
        public void TestInequalityOperator()
        {
            Assert.IsFalse(new TrueClass() != new TrueClass());
            Assert.IsFalse(new TrueClass() != true);
            Assert.IsTrue(new TrueClass() != new NilClass());
            Assert.IsTrue(new TrueClass() != new Fixnum());
            Assert.IsTrue(new TrueClass() != new FalseClass());
            Assert.IsTrue(new TrueClass() != false);
        }
    }
}
