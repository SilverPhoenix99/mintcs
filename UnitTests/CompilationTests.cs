using System;
using System.Linq.Expressions;
using Mint.Compilation;
using Mint.Parse;
using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    public class CompilationTests
    {
        private static Compiler CreateCompiler(string name, string fragment)
        {
            name = $"(CompilationTests.{name})";
            var binding = new Closure(new Object());
            var ast = Parser.Parse(name, fragment);
            return new Compiler(name, binding, ast);
        }

        private static iObject Eval(string name, string code)
        {
            var compiler = CreateCompiler(name, code);
            var body = compiler.Compile();
            var lambda = Expression.Lambda<Func<iObject>>(body);
            var function = lambda.Compile();
            return function();
        }

        [Test]
        public void TestInteger()
        {
            Assert.That(Eval(nameof(TestInteger), "1"), Is.EqualTo(new Fixnum(1)));
            Assert.That(Eval(nameof(TestInteger), "100"), Is.EqualTo(new Fixnum(100)));
            Assert.That(Eval(nameof(TestInteger), "0x100"), Is.EqualTo(new Fixnum(0x100)));
            Assert.That(Eval(nameof(TestInteger), "0b1010"), Is.EqualTo(new Fixnum(10)));
            Assert.That(Eval(nameof(TestInteger), "0o2_10"), Is.EqualTo(new Fixnum(136)));
        }

        [Test]
        public void TestFloat()
        {
            Assert.That(Eval(nameof(TestFloat), "1.0"), Is.EqualTo(new Float(1.0)));
            Assert.That(Eval(nameof(TestFloat), "100.0"), Is.EqualTo(new Float(100.0)));
        }

        [Test]
        public void TestString()
        {
            Assert.That(Eval(nameof(TestString), "\"a\""), Is.EqualTo(new String("a")));
            Assert.That(Eval(nameof(TestString), "'a'"), Is.EqualTo(new String("a")));
            Assert.That(Eval(nameof(TestString), "'a' 'b'"), Is.EqualTo(new String("ab")));
        }

        [Test]
        public void TestChar()
        {
            Assert.That(Eval(nameof(TestString), "?a"), Is.EqualTo(new String("a")));
            Assert.That(Eval(nameof(TestString), "?a 'z'"), Is.EqualTo(new String("az")));
        }

        [Test]
        public void TestSymbol()
        {
            Assert.That(Eval(nameof(TestSymbol), ":a"), Is.EqualTo(new Symbol("a")));
            Assert.That(Eval(nameof(TestSymbol), ":'a'"), Is.EqualTo(new Symbol("a")));
        }

        [Test]
        public void TestWords()
        {
            var actual = Eval(nameof(TestWords), "%w()");
            var expected = new Array();
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval(nameof(TestWords), "%w(a)");
            expected = new Array(new String("a"));
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval(nameof(TestWords), "%w(a b)");
            expected = new Array(new String("a"), new String("b"));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestSymbolWords()
        {
            var actual = Eval(nameof(TestWords), "%i()");
            var expected = new Array();
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval(nameof(TestWords), "%i(a)");
            expected = new Array(new Symbol("a"));
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval(nameof(TestWords), "%i(a b)");
            expected = new Array(new Symbol("a"), new Symbol("b"));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestIf()
        {
            var actual = Eval(nameof(TestIf), "if true then :a end");
            iObject expected = new Symbol("a");
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval(nameof(TestIf), "if false then :a else 'b' end");
            expected = new String("b");
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval(nameof(TestIf), "if true then :a else 'b' end");
            expected = new Symbol("a");
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval(nameof(TestIf), "'a' if nil");
            expected = new NilClass();
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval(nameof(TestIf), "'a' if :a");
            expected = new String("a");
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval(nameof(TestIf), "if false then 1 elsif true then 2 else 3 end");
            expected = new Fixnum(2);
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval(nameof(TestIf), "unless false then 10 end");
            expected = new Fixnum(10);
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval(nameof(TestIf), "unless nil then 10 else 20 end");
            expected = new Fixnum(10);
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval(nameof(TestIf), "unless true then 10 else 20 end");
            expected = new Fixnum(20);
            Assert.That(actual, Is.EqualTo(expected));

            actual = Eval(nameof(TestIf), "10 unless nil");
            expected = new Fixnum(10);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
