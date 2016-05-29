using System.Reflection;
using Mint.Reflection;
using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    internal class ReflectorTests
    {
        [Test]
        public void TestMethod()
        {
            var member = typeof(Fixnum).GetMethod(nameof(Fixnum.BitLength));
            Assert.That(Reflector.Method(() => default(Fixnum).BitLength()), Is.EqualTo(member));
        }

        [Test]
        public void TestProperty()
        {
            var member = typeof(Fixnum).GetProperty(nameof(Fixnum.Value));
            Assert.That(Reflector.Property(() => default(Fixnum).Value), Is.EqualTo(member));
        }

        [Test]
        public void TestPropertyGetter()
        {
            var member = Reflector.Getter(() => default(BaseObject).Frozen);
            var property = typeof(BaseObject).GetProperty(nameof(BaseObject.Frozen));
            Assert.That(member, Is.EqualTo(property.GetMethod));
        }

        [Test]
        public void TestPropertySetter()
        {
            var member = Reflector.Setter(() => default(BaseObject).Frozen);
            var property = typeof(BaseObject).GetProperty(nameof(BaseObject.Frozen));
            Assert.That(member, Is.EqualTo(property.SetMethod));
        }

        [Test]
        public void TestConvert()
        {
            var member = Reflector.Convert(() => (long) default(Fixnum));
            Assert.That(member, Is.Not.Null);
            Assert.That(member, Is.InstanceOf(typeof(MethodInfo)));
        }

        [Test]
        public void TestIndexerAsProperty()
        {
            var member = Reflector.Property(() => default(Hash)[default(iObject)]);
            Assert.That(member, Is.Not.Null);
            Assert.That(member, Is.InstanceOf(typeof(PropertyInfo)));
        }

        [Test]
        public void TestIndexerAsMethod()
        {
            var member = Reflector.Method(() => default(Hash)[default(iObject)]);
            Assert.That(member, Is.Not.Null);
            Assert.That(member, Is.InstanceOf(typeof(MethodInfo)));
        }

        [Test]
        public void TestIndexerGetter()
        {
            var member = Reflector.Getter(() => default(Hash)[default(iObject)]);
            Assert.That(member, Is.Not.Null);
            Assert.That(member, Is.InstanceOf(typeof(MethodInfo)));
        }

        [Test]
        public void TestIndexerSetter()
        {
            var member = Reflector.Setter(() => default(Hash)[default(iObject)]);
            Assert.That(member, Is.Not.Null);
            Assert.That(member, Is.InstanceOf(typeof(MethodInfo)));
        }

        [Test]
        public void TestAction()
        {
            var member = typeof(ReflectorTests).GetMethod(nameof(TestAction));
            Assert.That(Reflector.Method(() => default(ReflectorTests).TestAction()), Is.EqualTo(member));
        }

        [Test]
        public void TestGenericAction()
        {
            var member = typeof(ReflectorTests).GetMethod(nameof(TestAction));
            Assert.That(Reflector<ReflectorTests>.Method(_ => _.TestAction()), Is.EqualTo(member));
        }
    }
}
