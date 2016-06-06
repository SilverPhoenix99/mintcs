using System;
using System.Text.RegularExpressions;
using Mint.Lex.States;
using static Mint.Parse.TokenType;

namespace Mint.Parse
{
    internal class Heredoc : iLiteral
    {
        private static readonly Regex HEREDOC_IDENT =
            new Regex("^<<([-~]?)([\"'`]?)(.+)(?:\\2)$", RegexOptions.Compiled);

        private readonly char indentType;
        private readonly char idDelimiter;
        private Regex regex;

        public uint BraceCount { get; set; }
        public bool CanLabel => false;
        public int ContentStart  { get; set; }
        public bool Dedents => indentType == '~';
        public int Indent { get; private set; }
        public bool Interpolates => idDelimiter != '\'';
        public bool IsRegexp => false;
        public bool IsWords => false;
        public int LineIndent { get; set; }
        public int Restore { get; }
        public State State //=> Lexer.States.HEREDOC_DELIMITER;
        {
            get { throw new NotImplementedException(); }
        }
        public TokenType Type => idDelimiter == '`' ? tXSTRING_BEG : tSTRING_BEG;
        public string UnterminatedMessage => $"can't find string {Delimiter} anywhere before EOF";
        public bool WasContent { get { return false; } set { } }
        public int Nesting { get { return 0; } set { } }
        public bool IsNested => false;
        private string Delimiter { get; }
        private Regex Regex => regex ?? (regex = CreateRegex());

        public Heredoc(string token, int restore)
        {
            /*
               indent_type '\0' (empty) doesn't allow whitespace before delimiter,
               i.e., the delimiter must be isolated in a line
  
               indent_type '-' allows whitespace before delimiter
  
               indent type '~' removes left margin:
                   margin = heredoc_content.scan(/^ +/).map(&:size).min
                   heredoc_content.gsub(/^ {#{margin}}/, '')
               it also allows whitespace before delimiter
            */

            var matches = HEREDOC_IDENT.Match(token).Groups;
            token = matches[1].Value;
            indentType = token.Length == 0 ? '\0' : token[0];

            token = matches[2].Value;
            idDelimiter = token.Length == 0 ? '\0' : token[0];

            Delimiter = matches[3].Value;

            Restore = restore;
            Indent = -1;
            LineIndent = 0;
        }

        public void CommitIndent()
        {
            if(!Dedents)
            {
                return;
            }

            if(Indent == -1 || (0 <= LineIndent && LineIndent < Indent))
            {
                Indent = LineIndent;
            }
            LineIndent = -1;
        }

        public bool IsDelimiter(string delimiter) => Regex.IsMatch(delimiter);

        public uint TranslateDelimiter(char delimiter) => delimiter;

        private Regex CreateRegex()
        {
            return new Regex(indentType == '\0'
                           ? $"^{Delimiter}\r?$"
                           : $@"^[\t\v\f\r ]*{Delimiter}\r?$");
        }

    }
}
