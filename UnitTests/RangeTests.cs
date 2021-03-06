﻿using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    [TestOf(typeof(Range))]
    public class RangeTests
    {
        [Test]
        public void TestCreate()
        {
            Assert.That(new Range(new Fixnum(0), new Fixnum(2)), Is.Not.Null);
            Assert.That(new Range(new String("a"), new String("z")), Is.Not.Null);
        }

        [Test]
        public void TestInclude()
        {
            var fixnum0 = new Fixnum(0);
            var range = new Range(fixnum0, new Fixnum(2));
            Assert.True(range.Include(fixnum0));
            Assert.False(range.Include(new Fixnum(5)));
            range = new Range(new String("a"), new String("z"));
            Assert.True(range.Include(new String("g")));
            Assert.False(range.Include(new String("1")));
        }

        [Test]
        public void TestToString()
        {
            var range = new Range(new Fixnum(0), new Fixnum(2));
            Assert.That(range.ToString, Is.EqualTo("0..2"));

            range = new Range(new String("a"), new String("z"));
            Assert.That(range.ToString, Is.EqualTo("a..z"));

            range = new Range(new Fixnum(0), new Fixnum(2), true);
            Assert.That(range.ToString, Is.EqualTo("0...2"));

            range = new Range(new String("a"), new String("z"), true);
            Assert.That(range.ToString, Is.EqualTo("a...z"));
        }

        [Test]
        public void TestInspect()
        {
            var range = new Range(new Fixnum(0), new Fixnum(2));
            Assert.That(range.Inspect, Is.EqualTo("0..2"));

            range = new Range(new String("a"), new String("z"));
            Assert.That(range.Inspect, Is.EqualTo("\"a\"..\"z\""));

            range = new Range(new Fixnum(0), new Fixnum(2), true);
            Assert.That(range.Inspect, Is.EqualTo("0...2"));

            range = new Range(new String("a"), new String("z"), true);
            Assert.That(range.Inspect, Is.EqualTo("\"a\"...\"z\""));
        }
        [Test]
        public void TestEquals()
        {
            var range = new Range(new Fixnum(0), new Fixnum(2));
            Assert.True(range.Equals(range));
            var rangeDifferent = new Range(new Fixnum(0), new Fixnum(3));
            Assert.False(range.Equals(rangeDifferent));
            Assert.False(range.Equals(new Fixnum(0)));
        }
    }
}
