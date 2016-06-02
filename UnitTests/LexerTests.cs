using System.Runtime.CompilerServices;
using Mint.Lex;
using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    [TestOf(typeof(Lexer))]
    internal class LexerTests
    {
        private static Lexer CreateLexer(string source, [CallerMemberName] string filename = nameof(LexerTests))
        {
            return new Lexer(filename) { Data = source };
        }

        [Test]
        public void TestLength()
        {
            var source = "some random text";
            var lexer = CreateLexer(source);
            Assert.That(lexer.Length, Is.EqualTo(source.Length + 1));

            source = " a b c \x4 ";
            lexer.Data = source;
            Assert.That(lexer.Length, Is.EqualTo(source.IndexOf('\x4')));
        }

        [Test]
        public void TestShebang()
        {
            var lexer = CreateLexer("#! /bin/ruby");
            var token = lexer.NextToken();
            Assert.That(token, Is.Null);
            Assert.That(lexer.Position, Is.GreaterThanOrEqualTo(lexer.Data.Length));
        }

        [Test]
        public void TestInitialComment()
        {
            var lexer = CreateLexer("# something\n   # another comment");
            var token = lexer.NextToken();
            Assert.That(token, Is.Null);
            Assert.That(lexer.Position, Is.GreaterThanOrEqualTo(lexer.Data.Length));
        }
    }
}
