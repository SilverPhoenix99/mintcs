static int
comment_at_top(struct parser_params *parser)
{
    const char *p = lex_pbeg, *pend = lex_p - 1;
    if(parser->line_count != (parser->has_shebang ? 2 : 1))
        return 0;
    while(p < pend)
    {
        if(!ISSPACE(*p))
            return 0;
        p++;
    }
    return 1;
}