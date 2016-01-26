using System.Text.RegularExpressions;
using static mint.Compiler.TokenType;

namespace mint.Compiler
{
    class Heredoc : iLiteral
    {
        private char indent_type;
        private char id_delimiter;
        private Regex regex;

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
            indent_type = token.Length == 0 ? '\0' : token[0];

            token = matches[2].Value;
            id_delimiter = token.Length == 0 ? '\0' : token[0];

            Delimiter = matches[3].Value;

            Restore = restore;
            Indent = -1;
            LineIndent = 0;
        }

        public uint         BraceCount     { get; set; }
        public bool         CanLabel       => false;
        public int          ContentStart   { get; set; }
        public bool         Dedents        => indent_type == '~';
        public string       Delimiter      { get; }
        public string       EndDelimiter   => Delimiter;
        public int          Indent         { get; set; }
        public bool         Interpolates   => id_delimiter != '\'';
        public bool         IsRegexp       => false;
        public bool         IsWords        => false;
        public int          LineIndent     { get; set; }
        public int          Restore        { get; }
        public Lexer.States State          => Lexer.States.HEREDOC_DELIMITER;
        public TokenType    Type           => id_delimiter == '`' ? tXSTRING_BEG : tSTRING_BEG;
        public string UnterminatedMessage  => $"can't find string {Delimiter} anywhere before EOF";
        public bool         WasContent     { get { return false; } set { } }
        public int          Nesting        { get { return 0; } set { } }
        public bool         IsNested       => false;

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

        public bool IsDelimiter(string delimiter)
        {
            if(regex == null)
            {
                regex = new Regex(indent_type == '\0'
                                    ? $"^{Delimiter}$"
                                    : $@"^[\t\v\f\r ]*{Delimiter}\r?$");
            }
            
            return regex.IsMatch(delimiter);
        }

        public uint TranslateDelimiter(char delimiter) => delimiter;

        private static readonly Regex HEREDOC_IDENT = new Regex("^<<([-~]?)([\"'`]?)(.+)(?:\\2)$", RegexOptions.Compiled);
    }
}
