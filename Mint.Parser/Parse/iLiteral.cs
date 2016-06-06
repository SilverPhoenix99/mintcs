using Mint.Lex.States;

namespace Mint.Parse
{
    internal interface iLiteral
    {
        uint         BraceCount          { get; set; }
        bool         CanLabel            { get; }
        bool         Dedents             { get; }
        int          Indent              { get; }
        bool         Interpolates        { get; }
        int          LineIndent          { get; set; }
        bool         IsRegexp            { get; }
        string       UnterminatedMessage { get; }
        bool         IsWords             { get; }
        int          ContentStart        { get; set; }
        State        State               { get; }
        bool         WasContent          { get; set; }
        int          Nesting             { get; set; }
        bool         IsNested            { get; }

        void CommitIndent();
        bool IsDelimiter(string token);
        uint TranslateDelimiter(char token);
    }
}
