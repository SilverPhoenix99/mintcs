using System.Runtime.CompilerServices;
using Mint.Lex;
using Mint.Parse;
using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    [TestOf(typeof(Lexer))]
    internal class LexerTests
    {
        private static Lexer CreateLexer(string source,
                                         bool isFile = false,
                                        [CallerMemberName] string filename = nameof(LexerTests))
        {
            return new Lexer(filename, source, isFile);
        }

        [Test]
        public void TestLength()
        {
            var source = "some random text";
            var lexer = CreateLexer(source, isFile: false);
            Assert.That(lexer.DataLength, Is.EqualTo(source.Length));

            source = " a b c \x4 ";
            lexer.Data = source;
            Assert.That(lexer.DataLength, Is.EqualTo(source.IndexOf('\x4')));
        }

        [Test]
        public void TestShebang()
        {
            var lexer = CreateLexer("#! /bin/ruby", isFile: true);
            var token = lexer.NextToken();
            Assert.That(token.Type, Is.EqualTo(TokenType.EOF));
            Assert.That(lexer.Position, Is.GreaterThanOrEqualTo(lexer.Data.Length));
        }

        [Test]
        public void TestInitialComment()
        {
            var lexer = CreateLexer("# something\n   # another comment", isFile: true);
            var token = lexer.NextToken();
            Assert.That(token.Type, Is.EqualTo(TokenType.EOF));
            Assert.That(lexer.Position, Is.GreaterThanOrEqualTo(lexer.Data.Length));
        }
    }
}
