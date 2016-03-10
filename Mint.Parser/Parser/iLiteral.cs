﻿namespace Mint.Parser
{
    interface iLiteral
    {
        uint         BraceCount          { get; set; }
        bool         CanLabel            { get; }
        bool         Dedents             { get; }
        string       Delimiter           { get; }
        string       EndDelimiter        { get; }
        int          Indent              { get; set; }
        bool         Interpolates        { get; }
        int          LineIndent          { get; set; }
        bool         IsRegexp            { get; }
        TokenType    Type                { get; }
        string       UnterminatedMessage { get; }
        bool         IsWords             { get; }
        int          ContentStart        { get; set; }
        Lexer.States State               { get; }
        bool         WasContent          { get; set; }
        int          Nesting             { get; set; }
        bool         IsNested            { get; }

        void CommitIndent();
        bool IsDelimiter(string token);
        uint TranslateDelimiter(char token);
    }
}