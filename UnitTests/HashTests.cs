using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    [TestOf(typeof(Hash))]
    class HashTests
    {
        [Test]
        public void TestCreate()
        {
            Assert.That(new Hash(), Is.Not.Null);
            Assert.That(new Hash(3), Is.Not.Null);
        }

        [Test]
        public void TestGetter()
        {
            var emptyHash = new Hash();
            var fixnum0 = new Fixnum(0);
            var symbol = new Symbol("symbol");
            Assert.That(emptyHash[fixnum0], Is.EqualTo(null));

            var hash = new Hash { [symbol] = fixnum0 };
            Assert.That(hash[symbol], Is.EqualTo(fixnum0));
        }

        [Test]
        public void TestToString()
        {
            var emptyHash = new Hash();
            var fixnum0 = new Fixnum(0);
            var symbol = new Symbol("symbol");
            Assert.That(emptyHash.ToString, Is.EqualTo("{}"));

            var hash = new Hash();
            hash[symbol] = fixnum0;
            Assert.That(hash.ToString, Is.EqualTo("{:symbol=>0}"));
        }

        [Test]
        public void TestToArray()
        {
            var emptyHash = new Hash();
            var emptyArray = new Array();
            var fixnum0 = new Fixnum(0);
            var symbol = new Symbol("symbol");
            Assert.That(emptyHash.ToArray, Is.EqualTo(emptyArray));

            var hash = new Hash();
            var array = new Array();
            array.Push(new Array(symbol, fixnum0));
            hash[symbol] = fixnum0;
            Assert.That(hash.ToArray, Is.EqualTo(array));
        }
    }
}
