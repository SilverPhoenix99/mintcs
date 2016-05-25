using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    [TestOf(typeof(Array))]
    public class ArrayTests
    {
        [Test]
        public void TestCreate()
        {
            Assert.That(new Array(), Is.Not.Null);
            Assert.That(new Array(new iObject[0]), Is.Not.Null);
        }

        [Test]
        public void TestCount()
        {
            var emptyArray = new Array();
            Assert.That(emptyArray.Count, Is.EqualTo(0));
            var array = new Array(new Fixnum(1), new String("string"));
            Assert.That(array.Count, Is.EqualTo(2));
        }

        [Test]
        public void TestIndexerGetter()
        {
            var emptyArray = new Array();
            Assert.That(emptyArray[0], Is.EqualTo(new NilClass()));
            var array = new Array(new Fixnum(1), new String("string"));
            Assert.That(array[0], Is.EqualTo(new Fixnum(1)));
            Assert.That(array[1], Is.EqualTo(new String("string")));
            Assert.That(array[2], Is.EqualTo(new NilClass()));
            Assert.That(array[-2], Is.EqualTo(new Fixnum(1)));
            Assert.That(array[-1], Is.EqualTo(new String("string")));
            Assert.That(array[-3], Is.EqualTo(new NilClass()));
            Assert.Throws<TypeError>(() => { var a = array[new Symbol("symbol")]; });
        }

        [Test]
        public void TestIndexerSetter()
        {
            var fixnum1 = new Fixnum(1);
            var str = new String("string");
            var nil = new NilClass();
            var emptyArray = new Array();
            emptyArray[0] = fixnum1;
            Assert.That(emptyArray[0], Is.EqualTo(fixnum1));

            emptyArray = new Array();
            emptyArray[1] = fixnum1;
            Assert.That(emptyArray[0], Is.EqualTo(nil));
            Assert.That(emptyArray[1], Is.EqualTo(fixnum1));

            emptyArray = new Array();
            Assert.Throws<IndexError>(() => { emptyArray[-2] = fixnum1; });

            var array = new Array(fixnum1, str);
            array[0] = str;
            array[1] = new NilClass();

            Assert.That(array[0], Is.EqualTo(str));
            Assert.That(array[1], Is.EqualTo(nil));

            array[-1] = str;
            array[-2] = new NilClass();

            Assert.That(array[-1], Is.EqualTo(str));
            Assert.That(array[-2], Is.EqualTo(nil));

            Assert.Throws<IndexError>(() => { emptyArray[-3] = fixnum1; });
            Assert.Throws<TypeError>(() => { emptyArray[nil] = fixnum1; });
        }

        [Test]
        public void TestPush()
        {
            var fixnum1 = new Fixnum(1);
            var str = new String("string");
            var nil = new NilClass();

            var emptyArray = new Array();
            Assert.That(emptyArray.Push(fixnum1), Is.EqualTo(new Array(fixnum1)));

            var array = new Array(fixnum1, str, nil);
            Assert.That(array.Push(fixnum1), Is.EqualTo(new Array(fixnum1, str, nil, fixnum1)));
        }

        [Test]
        public void TestToString()
        {
            var fixnum1 = new Fixnum(1);
            var str = new String("string");
            var nil = new NilClass();

            var emptyArray = new Array();
            Assert.That(emptyArray.ToString, Is.EqualTo("[]"));

            var array = new Array(fixnum1, str, nil);
            Assert.That(array.ToString, Is.EqualTo("[1, \"string\", nil]"));
        }
    }
}
