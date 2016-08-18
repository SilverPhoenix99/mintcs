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
            var fixnum3 = new Fixnum(3);
            Assert.That(new Array(), Is.Not.Null);
            Assert.That(new Array(new iObject[0]), Is.Not.Null);

            var array = new Array((int) fixnum3.Value, new Hash());
            Assert.That(array, Is.Not.Null);
            Assert.That(array.Count, Is.EqualTo((int) fixnum3.Value));

            array = new Array((int) fixnum3.Value);
            Assert.That(array, Is.Not.Null);
            Assert.That(array.Count, Is.EqualTo((int) fixnum3.Value));
        }

        [Test]
        public void TestDefaultValue()
        {
            var sym = new Symbol("x");
            var array = new Array(3, sym);
            Assert.That(array[0], Is.EqualTo(sym));
            Assert.That(array[1], Is.EqualTo(sym));
            Assert.That(array[2], Is.EqualTo(sym));
        }

        [Test] //count length size
        public void TestCount()
        {
            // [1, "string"].count = 2
            var emptyArray = new Array();
            Assert.That(emptyArray.Count, Is.EqualTo(0));
            var array = new Array(new Fixnum(1), new String("string"));
            Assert.That(array.Count, Is.EqualTo(2));

            // TODO [1, "string"].count(1) = 1
            // TODO [1, 2, 3, 5].count { |x| x % 2 == 0 } = 3
            Assert.Fail("Test Not Implemented Yet");
        }

        // TODO
        // try_convert
        // <=>
        // ==
        // [start, length]=
        // [range]=
        // any?
        // assoc
        // at
        // bsearch
        // collect collect!
        // combination
        // concat
        // cycle
        // delete_at
        // delete_if
        // drop
        // drop_while
        // each
        // each_index
        // empty?
        // eql?
        // fetch
        // fill
        // find_index
        // flatten flatten!
        // frozen?
        // hash
        // include?
        // index
        // initialize_copy
        // insert
        // keep_if
        // map map!
        // pack
        // permutation
        // pop
        // product
        // rassoc
        // reject reject!
        // repeated_combination
        // repeated_permutation
        // reverse_each
        // rindex
        // rotate rotate!
        // sample
        // select select!
        // shift
        // shuffle shuffle!
        // sort sort!
        // sort_by!
        // take
        // take_while
        // to_a
        // to_ary
        // to_h
        // transpose
        // uniq uniq!
        // unshift
        // values_at
        // zip
        // |

        [Test] // &
        public void TestAnd()
        {
            // [1, 1, 3, 5] & [1, 2, 3] = [1, 3]
            var fixnum1 = new Fixnum(1);
            var fixnum3 = new Fixnum(3);
            var array = new Array(fixnum1, fixnum1, fixnum3, new Fixnum(5));
            var otherArray = new Array(fixnum1, new Fixnum(2), fixnum3);
            var expectedResult = new Array(fixnum1, fixnum3);
            var result = array.AndAlso(otherArray);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test] // * join
        public void TestMultiply()
        {
            // [1, 2, 3] * 3 = [1, 2, 3, 1, 2, 3, 1, 2, 3]
            var fixnum1 = new Fixnum(1);
            var fixnum2 = new Fixnum(2);
            var fixnum3 = new Fixnum(3);
            var array = new Array(fixnum1, fixnum2, fixnum3);
            var result = array * fixnum3;
            var expectedResult = new Array(fixnum1, fixnum2, fixnum3, fixnum1, fixnum2, fixnum3, fixnum1, fixnum2, fixnum3);
            Assert.That(result.Count, Is.EqualTo(9));
            Assert.That(result, Is.EqualTo(expectedResult));

            // [1, 2, 3] * "," = "1,2,3"
            var result2 = array * new String(",");
            Assert.That(result2, Is.EqualTo(new String("1,2,3")));

            result2 = array.Join();
            Assert.That(result2, Is.EqualTo(new String("123")));

            // [1, 2, 3].join(",") = "1,2,3"
            result2 = array.Join(new String(","));
            Assert.That(result2, Is.EqualTo(new String("1,2,3")));

            // TODO move to fixnum 1 * [1, 2, 3] = TypeError
            // Assert.Throws<TypeError>(() => { var a = fixnum1 * array; });

            // [1, 2, 3] *= 3
            array *= fixnum3;
            Assert.That(array, Is.EqualTo(expectedResult));
        }

        [Test] // +
        public void TestPlus()
        {
            // [1, 2, 3] + [4, 5] = [1, 2, 3, 4, 5]
            var fixnum1 = new Fixnum(1);
            var fixnum2 = new Fixnum(2);
            var fixnum3 = new Fixnum(3);
            var fixnum4 = new Fixnum(4);
            var fixnum5 = new Fixnum(5);
            var array = new Array(fixnum1, fixnum2, fixnum3);
            var otherArray = new Array(fixnum4, fixnum5);
            var expectedResult = new Array(fixnum1, fixnum2, fixnum3, fixnum4, fixnum5);
            var result = array + otherArray;
            Assert.That(result, Is.EqualTo(expectedResult));

            // [4, 5] + [1, 2, 3] = [4, 5, 1, 2, 3]
            expectedResult = new Array(fixnum4, fixnum5, fixnum1, fixnum2, fixnum3);
            result = otherArray + array;
            Assert.That(result, Is.EqualTo(expectedResult));


            Assert.Throws<TypeError>(() => { var a = array + (Array) null; }); // [1, 2, 3] + nil
            Assert.Throws<TypeError>(() => { var a = (Array) null + array; }); // nil + [1, 2, 3]
        }

        [Test] // -
        public void TestMinus()
        {
            // [ 1, 1, 2, 2, 3, 3, 4, 5 ] - [ 1, 2, 4 ]  #=>  [ 3, 3, 5 ]
            var fixnum1 = new Fixnum(1);
            var fixnum2 = new Fixnum(2);
            var fixnum3 = new Fixnum(3);
            var fixnum4 = new Fixnum(4);
            var fixnum5 = new Fixnum(5);
            var array = new Array(fixnum1, fixnum1, fixnum2, fixnum2, fixnum3, fixnum3, fixnum4, fixnum5);
            var otherArray = new Array(fixnum1, fixnum2, fixnum4);
            var expectedResult = new Array(fixnum3, fixnum3, fixnum5);
            var result = array - otherArray;
            Assert.That(result, Is.EqualTo(expectedResult));

            Assert.Throws<TypeError>(() => { var a = array + (Array) null; }); // [1, 2, 3] - nil
            Assert.Throws<TypeError>(() => { var a = (Array) null + array; });  // nil - [1, 2, 3]
        }

        [Test] // <<
        public void TestAdd()
        {
            // [ 1, 2 ] << "c" << "d" << [ 3, 4 ] => [ 1, 2, "c", "d", [ 3, 4 ] ]
            var fixnum1 = new Fixnum(1);
            var fixnum2 = new Fixnum(2);
            var fixnum3 = new Fixnum(3);
            var fixnum4 = new Fixnum(4);
            var c = new String("c");
            var d = new String("d");
            var array1 = new Array(fixnum1, fixnum2);
            var array2 = new Array(fixnum3, fixnum4);
            var expected = new Array(fixnum1, fixnum2, c, d, array2);
            Assert.That(array1.Add(c).Add(d).Add(array2), Is.EqualTo(expected));

            // TODO Assert.Throws<ArgumentError> (() =>  `a.<<*[1, 2, 3]` )
        }

        [Test] // TODO <=>
        public void TestCompareTo()
        {
            // [ "a", "a", "c" ] <=> [ "a", "b", "c" ]   #=> -1
            var a = new String("a");
            var b = new String("b");
            var c = new String("c");
            var fixnum1 = new Fixnum(1);
            var fixnum2 = new Fixnum(2);
            var fixnum3 = new Fixnum(3);
            var fixnum4 = new Fixnum(4);
            var fixnum5 = new Fixnum(5);
            var fixnum6 = new Fixnum(6);
            var symbol = new Symbol("two");
            var array = new Array(a, a, c);
            var otherArray = new Array(a, b, c);
            // Assert.That(array.CompareTo(otherArray), Is.EqualTo(-1));

            //[ 1, 2, 3, 4, 5, 6 ] <=> [ 1, 2 ] => +1
            array = new Array(fixnum1, fixnum2, fixnum3, fixnum4, fixnum5, fixnum6);
            otherArray = new Array(fixnum1, fixnum2);
            // Assert.That(array.CompareTo(otherArray), Is.EqualTo(1));

            // [ 1, 2 ] <=> [ 1, :two ] => nil
            // Assert.That(array.CompareTo(otherArray), Is.EqualTo(new NilClass()));
            Assert.Fail("Test Not Implemented Yet");
        }

        [Test] // TODO ==
        public void TestEqual()
        {
            // [ "a", "c" ]    == [ "a", "c", 7 ]     #=> false
            var a = new String("a");
            var c = new String("c");
            var d = new String("d");
            var f = new String("f");
            var fixnum7 = new Fixnum(7);
            var array = new Array(a, c);
            var otherArray = new Array(a, c, fixnum7);
            // Assert.That(array == otherArray, Is.False);
            // [ "a", "c", 7 ] == [ "a", "c", 7 ]     #=> true
            array = new Array(a, c, fixnum7);
            // Assert.That(array == otherArray, Is.True);

            // [ "a", "c", 7 ] == [ "a", "d", "f" ]   #=> false
            otherArray = new Array(a, d, f);
            // Assert.That(array == otherArray, Is.False);
            Assert.Fail("Test Not Implemented Yet");
        }

        [Test] // clear
        public void TestClear()
        {
            var array = new Array(new Fixnum(1), new String("string"));
            array.Clear();
            Assert.That(array.Count, Is.EqualTo(0));
        }

        [Test] // compact compact!
        public void TestCompact()
        {
            var nil = new NilClass();
            var fixnum0 = new Fixnum(0);
            var array = new Array(nil, nil);
            var newArray = array.Compact();
            Assert.That(newArray.Count, Is.EqualTo(0));
            Assert.That(array.Count, Is.EqualTo(2));
            array.CompactSelf();
            Assert.That(array.Count, Is.EqualTo(0));
        }

        [Test] // delete TODO
        public void TestDelete()
        {
            Assert.Fail("Test Not Implemented Yet");
        }

        [Test] // [] [start, length] [range] first last slice slice!
        public void TestIndexerGetter()
        {
            var fixnum0 = new Fixnum(0);
            var str = new String("string");
            var nil = new NilClass();
            var emptyArray = new Array();
            Assert.That(emptyArray[0], Is.EqualTo(nil));
            var array = new Array(fixnum0, str);         // a = [0, "string"]

            // []
            Assert.That(array[0], Is.EqualTo(fixnum0));  //  a[0] => 0
            Assert.That(array[1], Is.EqualTo(str));      //  a[1] => "string"
            Assert.That(array[2], Is.EqualTo(nil));      //  a[2] => nil

            Assert.That(array[-2], Is.EqualTo(fixnum0)); // a[-2] => 0
            Assert.That(array[-1], Is.EqualTo(str));     // a[-1] => "string"
            Assert.That(array[-3], Is.EqualTo(nil));     // a[-3] => "string"

            // TODO
            // a[0, 3]  => [0, "string"]
            // a[-2, 1] => [0]
            // a[0..5]  => [0, "string"]
            // a[0...1] => [0]
            Assert.That(array.First, Is.EqualTo(fixnum0)); // a.first => 0
            Assert.That(array.Last, Is.EqualTo(str));      // a.last  => "string"

            // TODO
            // slice!
            Assert.Fail("Test Not Implemented Yet");
        }

        [Test] // []= [start, length]= [range]=
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
        }

        [Test] // push
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

        [Test] // replace
        public void TestReplace()
        {
            // [1, 2, 3].replace(['a', 'b', 'c']) = ['a', 'b', 'c']
            var a = new String("a");
            var b = new String("b");
            var c = new String("c");
            var fixnum1 = new Fixnum(1);
            var fixnum2 = new Fixnum(2);
            var fixnum3 = new Fixnum(3);
            var array = new Array(fixnum1, fixnum2, fixnum3);
            var otherArray = new Array(a, b, c);
            array.Replace(otherArray);
            Assert.That(array, Is.EqualTo(otherArray));
        }

        [Test] // reverse reverse!
        public void TestReverse()
        {
            // [1, 2, 3].reverse = [3, 2, 1]
            var a = new String("a");
            var b = new String("b");
            var c = new String("c");
            var fixnum1 = new Fixnum(1);
            var fixnum2 = new Fixnum(2);
            var fixnum3 = new Fixnum(3);
            var array = new Array(fixnum1, fixnum2, fixnum3);
            var expectedResult = new Array(fixnum3, fixnum2, fixnum1);
            var result = array.Reverse();
            Assert.That(result, Is.EqualTo(expectedResult));
            array.ReverseSelf();
            Assert.That(array, Is.EqualTo(expectedResult));
        }

        [Test] // to_string
        public void TestToString()
        {
            // [1, "string", nil].to_s = "[1, \"string\", nil]"
            var fixnum1 = new Fixnum(1);
            var str = new String("string");
            var nil = new NilClass();

            var emptyArray = new Array();
            Assert.That(emptyArray.ToString, Is.EqualTo("[]"));

            var array = new Array(fixnum1, str, nil);
            Assert.That(array.ToString, Is.EqualTo("[1, \"string\", nil]"));
        }

        [Test] // inspet
        public void TestInspect()
        {
            // [1, "string", nil].to_s = "[1, \"string\", nil]"
            var fixnum1 = new Fixnum(1);
            var str = new String("string");
            var nil = new NilClass();

            var emptyArray = new Array();
            Assert.That(emptyArray.Inspect, Is.EqualTo("[]"));

            var array = new Array(fixnum1, str, nil);
            Assert.That(array.Inspect, Is.EqualTo("[1, \"string\", nil]"));
        }
    }
}