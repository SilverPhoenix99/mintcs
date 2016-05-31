using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    [TestOf(typeof(Array))]
    public class ArrayTests
    {
        public readonly Fixnum fixnum0 = new Fixnum(0);
        public readonly Fixnum fixnum1 = new Fixnum(1);
        public readonly Fixnum fixnum2 = new Fixnum(2);
        public readonly Fixnum fixnum3 = new Fixnum(3);
        public readonly Fixnum fixnum4 = new Fixnum(4);
        public readonly Fixnum fixnum5 = new Fixnum(5);
        public readonly Fixnum fixnum7 = new Fixnum(7);

        public readonly String a = new String("a");
        public readonly String b = new String("b");
        public readonly String c = new String("c");
        public readonly String d = new String("d");
        public readonly String e = new String("e");
        public readonly String f = new String("f");

        public readonly NilClass nil = new NilClass();

        [Test]
        public void TestCreate()
        {
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
            var hash = new Hash();
            var array = new Array(3, hash);
            Assert.That(array[0], Is.EqualTo(hash));
            Assert.That(array[1], Is.EqualTo(hash));
            Assert.That(array[2], Is.EqualTo(hash));
        }

        [Test] //count &block length size
        public void TestCount()
        {
            // [1, "string"].count = 2
            var emptyArray = new Array();
            Assert.That(emptyArray.Count, Is.EqualTo(0));
            var array = new Array(new Fixnum(1), new String("string"));
            Assert.That(array.Count, Is.EqualTo(2));

            // TODO [1, "string", 1].count(1) = 2
            // TODO [1, 2, 3, 5].count { |x| x % 2 == 0 } = 3
            Assert.Fail("Test Not Implemented Yet");
        }

        // TODO
        // try_convert
        // <=>
        // ==
        // [start, length]=
        // [range]=
        // any? &block
        // assoc
        // bsearch
        // bsearch_index
        // collect collect! &block
        // combination &block
        // cycle &block
        // delete_if &block
        // dig
        // drop_while &block
        // each &block
        // each_index
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
        // unshift
        // values_at
        // zip
        // |

        [Test] // &
        public void TestAnd()
        {
            // [1, 1, 3, 5] & [1, 2, 3] = [1, 3]
            var array = new Array(fixnum1, fixnum1, fixnum3, fixnum5);
            var otherArray = new Array(fixnum1, fixnum2, fixnum3);
            var expectedResult = new Array(fixnum1, fixnum3);
            array = array.AndAlso(otherArray);
            Assert.That(array.Count, Is.EqualTo(2));
            Assert.That(array, Is.EqualTo(expectedResult));
        }

        [Test] // * join
        public void TestMultiply()
        {
            // [1, 2, 3] * 3 = [1, 2, 3, 1, 2, 3, 1, 2, 3]
            var array = new Array(fixnum1, fixnum2, fixnum3);
            var result = array * fixnum3;
            var expected = new Array(fixnum1, fixnum2, fixnum3, fixnum1, fixnum2, fixnum3, fixnum1, fixnum2, fixnum3);
            Assert.That(result.Count, Is.EqualTo(9));
            Assert.That(result, Is.EqualTo(expected));
            
            // [1, 2, 3] * "," = "1,2,3"
            var result2 = array * ",";
            Assert.That(result2, Is.EqualTo("1,2,3"));

            result2 = array.Join();
            Assert.That(result2, Is.EqualTo("123"));

            // [1, 2, 3].join(",") = "1,2,3"
            result2 = array.Join(",");
            Assert.That(result2, Is.EqualTo("1,2,3"));

            // TODO move to fixnum 1 * [1, 2, 3] = TypeError
            // Assert.Throws<TypeError>(() => { var a = fixnum1 * array; });

            // [1, 2, 3] *= 3
            array *= fixnum3;
            Assert.That(array, Is.EqualTo(expected));
        }

        [Test] // + concat
        public void TestPlus()
        {
            // [1, 2, 3] + [4, 5] = [1, 2, 3, 4, 5]
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
            var array = new Array(a, c);
            var otherArray = new Array(a, c, fixnum7);
            Assert.That(array.Equals(otherArray), Is.False);
            // [ "a", "c", 7 ] == [ "a", "c", 7 ]     #=> true
            array = new Array(a, c, fixnum7);
            Assert.That(array.Equals(otherArray), Is.True);

            // [ "a", "c", 7 ] == [ "a", "d", "f" ]   #=> false
            otherArray = new Array(a, d, f);
            Assert.That(array.Equals(otherArray), Is.False);

            // TODO class equality
            // class X; end
            // x1 = X.new; x2 = X.new
            // [x1, x2] == [x2, x1] => false
            Assert.Fail("Test Not Implemented Yet");
        }

        [Test] // clear
        public void TestClear()
        {
            var array = new Array(a, b,c, d, e);
            array.Clear();
            Assert.That(array.Count, Is.EqualTo(0));
            Assert.That(array, Is.EqualTo(new Array()));
        }

        [Test] // compact
        public void TestCompact()
        {
            // [ "a", nil, "b", nil, "c", nil].compact #=> [ "a", "b", "c" ]
            var array = new Array(a, nil, b, nil, c, nil);
            var expected = new Array(a, b, c);
            var result = array.Compact();
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(array.Count, Is.EqualTo(6));
            Assert.That(result.Compact(), Is.EqualTo(expected));
        }

        [Test] // compact!
        public void TestCompactSelf()
        {
            // [ "a", nil, "b", nil, "c", nil].compact #=> [ "a", "b", "c" ]
            var array = new Array(a, nil, b, nil, c, nil);
            var expected = new Array(a, b, c);
            array.CompactSelf();
            Assert.That(array.Count, Is.EqualTo(3));
            Assert.That(array, Is.EqualTo(expected));
            // [ "a", "b", "c"].compact! #=> nil
            Assert.That(array.CompactSelf(), Is.Null);
        }

        [Test] // delete &block
        public void TestDelete()
        {
            // ary = [ "a", "b", "b", "b", "c" ]
            // ary.delete("b") #=> "b"
            // ary #=> ["a", "c"]
            var array = new Array(a, b, c);
            var expected = new Array(a, c);
            Assert.That(array.Delete(b), Is.EqualTo(b));
            Assert.That(array, Is.EqualTo(expected));

            // TODO ary.delete("z") { "not found" } #=> "not found"
            // Assert.Fail("Not Implemented Yet");

        }

        [Test]
        public void TestDeleteAt()
        {
            // a = ["a", "b", "c", "d"]
            // a.delete_at(2) #=> "c"
            // a #=> ["a", "b", "d"]
            var array = new Array(a, b, c, d);
            var expected = new Array(a, b, d);
            Assert.That(array.DeleteAt(2), Is.EqualTo(c));
            Assert.That(array, Is.EqualTo(expected));
            // a.delete_at(99) #=> nil
            Assert.That(array.DeleteAt(99), Is.Null);
            // a.delete_at(-1) #=> "d"
            expected = new Array(a, b);
            Assert.That(array.DeleteAt(-1), Is.EqualTo(d));
            Assert.That(array, Is.EqualTo(expected));
        }

        [Test] // drop take
        public void TestDrop()
        {
            // [1, 2, 3, 4, 5, 0].drop(3) #=> [4, 5, 0]
            var array = new Array(fixnum1, fixnum2, fixnum3, fixnum4, fixnum5,fixnum0);
            var expected = new Array(fixnum4, fixnum5, fixnum0);
            Assert.That(array.Drop(3), Is.EqualTo(expected));
            Assert.Throws<ArgumentError>(() => array.Drop(-1));
        }

        [Test] // empty?
        public void TestIsEmpty()
        {
            // [].empty?   #=> true
            var array = new Array();
            Assert.That(array.IsEmpty, Is.True);
        }

        [Test]
        public void TestFirst()
        {
            var array = new Array(fixnum0, a, nil);
            var expected = new Array(fixnum0, a);
            Assert.That(array.First(), Is.EqualTo(fixnum0));   // a.first => 0
            Assert.That(array.First(2), Is.EqualTo(expected)); // a.first(2) => [0, "a"]
            Assert.That(array.First(4), Is.EqualTo(array)); // a.first(2) => [0, "a"]
        }

        [Test]
        public void TestLast()
        {
            var array = new Array(fixnum0, a, nil);
            var expected = new Array(a, nil);
            Assert.That(array.Last(), Is.EqualTo(nil));       // a.last => nil
            Assert.That(array.Last(2), Is.EqualTo(expected)); // a.last(2) => ["a", nil]
            Assert.That(array.Last(4), Is.EqualTo(array)); // a.last(2) => ["a", nil]
        }

        [Test] // [] [start, length] [range] at first last slice slice!
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
            var nil = new NilClass();
            var array = new Array(fixnum1, fixnum2, fixnum3);
            var otherArray = new Array(a, b, c);
            array.Replace(otherArray);
            Assert.That(array, Is.EqualTo(otherArray));

            Assert.Throws<TypeError>(() => { array.Replace(null); });
            
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

        [Test]// uniq uniq!
        public void TestUniq()
        {
            // [ "a", "a", "b", "b", "c" ].uniq = ["a", "b", "c"]
            var a = new String("a");
            var b = new String("b");
            var c = new String("c");
            var array = new Array(a, a, b, b, c);
            var expected = new Array(a, b, c);
            Assert.That(array.Uniq, Is.EqualTo(expected));
            Assert.That(array.UniqSelf, Is.EqualTo(expected));

            // TODO Block
            // b = [["student","sam"], ["student","george"], ["teacher","matz"]]
            // b.uniq { | s | s.first } # => [["student", "sam"], ["teacher", "matz"]]
            Assert.Fail("Not Implemented Yet");
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