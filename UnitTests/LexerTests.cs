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

        [Test]
        public void TestFid()
        {
            var lexer = CreateLexer("class c!\n  def f!\n    a!.b! v!", isFile: true);

            AssertToken(lexer.NextToken(), TokenType.kCLASS);
            AssertToken(lexer.NextToken(), TokenType.tFID, "c!");
            AssertToken(lexer.NextToken(), TokenType.kNL);

            AssertToken(lexer.NextToken(), TokenType.kDEF);
            AssertToken(lexer.NextToken(), TokenType.tFID, "f!");
            AssertToken(lexer.NextToken(), TokenType.kNL);

            AssertToken(lexer.NextToken(), TokenType.tFID, "a!");
            AssertToken(lexer.NextToken(), TokenType.kDOT);
            AssertToken(lexer.NextToken(), TokenType.tFID, "b!");
            AssertToken(lexer.NextToken(), TokenType.tFID, "v!");

            Assert.That(lexer.Position, Is.GreaterThanOrEqualTo(lexer.Data.Length));
        }

        private static void AssertToken(Token token, TokenType type, string text = null)
        {
            Assert.That(token.Type, Is.EqualTo(type));

            if(text != null)
            {
                Assert.That(token.Text, Is.EqualTo(text));
            }
        }
    }
}
