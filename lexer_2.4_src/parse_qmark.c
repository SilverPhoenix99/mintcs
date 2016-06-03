static int
parse_qmark(struct parser_params *parser)
{
    rb_encoding *enc;
    register int c;

    if(IS_lex_state(EXPR_END | EXPR_ENDARG | EXPR_ENDFN))
    {
        SET_LEX_STATE(EXPR_BEG);
        return '?';
    }
    c = nextc();
    if(c == -1)
    {
        compile_error(PARSER_ARG "incomplete character syntax");
        return 0;
    }
    if(rb_enc_isspace(c, current_enc))
    {
        if(!IS_lex_state(EXPR_ARG | EXPR_CMDARG))
        {
            int c2 = 0;
            switch (c)
            {
                case ' ':
                    c2 = 's';
                    break;
                case '\n':
                    c2 = 'n';
                    break;
                case '\t':
                    c2 = 't';
                    break;
                case '\v':
                    c2 = 'v';
                    break;
                case '\r':
                    c2 = 'r';
                    break;
                case '\f':
                    c2 = 'f';
                    break;
            }
            if(c2)
            {
                // gives error later
                rb_warn1("invalid character syntax; use ?\\%c", WARN_I(c2));
            }
        }
ternary:
        pushback(c);
        SET_LEX_STATE(EXPR_BEG);
        return '?';
    }
    newtok();
    enc = current_enc;
    if(!parser_isascii())
    {
        if(tokadd_mbchar(c) == -1)
            return 0;
    }
    else if((rb_enc_isalnum(c, current_enc) || c == '_') &&
             lex_p < lex_pend && is_identchar(lex_p, lex_pend, current_enc))
    {
        goto ternary;
    }
    else if(c == '\\')
    {
        if(peek('u'))
        {
            nextc();
            c = parser_tokadd_utf8(parser, &enc, 0, 0, 0);
            if(0x80 <= c)
            {
                tokaddmbc(c, enc);
            }
            else
            {
                tokadd(c);
            }
        }
        else if(!lex_eol_p() && !(c = *lex_p, ISASCII(c)))
        {
            nextc();
            if(tokadd_mbchar(c) == -1)
                return 0;
        }
        else
        {
            c = read_escape(0, &enc);
            tokadd(c);
        }
    }
    else
    {
        tokadd(c);
    }
    tokfix();
    set_yylval_str(STR_NEW3(tok(), toklen(), enc, 0));
    SET_LEX_STATE(EXPR_END);
    return tCHAR;
}