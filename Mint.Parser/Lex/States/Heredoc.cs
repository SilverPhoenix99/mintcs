using System.Text.RegularExpressions;
using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Lex.States
{
    internal class HeredocDelimiter
    {
        private static readonly Regex IDENTIFIER =
            new Regex("^<<([-~]?)([\"'`]?)(.+)(?:\\2)$", RegexOptions.Compiled);

        private const string WS = @"[\t\v\f\r ]*";

        public string Identifier { get; }

        public bool HasInterpolation { get; }

        public TokenType BeginType { get; }

        public bool Dedents { get; }

        public Regex EndMatcher { get; }

        public HeredocDelimiter(string text)
        {
            /*
               indentationType:
                 '\0' (empty) doesn't allow whitespace before delimiter,
                              i.e., the delimiter must be isolated in a line

                 '-' allows whitespace before delimiter

                 '~' allows whitespace before delimiter
                     removes left margin:
                       margin = heredoc_content.scan(/^ +/).map(&:size).min
                       heredoc_content.gsub(/^ {#{margin}}/, '')
            */

            var matches = IDENTIFIER.Match(text).Groups;

            Identifier = matches[3].Value;

            var match = MatchIdDelimiter(matches[2].Value);
            HasInterpolation = match != '\'';
            BeginType = match == '`' ? tXSTRING_BEG : tSTRING_BEG;

            match = MatchIndentationType(matches[1].Value);

            Dedents = match == '~';

            var allowsWhitespacePrefix = Dedents || match == '-';
            EndMatcher = CreateEndMatcher(Identifier, allowsWhitespacePrefix);
        }

        private static char MatchIndentationType(string match) => match.Length == 0 ? '\0' : match[0];

        private static char MatchIdDelimiter(string match) => match.Length == 0 ? '"' : match[0];

        private static Regex CreateEndMatcher(string id, bool allowsWhitespacePrefix)
        {
            var ws = allowsWhitespacePrefix ? WS : "";
            return new Regex($@"\G{ws}{id}(\r?\n|$)");
        }
    }

    internal partial class Heredoc : Literal
    {
        private readonly HeredocDelimiter delimiter;
        private readonly int restorePosition;

        public Heredoc(Lexer lexer, int ts, int te)
            : base(lexer, lexer.NextLinePosition())
        {
            var text = lexer.TextAt(ts, te);
            delimiter = new HeredocDelimiter(text);
            this.restorePosition = te;

            BeginToken = lexer.GenerateToken(delimiter.BeginType, ts, te);
            BeginToken.Properties["has_interpolation"] = delimiter.HasInterpolation;
        }

        protected bool IsEndDelimiter() => delimiter.EndMatcher.IsMatch(Lexer.Data, Lexer.Position);
    }
}