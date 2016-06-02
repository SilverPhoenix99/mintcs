static int
parser_parse_string(struct parser_params *parser, NODE *quote)
{
    int func = (int)quote->nd_func;
    int term = nd_term(quote);
    int paren = nd_paren(quote);
    int c, space = 0;
    rb_encoding *enc = current_enc;

    if(func == -1)
    {
        return tSTRING_END;
    }
    c = nextc();
    if((func & STR_FUNC_QWORDS) && ISSPACE(c))
    {
        do
        {
            c = nextc();
        } while (ISSPACE(c));
        space = 1;
    }
    if(c == term && !quote->nd_nest)
    {
        if(func & STR_FUNC_QWORDS)
        {
            quote->nd_func = -1;
            return ' ';
        }
        if(!(func & STR_FUNC_REGEXP))
        {
            return tSTRING_END;
        }
        set_yylval_num(regx_options());
        return tREGEXP_END;
    }
    if(space)
    {
        pushback(c);
        return ' ';
    }
    newtok();
    if((func & STR_FUNC_EXPAND) && c == '#')
    {
        int t = parser_peek_variable_name(parser);
        if(t)
        {
            return t;
        }
        tokadd('#');
        c = nextc();
    }
    pushback(c);
    if(tokadd_string(func, term, paren, &quote->nd_nest, &enc) == -1)
    {
        ruby_sourceline = nd_line(quote);
        if(func & STR_FUNC_REGEXP)
        {
            if(parser->eofp)
            {
                compile_error(PARSER_ARG "unterminated regexp meets end of file");
            }
            return tREGEXP_END;
        }
        else
        {
            if(parser->eofp)
            {
                compile_error(PARSER_ARG "unterminated string meets end of file");
            }
            return tSTRING_END;
        }
    }

    tokfix();
    set_yylval_str(STR_NEW3(tok(), toklen(), enc, func));

    return tSTRING_CONTENT;
}