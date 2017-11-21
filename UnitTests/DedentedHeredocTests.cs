using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    internal class DedentedHeredocTests
    {
        private static readonly string[] IDENTIFIERS = { "eos", "'eos'", "\"eos\"" };

        private static void AssertDedentedHeredoc(string expect, string original)
        {
            foreach(var eos in IDENTIFIERS)
            {
                var actual = CompilationTests.Eval($"<<~{eos}\n{original}eos\n").ToString();
                var expected = CompilationTests.Eval($"<<-{eos}\n{expect}eos\n").ToString();

                string msg = $"with {eos}";
                Assert.That(actual, Is.EqualTo(expected), msg);
            }
        }

        [Test]
        public void WithoutIndentation()
        {
            const string result = " y\n"
                                + "z\n";
            const string expect = result;
            AssertDedentedHeredoc(expect, result);
        }

        [Test]
        public void WithIndentation()
        {
            const string result = "         a\n"
                                + "\tb\n";
            const string expect = " a\n"
                                + "b\n";
            AssertDedentedHeredoc(expect, result);
        }

        [Test]
        public void WithBlankLessThanIndentedLine()
        {
            const string result = "    a\n"
                                + "  \n"
                                + "    b\n";
            const string expect = "a\n"
                                + "\n"
                                + "b\n";
            AssertDedentedHeredoc(expect, result);
        }

        [Test]
        public void WithBlankLessThanIndentedLineEscaped()
        {
            const string result = "    a\n"
                                + "\\ \\ \n"
                                + "    b\n";
            const string expect = result;
            AssertDedentedHeredoc(expect, result);
        }

        [Test]
        public void WithBlankMoreThanIndentedLine()
        {
            const string result = "    a\n"
                                + "      \n"
                                + "    b\n";
            const string expect = "a\n"
                                + "  \n"
                                + "b\n";
            AssertDedentedHeredoc(expect, result);
        }

        [Test]
        public void WithBlankMoreThanIndentedLineEscaped()
        {
            const string result = "    a\n"
                                + "\\ \\ \\ \\ \\ \\ \n"
                                + "    b\n";
            const string expect = result;
            AssertDedentedHeredoc(expect, result);
        }

        [Test]
        public void WithEmptyLine()
        {
            const string result = "      This would contain specially formatted text.\n"
                                + "\n"
                                + "      That might span many lines\n";
            const string expect = "This would contain specially formatted text.\n"
                                + "\n"
                                + "That might span many lines\n";
            AssertDedentedHeredoc(expect, result);
        }

        [Test]
        public void WithInterpolatedExpression()
        {
            const string result = "  #{1}a\n"
                                + " zy\n";
            const string expect = " #{1}a\n"
                                + "zy\n";
            AssertDedentedHeredoc(expect, result);
        }

        [Test]
        public void WithInterpolatedString()
        {
            const string result = "  \\#{1}a\n"
                                + " zy\n";
            const string expect = " \\#{1}a\n"
                                + "zy\n";
            AssertDedentedHeredoc(expect, result);
        }
    }
}
