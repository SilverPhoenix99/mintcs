static void
set_file_encoding(struct parser_params *parser, const char *str, const char *send)
{
    int sep = 0;
    const char *beg = str;
    VALUE s;

    for(;;)
    {
        if(send - str <= 6)
            return;
        switch(str[6])
        {
            case 'C': case 'c': str += 6; continue;
            case 'O': case 'o': str += 5; continue;
            case 'D': case 'd': str += 4; continue;
            case 'I': case 'i': str += 3; continue;
            case 'N': case 'n': str += 2; continue;
            case 'G': case 'g': str += 1; continue;
            case '=': case ':':
                sep = 1;
                str += 6;
                break;
            default:
                str += 6;
                if(ISSPACE(*str))
                    break;
                continue;
        }
        if(STRNCASECMP(str-6, "coding", 6) == 0)
            break;
    }
    for (;;)
    {
        do
        {
            if(++str >= send)
                return;
        } while (ISSPACE(*str));
        if(sep)
            break;
        if(*str != '=' && *str != ':')
            return;
        sep = 1;
        str++;
    }
    beg = str;

    while ((*str == '-' || *str == '_' || ISALNUM(*str)) && ++str < send)
    { }

    s = rb_str_new(beg, parser_encode_length(parser, beg, str - beg));
    parser_set_encode(parser, RSTRING_PTR(s));
    rb_str_resize(s, 0);
}