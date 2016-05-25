using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    [TestOf(typeof(Symbol))]
    class SymbolTests
    {
        [Test]
        public void TestCreate()
        {
            Assert.That(new Symbol("symbol"), Is.Not.Null);
        }

        [Test]
        public void TestToString()
        {
            var symbol = new Symbol("symbol");
            Assert.That(symbol.ToString, Is.EqualTo("symbol"));
        }

        [Test]
        public void TestInspect()
        {
            var symbol = new Symbol("symbol");
            Assert.That(symbol.Inspect, Is.EqualTo(":symbol"));
        }

        [Test]
        public void TestEquals()
        {
            var symbol = new Symbol("symbol");
            Assert.True(symbol.Equals(symbol));
            Assert.False(symbol.Equals(new Fixnum(0)));
        }

        //[Test]
        //public void TestIsA()
        //{
        //    var symbol = new Symbol("symbol");
        //    Assert.True(symbol.IsA();
        //    Assert.False(symbol.Equals(new Fixnum(0)));
        //}
    }
}
