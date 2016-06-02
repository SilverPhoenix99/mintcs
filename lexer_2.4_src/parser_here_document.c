static int
parser_here_document(struct parser_params *parser, NODE *here)
{
    int c, func, indent = 0;
    const char *eos, *p, *pend;
    long len;
    VALUE str = 0;
    rb_encoding *enc = current_enc;

    eos = RSTRING_PTR(here->nd_lit);
    len = RSTRING_LEN(here->nd_lit) - 1;
    indent = (func = *eos++) & STR_FUNC_INDENT;

    if((c = nextc()) == -1)
    {
error:
        compile_error(PARSER_ARG "can't find string \"%s\" anywhere before EOF", eos);
restore:
        heredoc_restore(lex_strterm);
        return 0;
    }
    if(was_bol() && whole_match_p(eos, len, indent))
    {
        heredoc_restore(lex_strterm);
        return tSTRING_END;
    }

    if(!(func & STR_FUNC_EXPAND))
    {
        do
        {
            p = RSTRING_PTR(lex_lastline);
            pend = lex_pend;
            if(pend > p)
            {
                switch (pend[-1])
                {
                    case '\n':
                        if(--pend == p || pend[-1] != '\r')
                        {
                            pend++;
                            break;
                        }
                    case '\r':
                        --pend;
                }
            }

            if(heredoc_indent > 0)
            {
                long i = 0;
                while (p + i < pend && parser_update_heredoc_indent(parser, p[i]))
                {
                    i++;
                }
                heredoc_line_indent = 0;
            }

            if(str)
            {
                rb_str_cat(str, p, pend - p);
            }
            else
            {
                str = STR_NEW(p, pend - p);
            }
            if(pend < lex_pend)
            {
                rb_str_cat(str, "\n", 1);
            }
            lex_goto_eol(parser);
            if(heredoc_indent > 0)
            {
                set_yylval_str(str);
                flush_string_content(enc);
                return tSTRING_CONTENT;
            }
            if(nextc() == -1)
            {
                if(str)
                {
                    dispose_string(str);
                    str = 0;
                }
                goto error;
            }
        } while (!whole_match_p(eos, len, indent));
    }
    else
    {
        /*        int mb = ENC_CODERANGE_7BIT, *mbp = &mb;*/
        newtok();
        if(c == '#')
        {
            int t = parser_peek_variable_name(parser);
            if(t)
            {
                return t;
            }
            tokadd('#');
            c = nextc();
        }
        do
        {
            pushback(c);
            if((c = tokadd_string(func, '\n', 0, NULL, &enc)) == -1)
            {
                if(parser->eofp)
                {
                    goto error;
                }
                goto restore;
            }
            if(c != '\n')
            {
flush:
                set_yylval_str(STR_NEW3(tok(), toklen(), enc, func));
                flush_string_content(enc);
                return tSTRING_CONTENT;
            }
            tokadd(nextc());
            if(heredoc_indent > 0)
            {
                lex_goto_eol(parser);
                goto flush;
            }
            /*            if(mbp && mb == ENC_CODERANGE_UNKNOWN) mbp = 0;*/
            if((c = nextc()) == -1)
            {
                goto error;
            }
        } while (!whole_match_p(eos, len, indent));
        str = STR_NEW3(tok(), toklen(), enc, func);
    }
    heredoc_restore(lex_strterm);
    lex_strterm = NEW_STRTERM(-1, 0, 0);
    set_yylval_str(str);
    return tSTRING_CONTENT;
}