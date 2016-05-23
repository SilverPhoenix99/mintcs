using Mint.MethodBinding;
using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    [TestOf(typeof(ArgumentKind))]
    public class ArgumentKindTests
    {
        [Test]
        public void TestDescription()
        {
            Assert.That(ArgumentKind.Simple.Description, Is.EqualTo("Simple"));
            Assert.That(ArgumentKind.Rest.Description, Is.EqualTo("Rest"));
            Assert.That(ArgumentKind.Key.Description, Is.EqualTo("Key"));
            Assert.That(ArgumentKind.KeyRest.Description, Is.EqualTo("KeyRest"));
            Assert.That(ArgumentKind.Block.Description, Is.EqualTo("Block"));
        }

        [Test]
        public void TestToString()
        {
            Assert.That(ArgumentKind.Simple.ToString(), Is.EqualTo("Simple"));
            Assert.That(ArgumentKind.Rest.ToString(), Is.EqualTo("Rest"));
            Assert.That(ArgumentKind.Key.ToString(), Is.EqualTo("Key"));
            Assert.That(ArgumentKind.KeyRest.ToString(), Is.EqualTo("KeyRest"));
            Assert.That(ArgumentKind.Block.ToString(), Is.EqualTo("Block"));
        }
    }
}