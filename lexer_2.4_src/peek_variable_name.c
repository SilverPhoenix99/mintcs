static int
parser_peek_variable_name(struct parser_params *parser)
{
    int c;
    const char *p = lex_p;

    if(p + 1 >= lex_pend)
    {
        return 0;
    }
    c = *p++;
    switch(c)
    {
        case '$':
            if((c = *p) == '-')
            {
                if (++p >= lex_pend)
                    return 0;
                c = *p;
            }
            else if (is_global_name_punct(c) || ISDIGIT(c))
            {
                return tSTRING_DVAR;
            }
            break;

        case '@':
            if((c = *p) == '@')
                {
                if(++p >= lex_pend)
                    return 0;
                c = *p;
            }
            break;

        case '{':
            lex_p = p;
            command_start = TRUE;
            return tSTRING_DBEG;

        default:
            return 0;
    }
    if(!ISASCII(c) || c == '_' || ISALPHA(c))
        return tSTRING_DVAR;
    return 0;
}