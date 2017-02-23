using System;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using Mint.Compilation;
using Mint.MethodBinding.Methods;
using Mint.Parse;
using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    internal class CompilationTests
    {
        public static Compiler CreateCompiler(string name, CallFrame frame = null)
        {
            name = $"(CompilationTests.{name})";
            frame = frame ?? new CallFrame(new Object());
            return new Compiler(name, frame);
        }

        public static iObject Eval(string code, CallFrame frame = null, [CallerMemberName] string name = "(eval)")
        {
            var ast = Parser.ParseString(name, code);
            var compiler = CreateCompiler(name, frame);
            var body = compiler.Compile(ast);
            var lambda = Expression.Lambda<Func<iObject>>(body);
            var function = lambda.Compile();
            return function();
        }

        [Test]
        public void TestInteger()
        {
            Assert.That(Eval("1"), Is.EqualTo(new Fixnum(1)));
            Assert.That(Eval("100"), Is.EqualTo(new Fixnum(100)));
            Assert.That(Eval("0x100"), Is.EqualTo(new Fixnum(0x100)));
            Assert.That(Eval("0b1010"), Is.EqualTo(new Fixnum(10)));
            Assert.That(Eval("0o2_10"), Is.EqualTo(new Fixnum(136)));
        }

        [Test]
        public void TestFloat()
        {
            Assert.That(Eval("1.0"), Is.EqualTo(new Float(1.0)));
            Assert.That(Eval("100.0"), Is.EqualTo(new Float(100.0)));
        }

        [Test]
        public void TestString()
        {
            Assert.That(Eval("\"a\""), Is.EqualTo(new String("a")));
            Assert.That(Eval("'a'"), Is.EqualTo(new String("a")));
            Assert.That(Eval("'a' 'b'"), Is.EqualTo(new String("ab")));
        }

        [Test]
        public void TestChar()
        {
            Assert.That(Eval("?a"), Is.EqualTo(new String("a")));
            Assert.That(Eval("?a 'z'"), Is.EqualTo(new String("az")));
        }

        [Test]
        public void TestSymbol()
        {
            Assert.That(Eval(":a"), Is.EqualTo(new Symbol("a")));
            Assert.That(Eval(":'a'"), Is.EqualTo(new Symbol("a")));
        }

        [Test]
        public void TestWords()
        {
            Assert.That(Eval("%w()"), Is.EqualTo(new Array()));

            var expected = new Array(new String("a"));
            Assert.That(Eval("%w(a)"), Is.EqualTo(expected));

            expected = new Array(new String("a"), new String("b"));
            Assert.That(Eval("%w(a b)"), Is.EqualTo(expected));
        }

        [Test]
        public void TestSymbolWords()
        {
            Assert.That(Eval("%i()"), Is.EqualTo(new Array()));

            var expected = new Array(new Symbol("a"));
            Assert.That(Eval("%i(a)"), Is.EqualTo(expected));

            expected = new Array(new Symbol("a"), new Symbol("b"));
            Assert.That(Eval("%i(a b)"), Is.EqualTo(expected));
        }

        [Test]
        public void TestIf()
        {
            var actual = Eval("if true then :a end");
            iObject expected = new Symbol("a");
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval("if false then :a else 'b' end");
            expected = new String("b");
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval("if true then :a else 'b' end");
            expected = new Symbol("a");
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval("'a' if nil");
            expected = new NilClass();
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval("'a' if :a");
            expected = new String("a");
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval("if false then 1 elsif true then 2 else 3 end");
            expected = new Fixnum(2);
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval("unless false then 10 end");
            expected = new Fixnum(10);
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval("unless nil then 10 else 20 end");
            expected = new Fixnum(10);
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval("unless true then 10 else 20 end");
            expected = new Fixnum(20);
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval("10 unless nil");
            expected = new Fixnum(10);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestRange()
        {
            var actual = Eval("1..2");
            Assert.That(actual, Is.EqualTo(new Range(1, 2)));
            Assert.That(((Range) actual).ExcludeEnd, Is.False);

            actual = Eval("3...4");
            Assert.That(actual, Is.EqualTo(new Range(3, 4, true)));
            Assert.That(((Range) actual).ExcludeEnd, Is.True);
        }

        [Test]
        public void TestArray()
        {
            Assert.That(Eval("[]"), Is.EqualTo(new Array()));

            var expected = new Array(new Fixnum(1), new Symbol("a"), new String("b"));
            Assert.That(Eval("[1, :a, 'b']"), Is.EqualTo(expected));
        }

        [Test]
        public void TestUMinusNum()
        {
            Assert.That(Eval("-1"), Is.EqualTo(new Fixnum(-1)));
            Assert.That(Eval("-1.0"), Is.EqualTo(new Float(-1.0)));
        }

        [Test]
        public void TestNot()
        {
            Assert.That(Eval("not 1"), Is.EqualTo(new FalseClass()));
            Assert.That(Eval("not true"), Is.EqualTo(new FalseClass()));
            Assert.That(Eval("not false"), Is.EqualTo(new TrueClass()));
            Assert.That(Eval("not nil"), Is.EqualTo(new TrueClass()));
            Assert.That(Eval("not ()"), Is.EqualTo(new TrueClass()));
        }

        [Test]
        public void TestOr()
        {
            Assert.That(Eval("1 || :a"), Is.EqualTo(new Fixnum(1)));
            Assert.That(Eval("nil || :a"), Is.EqualTo(new Symbol("a")));
            Assert.That(Eval("false || true"), Is.EqualTo(new TrueClass()));
            Assert.That(Eval("true || nil"), Is.EqualTo(new TrueClass()));
        }

        [Test]
        public void TestAnd()
        {
            Assert.That(Eval("1 && :a"), Is.EqualTo(new Symbol("a")));
            Assert.That(Eval("nil && :a"), Is.EqualTo(new NilClass()));
            Assert.That(Eval("false && true"), Is.EqualTo(new FalseClass()));
            Assert.That(Eval("true && nil"), Is.EqualTo(new NilClass()));
        }

        [Test]
        public void TestNotOperator()
        {
            Assert.That(Eval("!1"), Is.EqualTo(new FalseClass()));
            Assert.That(Eval("!true"), Is.EqualTo(new FalseClass()));
            Assert.That(Eval("!false"), Is.EqualTo(new TrueClass()));
            Assert.That(Eval("!nil"), Is.EqualTo(new TrueClass()));
            Assert.That(Eval("!()"), Is.EqualTo(new TrueClass()));
        }

        [Test]
        public void TestEquals()
        {
            Assert.That(Eval("1 == 1"), Is.EqualTo(new TrueClass()));
            Assert.That(Eval("1 == :a"), Is.EqualTo(new FalseClass()));
            Assert.That(Eval("1 == nil"), Is.EqualTo(new FalseClass()));
            Assert.That(Eval("nil == nil"), Is.EqualTo(new TrueClass()));
            Assert.That(Eval("nil == ()"), Is.EqualTo(new TrueClass()));
            Assert.That(Eval("() == ()"), Is.EqualTo(new TrueClass()));
        }

        [Test]
        public void TestNotEquals()
        {
            Assert.That(Eval("1 != 1"), Is.EqualTo(new FalseClass()));
            Assert.That(Eval("1 != :a"), Is.EqualTo(new TrueClass()));
            Assert.That(Eval("1 != nil"), Is.EqualTo(new TrueClass()));
            Assert.That(Eval("nil != nil"), Is.EqualTo(new FalseClass()));
            Assert.That(Eval("nil != ()"), Is.EqualTo(new FalseClass()));
            Assert.That(Eval("() != ()"), Is.EqualTo(new FalseClass()));
        }

        [Test]
        public void TestSelf()
        {
            var self = Eval("self");

            Assert.That(self, Is.Not.Null);
            Assert.That(self, Is.TypeOf(typeof(Object)));
            Assert.That(self.Class.FullName, Is.EqualTo("Object"));
        }

        [Test]
        public void TestIdentifierAndAssign()
        {
            Assert.That(Eval("a = 1; a"), Is.EqualTo(new Fixnum(1)));
        }

        [Test]
        public void TestIndexer()
        {
            var frame = new CallFrame(new Object());
            Eval("a = []", frame);
            var array = frame.Locals[new Symbol("a")].Value as Array;

            Assert.That(array, Is.Not.Null);
            Assert.That(Eval("a[0] = 1", frame), Is.EqualTo(array[0]));
            Assert.That(Eval("a[0]", frame), Is.EqualTo(array[0]));
        }
    }
}
